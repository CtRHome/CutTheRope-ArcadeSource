using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class HorizLogoSprite : Sprite
    {

        public HorizLogoSprite(ContentManager content, bool wide)
            : base(content.Load<Texture2D>(wide ? "ctrHorizontal968" : "ctrHorizontal"),
			      wide ? new Rectangle(0, 0, 968, 377) : new Rectangle(0, 0, 683, 267),
			       Point.Zero, wide ? new Point(968, 377) : new Point(683, 267))
        { }
    }
}
