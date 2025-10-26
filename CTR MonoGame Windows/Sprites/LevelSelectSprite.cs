using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class LevelSelectSprite : AnimatedSprite
    {
        public enum Sign { Locked, LockedKey, Playable, Stars0, Stars1, Stars2, Stars3 };

        Sign sign;
        float alpha;

        public LevelSelectSprite(ContentManager content, Sign sign)
            : base(content.Load<Texture2D>("menu_level_selection_hd"), "1,1,162,167,1,170,161,166,1,338,153,157,1,497,114,61,117,497,114,61,1,560,114,61,117,560,114,61",
            "4,6,4,6,8,8,47,112,47,112,47,112,47,112", new Point(175, 175))
        {
            this.sign = sign;
            this.alpha = 1;
        }

        public void SetAlpha(float alpha)
        {
            this.alpha = alpha;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            currentFrame = (int)sign;
            sb.Draw(image, position, frames[currentFrame], new Color(Color.White, alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), 1, SpriteEffects.None, 1);
        }
    }
}
