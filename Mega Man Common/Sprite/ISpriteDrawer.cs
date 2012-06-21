using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common
{
    public interface ISpriteDrawer
    {
        void Draw();
    }

    public class GdiSpriteDrawer : ISpriteDrawer
    {
        public void Draw()
        {
            
        }
    }

    public class XnaSpriteDrawer : ISpriteDrawer
    {
        public void Draw()
        {
            
        }
    }
}
