using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class ConfettiSprite : AnimatedSprite
    {
        static Random r = new Random();

        public float Alpha;

        public ConfettiSprite(ContentManager content)
            : base(content.Load<Texture2D>("confetti_particles_hd"), "1,1,34,30,1,33,35,29,1,64,37,26,1,92,37,26,1,120,36,29,1,151,34,30,1,183,33,33,1,218,35,34,1,254,33,33,1,289,37,27,1,318,38,25,1,345,39,30,1,377,41,36,1,415,41,42,1,459,41,31,1,492,39,22,1,516,37,25,1,543,37,27,1,572,43,43,1,617,35,43,1,662,26,44,38,617,18,43,46,572,9,43,29,662,9,43,40,662,18,44,1,708,26,44,1,754,35,43",
            "100,79,99,79,98,81,98,81,99,79,100,79,100,77,99,77,100,77,98,80,98,81,97,78,96,75,96,73,96,78,97,83,98,81,98,80,96,71,100,71,104,71,108,71,112,71,112,71,108,71,104,71,100,71", new Point(234, 184))
        {
            switch (r.Next(3))
            {
                default:
                case 0:
                    AddAnimation(0, new Animation(r.NextDouble() * 0.05 + 0.025, 0, 8, Animation.LoopType.Repeat));
                    break;
                case 1:
                    AddAnimation(0, new Animation(r.NextDouble() * 0.05 + 0.025, 9, 17, Animation.LoopType.Repeat));
                    break;
                case 2:
                    AddAnimation(0, new Animation(r.NextDouble() * 0.05 + 0.025, 18, 26, Animation.LoopType.Repeat));
                    break;
            }
            currentAnimation = 0;
            Alpha = 1;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            if (currentFrame < 0 || currentFrame > frames.Count || Alpha <= 0)
            {
                return;
            }
            sb.Draw(image, position, frames[currentFrame], new Color(Color.White, Alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), 1, SpriteEffects.None, 1);
        }
    }
}
