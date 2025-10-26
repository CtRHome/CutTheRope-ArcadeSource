using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class LoadScreenSprite : AnimatedSprite
    {

        public enum Sign { TapeLeft, TapeRight, Knife, TapeRoll }

        Sign sign;
        float alpha;

        public LoadScreenSprite(ContentManager content, Sign sign)
            : base(content.Load<Texture2D>("menu_loading_hd"), "1,1,49,1088,52,1,53,1088,1,1091,407,538,107,1,294,407",
            "346,95,395,95,0,650,234,34", new Point(800, 1280))
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
            sb.Draw(image, position, frames[currentFrame], new Color(Color.White, alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), 1.4f, SpriteEffects.None, 1);
        }
    }
}
