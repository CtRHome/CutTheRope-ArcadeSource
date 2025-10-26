using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class PlusMinusButton : Button
    {
        public PlusMinusButton(ContentManager content, Vector2 position, bool increment)
            :base(position, 10 * SingleLevel.SCALE)
        {
            sprite = new PlusMinusButtonSprite(content, increment);
        }
    }
}
