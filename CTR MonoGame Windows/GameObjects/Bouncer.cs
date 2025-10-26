using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class Bouncer : GameObject
    {
        protected Vector2 t1, t2, b1, b2;

        SoundFX bounce;

        public Bouncer(ContentManager content, Mover m, Vector2 position, float rotation, int size)
        {
            mover = m;
            this.position = m == null ? position : m.Position;
            this.rotation = m == null ? rotation : m.Rotation;
            bounce = new SoundFX("bouncer");
            sprite = new BouncerSprite(content, size);
            UpdateBounds();
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);
            if (mover != null)
            {
                UpdateBounds();
            }
        }

        public override void UpdateBounds()
        {
            float pWidth = (sprite as BouncerSprite).Width / 2f;

            t1.X = position.X - pWidth;
            t2.X = position.X + pWidth;
            t1.Y = t2.Y = position.Y - 10.0f * SingleLevel.SCALE / 2.0f;

            b1.X = t1.X;
            b2.X = t2.X;
            b1.Y = b2.Y = position.Y + 10.0f * SingleLevel.SCALE / 2.0f;


            t1 = Util.RotateVector(t1, rotation, position);
            t2 = Util.RotateVector(t2, rotation, position);
            b1 = Util.RotateVector(b1, rotation, position);
            b2 = Util.RotateVector(b2, rotation, position);
        }

        public virtual bool IntersectsCandy(Vector2 candyPos)
        {
            float candyRadius = 20 * SingleLevel.SCALE;
            return Util.LineInCircle(t1, t2, candyPos, candyRadius) || Util.LineInCircle(b1, b2, candyPos, candyRadius);
        }

        public void Bounce()
        {
            bounce.Play();
            (sprite as BouncerSprite).Bounce();
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            base.Draw(sb, cameraPosition);
            if (Util.DebugDraw)
            {
                sb.End();
            GLDrawer.DrawAntialiasedLine(b1, b2, 2, Color.Red);
            GLDrawer.DrawAntialiasedLine(t1, t2, 2, Color.Red);
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            }
        }
    }
}
