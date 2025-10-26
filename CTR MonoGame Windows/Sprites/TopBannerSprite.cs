using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class TopBannerSprite : Sprite
    {
        public TopBannerSprite(ContentManager content)
            : base(content.Load<Texture2D>("menu_pause_hd"), "1,1,800,219", "176,189", new Point(1141,524))
        {}

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            //sb.Draw(image, new Rectangle(0,1625,1080, 295), frame, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            sb.Draw(image, new Rectangle(0, -30, 1080, 295), frame, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
