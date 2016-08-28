using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public class ExclusiveController<TParam>
    {
        public event Action<TParam> Changed;

        public void Raise(TParam param)
        {
            var e = Changed;
            if (e != null)
            {
                e(param);
            }
        }
    }
}
