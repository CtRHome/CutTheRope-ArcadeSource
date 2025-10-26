using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class ResetClearButtonSprite : ButtonSprite
    {
        public ResetClearButtonSprite(ContentManager content, bool reset)
            :base(content.Load<Texture2D>("resetButton"),
            new Rectangle[] { new Rectangle(0, 0, 132, 47), new Rectangle(0, 47, 132, 47), new Rectangle(132, 0, 132, 47), new Rectangle(132, 47, 132, 47) },
            new Point[] {Point.Zero, Point.Zero, Point.Zero, Point.Zero}, new Point(132, 47), reset ? 0 : 1, reset ? 2 : 3)
        {
        }
    }
}
