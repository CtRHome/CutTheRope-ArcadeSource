using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class BluePlusMinusButton : ToggleButton
    {
        public BluePlusMinusButton(ContentManager content, Vector2 position)
            :base(position, 10 * SingleLevel.SCALE)
        {
            sprite = new BluePlusMinusButtonSprite(content);
        }

        public void Toggle(bool toggled)
        {
            Toggled = toggled;
        }
    }
}
