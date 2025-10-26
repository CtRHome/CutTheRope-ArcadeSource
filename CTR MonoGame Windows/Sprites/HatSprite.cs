using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class HatSprite : AnimatedSprite
    {
        int group;

        public HatSprite(ContentManager content, int group)
            : base(content.Load<Texture2D>("obj_hat_hd"), "1,1,236,269,1,272,238,270,239,1,234,206,241,272,246,235,1,544,222,180",
            "42,5,41,2,44,63,38,46,50,79", new Point(346, 346))
        {
            this.group = group;

            AddAnimation(0, new Animation(0.05, 2, 3, Animation.LoopType.Stop));
            AddAnimation(1, new Animation(0.05, 3, 3, Animation.LoopType.Vanish));
            SetNextAnimation(1, 0, 0.05);
        }

        public void Teleport()
        {
            SetAnimation(0);
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            float scale = 0.7f;
            sb.Draw(image, position, frames[group], Color.White, rotation, Vector2.UnitX * (PtoV(fixedSize) / 2 - PtoV(offsets[group])).X + 25 * SingleLevel.SCALE * Vector2.UnitY, scale, SpriteEffects.None, 1);

            if (currentFrame < 0 || currentFrame > frames.Count)
            {
                return;
            }
            Vector2 glowFrameSize = PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]);
            glowFrameSize.Y *= 2;
            glowFrameSize.Y -= 25 * SingleLevel.SCALE;
            sb.Draw(image, position, frames[currentFrame], Color.White, rotation, glowFrameSize, scale, SpriteEffects.None, 1);
        }

    }
}
