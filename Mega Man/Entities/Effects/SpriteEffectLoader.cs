using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.Engine.Entities.Effects
{
    public class SpriteEffectLoader : IEffectLoader
    {
        public Type PartInfoType
        {
            get
            {
                return typeof(SpriteEffectPartInfo);
            }
        }

        public Effect Load(IEffectPartInfo info)
        {
            var sprite = (SpriteEffectPartInfo)info;
            
            Effect action = entity => { };
            if (sprite.Name != null)
            {
                action += entity => {
                    SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                    spritecomp.ChangeSprite(sprite.Name);
                };
            }

            if (sprite.Playing != null)
            {
                bool play = sprite.Playing.Value;
                action += entity => {
                    SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                    spritecomp.Playing = play;
                };
            }

            if (sprite.Visible != null)
            {
                bool vis = sprite.Visible.Value;
                action += entity => {
                    SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                    spritecomp.Visible = vis;
                };
            }

            return action;
        }
    }
}
