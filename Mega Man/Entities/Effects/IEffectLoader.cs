using System;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public interface IEffectLoader
    {
        Type PartInfoType { get; }
        Effect Load(IEffectPartInfo info);
    }
}
