using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class PollenSprite : Sprite
    {
        public float Scale;
        public float Alpha;

        public PollenSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_pollen_hd"), new Rectangle(1, 1, 20, 20), new Point(0, 0), new Point(20, 20))
        {
            Scale = 1;
            Alpha = 1;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            sb.Draw(image, position, frame, new Color(Color.White, Alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offset), Scale, SpriteEffects.None, 1);
        }
    }
}
