using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class TextButton : Button
    {
        public TextButton(ContentManager content, Vector2 position, string text)
            :base(position, 337/3, 139/3)
        {
            sprite = new TextButtonSprite(content, text);
        }
    }
}
