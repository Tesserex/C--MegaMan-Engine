using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class VarsEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(VarsEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var varInfo = (VarsEffectPartInfo)info;

            if (varInfo.Call != null)
            {
                Query getVal = EffectParser.CompileQuery(varInfo.Call);
                return e => {
                    var target = e;

                    if (varInfo.EntityName != null)
                        target = e.Entities.GetEntityById(varInfo.EntityName);
                    
                    var val = getVal(target).ToString();
                    e.GetComponent<VarsComponent>().Set(varInfo.Name, val);
                };
            }

            return e => {
                e.GetComponent<VarsComponent>().Set(varInfo.Name, varInfo.Value);
            };
        }
    }
}
