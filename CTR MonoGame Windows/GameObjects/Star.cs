using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class Star : GameObject
    {
        Vector2 basePosition;
        double timeOffset;
        public bool Got;
        StarDisappearSprite sds;
        float lifeTime, lifeLeft;

        public bool HasTimer
        {
            get { return lifeTime > 0; }
        }

        public bool Gone
        {
            get { return lifeTime > 0 && lifeLeft <= 0; }
        }

        public Star(ContentManager content, Mover m, Vector2 position, float time)
        {
            mover = m;
            this.position = basePosition = m == null ? position : m.Position;
            timeOffset = Util.R.NextDouble() * 2;
            sprite = new StarSprite(content);
            sds = new StarDisappearSprite(content);
            lifeTime = lifeLeft = time;
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);
            if (lifeTime > 0 && !state.Camera.IgnoreTouches)
            {
                lifeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                (sprite as StarSprite).SetLife(lifeLeft / lifeTime);
            }
            if (Got)
            {
                sds.Update(gameTime);
            }
            else
            {
                if (mover == null)
                {
                    position.Y = basePosition.Y + (float)(3 * SingleLevel.SCALE * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 2 + timeOffset));
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            if (Got)
            {
                sds.Draw(sb, position - cameraPosition, rotation);
            }
            else
            {
                if (lifeTime <= 0 || lifeLeft > 0)
                {
                    base.Draw(sb, cameraPosition);
                }
            }
        }

        public override void DrawMiniMap(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, int levelY)
        {
            if (!Got && !Gone)
            {
                base.DrawMiniMap(sb, levelY);
            }
        }
    }
}
