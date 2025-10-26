using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class WheelGrabSprite : GrabSprite
    {
        bool highlight;
        float midScale;

        public WheelGrabSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_hook_regulated_hd"), "1,1,110,111,113,1,83,80,1,114,189,188,1,304,193,197",
            "52,56,68,69,14,15,10,11", new Point(220, 220))
        {}

        public void SetHighlight(bool highlight)
        {
            this.highlight = highlight;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            DrawFrame(sb, position, 0, 0);
            if (midScale > 0)
            {
                DrawF(sb, position, rotation, 1, midScale);
            }
        }

        public override void DrawTop(SpriteBatch sb, Vector2 position, float rotation)
        {
            DrawF(sb, position, rotation, highlight ? 2 : 3, 1);
        }

        void DrawF(SpriteBatch sb, Vector2 position, float rotation, int frame, float scale)
        {
            currentFrame = frame;
            if (currentFrame < 0 || currentFrame > frames.Count)
            {
                return;
            }
            sb.Draw(image, position, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), scale, SpriteEffects.None, 1);
        }


        internal void SetMiddleScale(float p)
        {
            midScale = p;
        }
    }
}
