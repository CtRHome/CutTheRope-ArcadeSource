using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class MenuGraphicSprite : AnimatedSprite
    {
        public enum Sign { Logo, Candy1, Candy2, Candy3, Marker, ZeptoLogo, SmLogo }

        Sign sign;
        float alpha;

        public MenuGraphicSprite(ContentManager content, Sign sign)
            : base(content.Load<Texture2D>("menu_logo_hd"), "1,1,697,452,700,1,206,215,700,218,201,198,1,455,192,218,0,0,4,5,195,455,215,242,0,0,245,108",
            "69,98,435,522,435,532,443,508,534,617,348,255,327,327", new Point(865, 790))
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
