using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class BoxCoverSprite : AnimatedSprite
    {
        public BoxCoverSprite(ContentManager content)
            : base(content.Load<Texture2D>("bgr_01_cover_hd"), "0,0,409,1280,409,0,48,1280",
            "0,0,402,0", new Point(1600, 2560))
        {
        }

        public void DrawBox(SpriteBatch sb, Rectangle dest, bool reflect)
        {
            sb.Draw(image, dest, frames[0], Color.White, 0, Vector2.Zero, reflect ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
        }

        public void DrawEdge(SpriteBatch sb, Rectangle dest, bool reflect)
        {
            sb.Draw(image, dest, frames[1], Color.White, 0, Vector2.Zero, reflect ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
        }
    }
}
