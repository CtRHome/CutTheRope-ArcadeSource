using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class TutorialGraphicSprite : AnimatedSprite
    {

        public enum Sign { CutLine, Arrow1, Arrow2, Arrow3, Pop, Warning, Marks, Reset, Tip, Finger}

        Sign sign;
        float alpha;

        public TutorialGraphicSprite(ContentManager content, Sign sign)
            : base(content.Load<Texture2D>("tutorial_signs_hd"), "1,1,119,10,1,13,31,151,34,13,147,120,1,166,116,105,1,273,195,187,1,462,128,115,122,1,39,8,183,13,72,68,1,579,82,118,1,699,208,253",
            "471,616,130,193,331,440,247,319,149,533,426,180,544,433,434,341,283,170,554,159", new Point(802,850))
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
