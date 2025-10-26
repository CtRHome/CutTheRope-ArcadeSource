using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class TestButtonSprite : ButtonSprite
    {
        public TestButtonSprite(ContentManager content)
            :base(content.Load<Texture2D>("testButton"),
            new Rectangle[] { new Rectangle(0, 0, 132, 47), new Rectangle(132, 0, 132, 47) },
            new Point[] {Point.Zero, Point.Zero, Point.Zero, Point.Zero}, new Point(132, 47), 0,1)
        {
        }
    }
}
