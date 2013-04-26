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

namespace MegaMan.Engine.Rendering
{
    public class XnaRenderingContext : IRenderingContext
    {
        private const int LAYER_COUNT = 6;

        private GraphicsDevice _graphicsDevice;
        private List<Texture2D> _loadedTextures;
        private SpriteBatch[] _spriteBatchLayers;
        private bool[] _layersEnabled;
        private float _opacity;
        private Color _opacityColor;

        public XnaRenderingContext(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException("graphicsDevice");

            _graphicsDevice = graphicsDevice;

            _loadedTextures = new List<Texture2D>();
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
            _opacityColor = new Color(_opacity, _opacity, _opacity);
        }

        public float GetOpacity()
        {
            return _opacity;
        }

        public int LoadTexture(FilePath texturePath)
        {
            var texture = GetTextureFromPath(texturePath);
            return AddTexture(texture);
        }

        public int CreateColorTexture(int red, int green, int blue)
        {
            var texture = new Texture2D(Engine.Instance.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { new Color(red, green, blue) });
            return AddTexture(texture);
        }

        private int AddTexture(Texture2D texture)
        {
            var nextIndex = _loadedTextures.Count;
            _loadedTextures.Add(texture);
            return nextIndex;
        }

        public void Draw(int textureId, int layer, MegaMan.Common.Geometry.Point position, MegaRect? sourceRect = null)
        {
            var texture = _loadedTextures[textureId];
            var batch = _spriteBatchLayers[layer];

            var destination = new Vector2(position.X, position.Y);

            XnaRect? source = null;
            if (sourceRect != null)
                source = new XnaRect(sourceRect.Value.X, sourceRect.Value.Y, sourceRect.Value.Width, sourceRect.Value.Height);

            batch.Draw(texture,
                destination, source,
                _opacityColor, 0,
                Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        private Texture2D GetTextureFromPath(FilePath path)
        {
            StreamReader sr = new StreamReader(path.Absolute);
            return Texture2D.FromStream(_graphicsDevice, sr.BaseStream);
        }
    }
}
