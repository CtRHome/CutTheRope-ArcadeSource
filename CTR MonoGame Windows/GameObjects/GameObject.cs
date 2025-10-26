using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    abstract class GameObject
    {
        protected Sprite sprite;
        protected Vector2 position;
        protected float rotation;
        protected Mover mover;

        public Vector2 Position
        {
            get { return position; }
        }

        public float Rotation
        {
            get { return rotation; }
        }

        public Mover Mover
        {
            get { return mover; }
        }

        public virtual void Update(GameTime gameTime, GlobalState state)
        {
            sprite.Update(gameTime);
            if (mover != null)
            {
                mover.Update(gameTime);
                position = mover.Position;
                rotation = mover.Rotation;
            }
        }

        public virtual void UpdateBounds()
        {}

        public virtual void Draw(SpriteBatch sb, Vector2 cameraPosition)
        {
            sprite.Draw(sb, position - cameraPosition, rotation);
        }

        public virtual void DrawMiniMap(SpriteBatch sb, int levelY)
        {
            sprite.DrawMiniMap(sb, GetMiniPos(position, levelY), rotation);
        }

        protected Vector2 GetMiniPos(Vector2 pos, int levelY)
        {
            Vector2 drawPos = pos;
            if (levelY > 10)
            {
                drawPos.Y /= levelY;
                drawPos.Y *= 4f / 3f;
            }
            else
            {
                drawPos.Y *= 0.125f;
                drawPos.Y += 100;
            }
            drawPos.X -= 120f;
            drawPos.X *= 0.125f;
            return drawPos;
        }
    }
}
