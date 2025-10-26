using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class MagicHat : GameObject
    {
        public int Group
        {
            get;
            protected set;
        }
        protected Vector2 t1, t2, b1, b2;
        SoundFX snd;
        public float CoolDown
        {
            get;
            protected set;
        }

        public MagicHat(ContentManager content, Vector2 position, float angle, Mover m, int group)
        {
            this.Group = group;
            this.sprite = new HatSprite(content, group % 2);
            this.position = m == null ? position : m.Position;
            this.rotation = angle + (float)Math.PI / 2f;
            this.mover = m;
            if (mover != null)
            {
                mover.ModifyHatRotation();
            }
            UpdateBounds();
            snd = new SoundFX("teleport");
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);
            if (mover != null)
            {
                UpdateBounds();
            }
            if (CoolDown > 0)
            {
                CoolDown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public virtual bool IntersectsCandy(Candy c)
        {
            Vector2 candyPos = c.Position;
            float candyRadius = 14 * SingleLevel.SCALE;

            Vector2 rs = Util.RotateVector(c.Velocity, -rotation);

            return rs.Y >= 0 && (Util.LineInCircle(t1, t2, candyPos, candyRadius) || Util.LineInCircle(b1, b2, candyPos, candyRadius));
        }

        public override void UpdateBounds()
        {
            float pWidth = 15f * SingleLevel.SCALE;

            t1.X = position.X - pWidth;
            t2.X = position.X + pWidth;
            t1.Y = t2.Y = position.Y;

            b1.X = t1.X;
            b2.X = t2.X;
            b1.Y = b2.Y = position.Y + 1;


            t1 = Util.RotateVector(t1, rotation, position);
            t2 = Util.RotateVector(t2, rotation, position);
            b1 = Util.RotateVector(b1, rotation, position);
            b2 = Util.RotateVector(b2, rotation, position);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            base.Draw(sb, cameraPosition);
            if (Util.DebugDraw)
            {
                sb.End();
            GLDrawer.DrawAntialiasedLine(b1, b2, 2, Color.Yellow);
            GLDrawer.DrawAntialiasedLine(t1, t2, 2, Color.Yellow);
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            }
        }

        internal void Teleport(bool p)
        {
            if(p)
            {
                snd.Play();
            }
            (sprite as HatSprite).Teleport();
            CoolDown = 0.8f;
        }
    }
}
