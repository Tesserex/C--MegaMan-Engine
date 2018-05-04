using MegaMan.Common;
using MegaMan.Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MegaRect = MegaMan.Common.Geometry.Rectangle;
using XnaRect = Microsoft.Xna.Framework.Rectangle;
using XnaColor = Microsoft.Xna.Framework.Color;
using MegaMan.Common.Rendering;

namespace MegaMan.Engine.Rendering
{
    public class XnaRenderingContext : IRenderingContext
    {
        private const int LAYER_COUNT = 6;

        private GraphicsDevice _graphicsDevice;
        private List<Texture2D> _loadedTextures;
        private Dictionary<FilePath, IResourceImage> _loadedResources;
        private Dictionary<int, List<Texture2D>> _paletteSwaps;
        private SpriteBatch[] _spriteBatchLayers;
        private bool[] _layersEnabled;
        private float _opacity;
        private XnaColor _opacityColor;

        public XnaRenderingContext(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException("graphicsDevice");

            _graphicsDevice = graphicsDevice;

            _loadedTextures = new List<Texture2D>();
            _loadedResources = new Dictionary<FilePath, IResourceImage>();
            _paletteSwaps = new Dictionary<int, List<Texture2D>>();
            _spriteBatchLayers = new SpriteBatch[LAYER_COUNT];
            _layersEnabled = new bool[LAYER_COUNT];

            CreateAllLayers();
            EnableAllLayers();
        }

        private void EnableAllLayers()
        {
            for (int i = 0; i < LAYER_COUNT; i++)
            {
                _layersEnabled[i] = true;
            }
        }

        private void CreateAllLayers()
        {
            for (int i = 0; i < LAYER_COUNT; i++)
            {
                _spriteBatchLayers[i] = new SpriteBatch(_graphicsDevice);
            }
        }

        public void Begin()
        {
            for (int i = 0; i < LAYER_COUNT; i++)
            {
                if (IsLayerEnabled(i))
                    _spriteBatchLayers[i].Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            }
        }

        public void End()
        {
            for (int i = 0; i < LAYER_COUNT; i++)
            {
                if (IsLayerEnabled(i))
                    _spriteBatchLayers[i].End();
            }
        }

        public void EnableLayer(int layer)
        {
            if (layer < 0 || layer >= LAYER_COUNT)
                throw new ArgumentException("Rendering context does not support that many layers.");

            _layersEnabled[layer] = true;
        }

        public void DisableLayer(int layer)
        {
            if (layer < 0 || layer >= LAYER_COUNT)
                throw new ArgumentException("Rendering context does not support that many layers.");

            _layersEnabled[layer] = false;
        }

        public bool IsLayerEnabled(int layer)
        {
            if (layer < 0 || layer >= LAYER_COUNT)
                throw new ArgumentException("Rendering context does not support that many layers.");

            return _layersEnabled[layer];
        }

        public void SetOpacity(float opacity)
        {
            _opacity = opacity;
            _opacityColor = new XnaColor(_opacity, _opacity, _opacity);
        }

        public float GetOpacity()
        {
            return _opacity;
        }

        public IResourceImage LoadResource(FilePath texturePath, byte[] textureData, string paletteName = null)
        {
            if (!_loadedResources.ContainsKey(texturePath))
            {
                using (var stream = new MemoryStream(textureData))
                {
                    var texture = Texture2D.FromStream(_graphicsDevice, stream);
                    var resource = AddTexture(texture, paletteName);
                    _loadedResources[texturePath] = resource;
                }
            }

            return _loadedResources[texturePath];
        }

        public IResourceImage CreateColorResource(Common.Color color)
        {
            var texture = new Texture2D(Engine.Instance.GraphicsDevice, 1, 1);
            texture.SetData(new XnaColor[] { new XnaColor(color.R, color.G, color.B, color.A) });
            return AddTexture(texture, null);
        }

        private IResourceImage AddTexture(Texture2D texture, string palette)
        {
            var nextIndex = _loadedTextures.Count;
            _loadedTextures.Add(texture);

            return new XnaResourceImage(nextIndex, palette, texture.Width, texture.Height);
        }

        public void Draw(IResourceImage resource, int layer, MegaMan.Common.Geometry.Point position, MegaRect? sourceRect = null, bool flipHorizontal = false, bool flipVertical = false)
        {
            if (!IsLayerEnabled(layer)) return;

            var texture = _loadedTextures[resource.ResourceId];
            var batch = _spriteBatchLayers[layer];

            if (resource.PaletteName != null)
            {
                var palette = PaletteSystem.Get(resource.PaletteName);
                if (palette != null)
                {
                    VerifyPaletteSwaps(palette, resource.ResourceId, texture);
                    texture = this._paletteSwaps[resource.ResourceId][palette.CurrentIndex];
                }
            }

            var destination = new Vector2(position.X, position.Y);

            XnaRect? source = null;
            if (sourceRect != null)
                source = new XnaRect(sourceRect.Value.X, sourceRect.Value.Y, sourceRect.Value.Width, sourceRect.Value.Height);

            SpriteEffects effect = SpriteEffects.None;
            if (flipHorizontal) effect = SpriteEffects.FlipHorizontally;
            if (flipVertical) effect |= SpriteEffects.FlipVertically;

            batch.Draw(texture,
                destination, source,
                _opacityColor, 0,
                Vector2.Zero, 1, effect, 0);
        }

        private void VerifyPaletteSwaps(Palette palette, int id, Texture2D texture)
        {
            if (!this._paletteSwaps.ContainsKey(id))
            {
                _paletteSwaps[id] = GenerateSwappedTextures(palette, texture);
            }
        }

        private List<Texture2D> GenerateSwappedTextures(Palette palette, Texture2D image)
        {
            byte[] pixelData = new byte[image.Width * image.Height * 4];
            image.GetData(pixelData);

            var swappedPixels = palette.GetSwappedPixels(pixelData, true);

            var swappedTextures = new List<Texture2D>();

            foreach (var pixels in swappedPixels)
            {
                var texture = new Texture2D(_graphicsDevice, image.Width, image.Height);

                texture.SetData(pixels);

                swappedTextures.Add(texture);
            }

            return swappedTextures;
        }
    }
}
