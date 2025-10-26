using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class PopupFrameSprite : Sprite
    {
        public PopupFrameSprite(ContentManager content)
            : base(content.Load<Texture2D>("menu_popup_hd"), new Rectangle(1, 1, 771, 811), new Point(13, 230), new Point(802, 1336))
        {}

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            sb.Draw(image, position, frame, new Color(Color.White, 0.4f), rotation, PtoV(fixedSize) / 2 - PtoV(offset), 0.6f, SpriteEffects.None, 1);
        }

    }
}
