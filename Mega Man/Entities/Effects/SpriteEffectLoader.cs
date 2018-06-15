using System;
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
                    var spritecomp = entity.GetComponent<SpriteComponent>();
                    spritecomp.ChangeSprite(sprite.Name);
                };
            }

            if (sprite.Playing != null)
            {
                var play = sprite.Playing.Value;
                action += entity => {
                    var spritecomp = entity.GetComponent<SpriteComponent>();
                    spritecomp.Playing = play;
                };
            }

            if (sprite.Visible != null)
            {
                var vis = sprite.Visible.Value;
                action += entity => {
                    var spritecomp = entity.GetComponent<SpriteComponent>();
                    spritecomp.Visible = vis;
                };
            }

            if (sprite.Facing != null)
            {
                var facing = sprite.Facing.Value;
                action += entity => {
                    var player = entity.Entities.GetEntityById("Player");
                    var playerPos = player.GetComponent<PositionComponent>();

                    var spritecomp = entity.GetComponent<SpriteComponent>();
                    var positioncomp = entity.GetComponent<PositionComponent>();

                    spritecomp.HorizontalFlip = false;  // Skip cases to set it to false

                    if (facing == FacingValues.Left) spritecomp.HorizontalFlip = true;
                    else
                    {
                        var leftFromPlayer = (positioncomp.Position.X <= playerPos.Position.X);
                        if (facing == FacingValues.Player) spritecomp.HorizontalFlip = !leftFromPlayer;
                        else if (facing == FacingValues.PlayerOpposite) spritecomp.HorizontalFlip = leftFromPlayer;
                    }
                };
            }

            return action;
        }
    }
}
