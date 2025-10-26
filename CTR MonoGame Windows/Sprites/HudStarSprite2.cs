using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class HudStarSprite2 : AnimatedSprite
    {
        ResultGraphicSprite empty, full;
        bool got;

        public HudStarSprite2(ContentManager content)
            : base(content.Load<Texture2D>("obj_star_disappear_hd"), "1,1,198,216,201,1,270,298,1,219,165,187,473,1,248,285,201,301,311,371,201,674,261,321,723,1,270,265,514,301,239,213,1,408,194,186,1,596,167,175,1,773,165,166,755,301,161,157,514,516,158,107",
            "117,94,61,31,134,106,91,45,61,-1,91,28,70,66,82,92,103,100,119,105,120,108,122,111,123,114", new Point(444, 444))
        {
            AddAnimation(0, new Animation(0.05, 0, 12, Animation.LoopType.Vanish));
            currentFrame = -1;
            empty = new ResultGraphicSprite(content, ResultGraphicSprite.Image.StarEmpty);
            empty.SetScale(0.5f);
            full = new ResultGraphicSprite(content, ResultGraphicSprite.Image.Star);
            full.SetScale(0.5f);
        }

        public void Earn()
        {
            SetAnimation(0);
            got = true;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            if (got)
            {
                full.Draw(sb, position + Vector2.UnitY * 115, rotation);
            }
            else
            {
                empty.Draw(sb, position + Vector2.UnitY * 115, rotation);
            }
            if (currentFrame < 0 || currentFrame > frames.Count)
            {
                return;
            }
            sb.Draw(image, position, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), 1, SpriteEffects.None, 1);
        }
    }
}
