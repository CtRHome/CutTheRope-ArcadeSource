using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class RotoSpike : Spike
    {
        SpikeRotateButton button;

        public bool ButtonPressed
        {
            get { return button.Pressed; }
        }

        public bool Rotating
        {
            get;
            protected set;
        }

        float baseRotation;

        public int Group
        {
            get;
            protected set;
        }

        const float ROTATE_SPEED = 300f / 180f * (float)Math.PI;

        SoundFX rotIn, rotOut;

        public RotoSpike(ContentManager content, Mover m, Vector2 position, float rotation, int size, int toggleGroup)
            : this(content, m, position, rotation, size, toggleGroup, false)
        {}

        public RotoSpike(ContentManager content, Mover m, Vector2 position, float rotation, int size, int toggleGroup, bool bonus)
            :base(content, m, position, rotation)
        {
            sprite = new SpikeSprite(content, size, true, bonus);
            button = new SpikeRotateButton(content, position, toggleGroup - 1, bonus);
            UpdateBounds();
            baseRotation = rotation;
            Group = toggleGroup - 1;
            button.SetRotation(rotation);
            rotIn = new SoundFX("spike_rotate_in");
            rotOut = new SoundFX("spike_rotate_out");
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);
            button.Update(gameTime, state);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Rotating)
            {
                if (button.Flipped) // Rotating away from base
                {
                    rotation += ROTATE_SPEED * elapsed;
                    float targetRot = baseRotation + (float)Math.PI / 2f;
                    if (rotation >= targetRot)
                    {
                        rotation = targetRot;
                        Rotating = false;
                    }
                }
                else
                {
                    rotation -= ROTATE_SPEED * elapsed;
                    if (rotation <= baseRotation)
                    {
                        rotation = baseRotation;
                        Rotating = false;
                    }
                }
                button.SetRotation(rotation);
                UpdateBounds();
            }
        }

        public void Rotate()
        {
            Rotating = true;
            button.Flipped = !button.Flipped;
            if (button.Flipped)
            {
                rotOut.Play();
            }
            else
            {
                rotIn.Play();
            }
        }

        public override void Draw(SpriteBatch sb, Vector2 cameraPosition)
        {
            base.Draw(sb, cameraPosition);
            button.Draw(sb, cameraPosition);
        }

        public override void DrawMiniMap(SpriteBatch sb, int levelY)
        {
            base.DrawMiniMap(sb, levelY);
            button.DrawMiniMap(sb, levelY);
        }
    }
}
