using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class ToggleButton : Button
    {
        public bool Toggled
        {
            get;
            protected set;
        }

        public ToggleButton(Vector2 position, float radius)
            :base(position, radius)
        {
        }

        public ToggleButton(Vector2 position, float width, float height)
            : base(position, width, height)
        {
        }


        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);

            if (Pressed)
            {
                Toggled = !Toggled;
            }

            (sprite as ButtonSprite).SetPressed(Toggled);
        }
    }
}
