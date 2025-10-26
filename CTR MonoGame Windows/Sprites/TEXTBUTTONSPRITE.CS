using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class TextButtonSprite : ButtonSprite
    {
        TextSprite font;
        string text;

        public TextButtonSprite(ContentManager content, string text)
            :base(content.Load<Texture2D>("menu_button_short_hd"), "1,1,337,139,1,142,337,139", "211,93,211,93", new Point(752,336), 0, 1)
        {
            font = new TextSprite(content, true);
            this.text = text;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            currentFrame = (pressed ? pressedFrame : releasedFrame);
            sb.Draw(image, position, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), 0.33f, SpriteEffects.None, 1);
            font.Draw(sb, text, position - Vector2.UnitY * 40, TextSprite.Alignment.Center);
        }
    }
}
