using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class GravityButton : ToggleButton
    {
        SoundFX up, down;

        public GravityButton(ContentManager content, Vector2 position)
            :base(position, 15 * SingleLevel.SCALE)
        {
            sprite = new GravityButtonSprite(content);
            up = new SoundFX("gravity_on");
            down = new SoundFX("gravity_off");
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            Toggled = state.Gravity.Y < 0;

            base.Update(gameTime, state);

            if (Pressed)
            {
                if (Toggled)
                {
                    up.Play();
                    state.Gravity = -Vector2.UnitY * 1568;
                }
                else
                {
                    down.Play();
                    state.Gravity = Vector2.UnitY * 1568;
                }
            }
        }

    }
}
