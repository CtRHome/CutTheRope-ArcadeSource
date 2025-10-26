using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class PlusMinusButtonSprite : ButtonSprite
    {
        public PlusMinusButtonSprite(ContentManager content, bool plus)
            :base(content.Load<Texture2D>("PlusMinusButtonStates"),
            new Rectangle[] { new Rectangle(0, 0, 48, 48), new Rectangle(0, 48, 48, 48), new Rectangle(48, 0, 48, 48), new Rectangle(48, 48, 48, 48)},
            new Point[] {Point.Zero, Point.Zero, Point.Zero, Point.Zero}, new Point(48, 48), plus ? 0 : 1, plus ? 2 : 3)
        {
        }
    }
}
