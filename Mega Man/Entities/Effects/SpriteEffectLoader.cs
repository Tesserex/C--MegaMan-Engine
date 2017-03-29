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

            if (sprite.Facing != null)
            {
                FacingValues facing = sprite.Facing.Value;
                action += entity => {
                    GameEntity player = entity.Entities.GetEntityById("Player");
                    PositionComponent playerPos = player.GetComponent<PositionComponent>();

                    SpriteComponent spritecomp = entity.GetComponent<SpriteComponent>();
                    PositionComponent positioncomp = entity.GetComponent<PositionComponent>();

                    spritecomp.HorizontalFlip = false;  // Skip cases to set it to false

                    if (facing == FacingValues.Left) spritecomp.HorizontalFlip = true;
                    else
                    {
                        bool leftFromPlayer = (positioncomp.Position.X <= playerPos.Position.X);
                        if (facing == FacingValues.Player) spritecomp.HorizontalFlip = !leftFromPlayer;
                        else if (facing == FacingValues.PlayerOpposite) spritecomp.HorizontalFlip = leftFromPlayer;
                    }
                };
            }

            return action;
        }
    }
}
