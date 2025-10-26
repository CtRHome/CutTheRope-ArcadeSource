using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class PlatformSprite : Sprite
    {
        public float Scale = 1;

        public PlatformSprite(Texture2D image, Rectangle frame, Point offset, Point fixedSize)
            : base(image, frame, offset, fixedSize)
        { }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            sb.Draw(image, position, frame, Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offset), Scale, SpriteEffects.None, 1);
        }

    }
}
