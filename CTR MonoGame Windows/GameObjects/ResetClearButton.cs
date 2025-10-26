using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class ResetClearButton : Button
    {
        public ResetClearButton(ContentManager content, Vector2 position, bool reset, bool test)
            :base(position, 132, 47)
        {
            if (test)
            {
                sprite = new TestButtonSprite(content);
            }
            else
            {
                sprite = new ResetClearButtonSprite(content, reset);
            }
        }
    }
}
