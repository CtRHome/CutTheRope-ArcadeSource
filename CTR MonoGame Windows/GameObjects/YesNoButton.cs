using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class YesNoButton : Button
    {
        public YesNoButton(ContentManager content, Vector2 position, bool yes)
            :base(position, 254, 93)
        {
            sprite = new YesNoButtonSprite(content, yes);
        }
    }
}
