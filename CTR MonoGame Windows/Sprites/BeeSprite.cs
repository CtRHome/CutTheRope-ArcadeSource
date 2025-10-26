using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class BeeSprite : AnimatedSprite
    {
        public BeeSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_bee_hd"), "0,0,2,3,1,1,71,93,74,1,142,29,74,32,150,61,1,96,69,48", "116,158,75,60,40,68,36,31,77,29", new Point(221, 221))
        {
            AddAnimation(0, new Animation(0.03, 2, 4, Animation.LoopType.PingPong));
            SetAnimation(0);
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            Vector2 bodyCenter = PtoV(fixedSize) / 2 - PtoV(offsets[1]);
            bodyCenter.Y *= 2;
            bodyCenter.X += 5;
            sb.Draw(image, position, frames[1], Color.White, rotation, bodyCenter, 1f / 1.3f, SpriteEffects.None, 1f);
            if (currentFrame < 0 || currentFrame > frames.Count)
            {
                return;
            }
            Vector2 wingCenter = PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]);
            wingCenter.Y += bodyCenter.Y / 2;
            wingCenter.X += 5;
            sb.Draw(image, position, frames[currentFrame], Color.White, rotation, wingCenter, 1f / 1.3f, SpriteEffects.None, 1);
        }
    }
}
