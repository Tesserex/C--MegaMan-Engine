using MegaMan.Common;
using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Rendering
{
    public interface IRenderingContext
    {
        Int32 LoadTexture(FilePath texturePath);
        Int32 CreateColorTexture(int red, int green, int blue);
        void Begin();
        void End();
        void EnableLayer(Int32 layer);
        void DisableLayer(Int32 layer);
        bool IsLayerEnabled(Int32 layer);
        void SetOpacity(float opacity);
        float GetOpacity();

        void Draw(Int32 textureId, Int32 layer, Point position, Rectangle? sourceRect = null);
    }
}
