using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class YesNoButtonSprite : ButtonSprite
    {
        public YesNoButtonSprite(ContentManager content, bool yes)
            :base(content.Load<Texture2D>("yesNo"),
            new Rectangle[] { new Rectangle(0, 0, 254, 93), new Rectangle(0, 93, 254, 93), new Rectangle(254, 0, 254, 93), new Rectangle(254, 93, 254, 93) },
            new Point[] { Point.Zero, Point.Zero, Point.Zero, Point.Zero }, new Point(254, 93), yes ? 0 : 1, yes ? 2 : 3)
        {
        }
    }
}
