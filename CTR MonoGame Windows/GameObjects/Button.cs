using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class Button : GameObject
    {
        public bool Pressed
        {
            get;
            protected set;
        }

        public bool Touched
        {
            get;
            protected set;
        }

        public bool Held
        {
            get;
            protected set;
        }

        bool useRectangle;
        Rectangle bounds;
        float radius;

        private Button(Vector2 position)
        {
            this.position = position;
        }

        public Button(Vector2 position, float radius)
            :this(position)
        {
            useRectangle = false;
            this.radius = radius;
        }

        public Button(Vector2 position, float width, float height)
            : this(position)
        {
            useRectangle = true;
            this.bounds = new Rectangle((int)(position.X - width / 2), (int)(position.Y - height / 2), (int)width, (int)height);
        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
            UpdateBounds();            
        }

        public override void UpdateBounds()
        {
            if (useRectangle)
            {
                this.bounds = new Rectangle((int)(position.X - bounds.Width / 2), (int)(position.Y - bounds.Height / 2), (int)bounds.Width, (int)bounds.Height);
            }
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);

            Pressed = false;
            Touched = false;

            bool wasHeld = Held;
            Held = false;

            for (int i = Util.OnDevice ? 0 : -1; i < Input.TOUCH_COUNT; i++)
            {
                if ((useRectangle && bounds.Contains(state.Input.TouchPos(i) + state.Camera.Position)) ||
                    (!useRectangle && (state.Input.TouchPos(i) + state.Camera.Position - Position).LengthSquared() < radius * radius))
                {
                    if (state.Input.MouseJustClicked(i))
                    {
                        state.Input.ConsumeClick(i);
                        Touched = true;
                        Held = true;
                    }
                    if (state.Input.TouchDown(i) && wasHeld)
                    {
                        Held = true;
                    }
                }
            }

            Pressed = wasHeld && !Held;


            (sprite as ButtonSprite).SetPressed(Held);
        }
    }
}
