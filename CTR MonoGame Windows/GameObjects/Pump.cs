using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class Pump : OnRecord
    {
        Vector2 t1, t2;
        List<PumpParticle> particles;
        SoundFX[] pumpSounds;

        public Pump(ContentManager content, Vector2 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
            sprite = new PumpSprite(content);
            particles = new List<PumpParticle>();
            pumpSounds = new SoundFX[] {
                new SoundFX("pump_1"),
                new SoundFX("pump_2"),
                new SoundFX("pump_3"),
                new SoundFX("pump_4"),
            };
            UpdateBounds();
        }

        public void OperatePump(GlobalState state, int touch)
        {
            state.Input.ConsumeClick(touch);
            pumpSounds[Util.R.Next(4)].Play();
            (sprite as PumpSprite).PlayAnimation();
            Vector2 v = position + 25 * SingleLevel.SCALE * Vector2.UnitX;
            v = Util.RotateVector(v, rotation - (float)Math.PI / 2f, position);
            for (int i = 0; i < 5; i++)
            {
                particles.Add(new PumpParticle(v, Vector2.Normalize(v - position) * 500 * SingleLevel.SCALE + 100 * SingleLevel.SCALE * Util.RandomInsideUnitCircle()));
            }

            BlowCandy(state.Candy);
            if (state.Candy2 != null)
            {
                BlowCandy(state.Candy2);
            }
        }

        private void BlowCandy(Candy c)
        {
            Vector2 toCandy = c.Position - position;

            float lateralDistance = Vector2.Dot(toCandy, Vector2.Normalize(t2 - t1));
            if (Math.Abs(lateralDistance) > (57 + 14) * SingleLevel.SCALE)
            {
                return;
            }

            toCandy = Util.RotateVector(toCandy, -rotation);
            float range = -toCandy.Y;

            float maxRange = 750;

            if (range > 0 && range < maxRange)
            {
                float power = 1.2f * (maxRange - range);
                Vector2 pumpForce = Vector2.UnitY * -power;
                pumpForce = Util.RotateVector(pumpForce, rotation);
                c.Physics.position += pumpForce * 0.016f;
            }
        }

        public override void UpdateBounds()
        {
            t1.X = position.X - 57 * SingleLevel.SCALE / 2.0f;
            t2.X = position.X + 57 * SingleLevel.SCALE / 2.0f;
            t1.Y = t2.Y = position.Y;

            t1 = Util.RotateVector(t1, rotation, position);
            t2 = Util.RotateVector(t2, rotation, position);
        }

        public override void RotatePosition(float angle, Vector2 center)
        {
            rotation += angle;
            base.RotatePosition(angle, center);
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = particles.Count() - 1; i >= 0; i--)
            {
                particles[i].life -= elapsed;
                particles[i].position += particles[i].velocity * elapsed;
                if (particles[i].life <= 0)
                {
                    particles.RemoveAt(i);
                }
            }
            for (int i = Util.OnDevice ? 0 : -1; i < Input.TOUCH_COUNT; i++)
            {
                if (state.Input.MouseJustClicked(i) && (state.Input.TouchPos(i) + state.Camera.Position - position).LengthSquared() < 30 * 30 * SingleLevel.SCALE * SingleLevel.SCALE)
                {
                    OperatePump(state, i);
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            base.Draw(sb, cameraPosition);
            foreach (PumpParticle particle in particles)
            {
                (sprite as PumpSprite).DrawParticle(sb, particle.position - cameraPosition, particle.type);
            }
            if (Util.DebugDraw)
            {
                sb.End();
                GLDrawer.DrawAntialiasedLine(t1, t2, 2, Color.Red);
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            }
        }

        class PumpParticle
        {
            public Vector2 position;
            public Vector2 velocity;
            public float life;
            public int type;

            public PumpParticle(Vector2 position, Vector2 velocity)
            {
                this.position = position;
                this.velocity = velocity;
                life = 0.3f;
                type = Util.R.Next(3);
            }

        }

    }
}
