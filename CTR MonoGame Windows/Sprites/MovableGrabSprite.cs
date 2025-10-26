using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class MovableGrabSprite : GrabSprite
    {
        bool highlight, vertical;
        float length, grabPosition;

        public float Alpha;

        public MovableGrabSprite(ContentManager content, bool vertical, float length)
            : base(content.Load<Texture2D>("obj_hook_movable_hd"), "2,2,56,89,62,2,42,89,108,2,40,89,1,94,127,128,1,224,148,147",
            "54,66,110,66,90,66,45,46,35,36", new Point(220, 220))
        {
            this.vertical = vertical;
            this.length = length;
            if (vertical)
            {
                for (int i = 0; i < offsets.Count; i++)
                {
                    offsets[i] = new Point(offsets[i].Y, offsets[i].X);
                }
            }
            Alpha = 1;
        }

        public void SetHighlight(bool highlight)
        {
            this.highlight = highlight;
        }

        public void SetGrabPosition(float position)
        {
            this.grabPosition = position;
        }


        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            float angle = vertical ? (float)Math.PI / 2 : 0;

            Vector2 curPos = position;
            if (vertical)
            {
                curPos.X -= 130;
            }
            currentFrame = 0;
            DrawF(sb, curPos, angle);
            currentFrame = 2;
            float lengthToDraw = length -frames[0].Width / 2;
            curPos += (vertical ? Vector2.UnitY : Vector2.UnitX) * (/*frames[1].Width +*/ frames[2].Width) / 2;
            while (lengthToDraw > 0)
            {
                DrawF(sb, curPos, angle);
                curPos += (vertical ? Vector2.UnitY : Vector2.UnitX) * frames[2].Width;
                lengthToDraw -= frames[2].Width;
            }
            DrawF(sb, curPos, angle, -lengthToDraw);
            curPos -= (vertical ? Vector2.UnitY : Vector2.UnitX) * lengthToDraw;
            curPos -= (vertical ? Vector2.UnitY : Vector2.UnitX) * frames[2].Width / 2;
            currentFrame = 1;
            DrawF(sb, curPos, angle);
        }

        public override void DrawTop(SpriteBatch sb, Vector2 position, float rotation)
        {
            float angle = vertical ? (float)Math.PI / 2 : 0;
            currentFrame = highlight ? 3 : 4;
            DrawF(sb, position + (vertical ? Vector2.UnitY : Vector2.UnitX) * grabPosition - (vertical ? Vector2.UnitX * (highlight ? 90 : 70) : Vector2.Zero), angle);
        }

        void DrawF(SpriteBatch sb, Vector2 position, float rotation, float extraLength)
        {
            Rectangle rect = frames[currentFrame];
            rect.Width = (int)extraLength + 1;
            sb.Draw(image, GetDrawPos(position), rect, new Color(Color.White, Alpha), rotation, PtoV(fixedSize) / 2, 1, SpriteEffects.None, 1);
        }

        void DrawF(SpriteBatch sb, Vector2 position, float rotation)
        {
            sb.Draw(image, GetDrawPos(position), frames[currentFrame], new Color(Color.White, Alpha), rotation, PtoV(fixedSize) / 2, 1, SpriteEffects.None, 1);
        }

        private Vector2 GetDrawPos(Vector2 position)
        {
            return new Vector2(position.X + offsets[currentFrame].X, position.Y + offsets[currentFrame].Y);
        }
    }
}
