using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class PumpSprite : AnimatedSprite
    {
        public PumpSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_pump_hd"), "1,1,176,191,1,194,165,232,168,194,165,232,335,194,176,197,179,1,191,176,1,428,180,186,179,179,14,13,195,179,9,9,372,1,31,32",
            "218,210,230,187,230,187,218,200,210,218,216,213,307,302,309,304,298,293", new Point(611,611))
        {
            AddAnimation(0, new Animation(0.05, 1, 3, Animation.LoopType.Stop));
            AddAnimation(1, new Animation(0.05, 0, 0, Animation.LoopType.Stop));
            SetNextAnimation(1, 0, 0.05);
            SetAnimation(1);
        }

        public void PlayAnimation()
        {
            SetAnimation(0);
        }

        public void DrawParticle(SpriteBatch sb, Vector2 position, int type)
        {
            sb.Draw(image, position, frames[type + 6], new Color(Color.White, 0.6f), 0, PtoV(fixedSize) / 2 - PtoV(offsets[type + 6]), 1, SpriteEffects.None, 1);
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            if (currentFrame < 0 || currentFrame > frames.Count)
            {
                return;
            }
            sb.Draw(image, position, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), 1, SpriteEffects.None, 1);
        }
    }
}
