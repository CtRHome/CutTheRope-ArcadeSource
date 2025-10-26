using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class BubbleSprite : AnimatedSprite
    {
        public bool Released;
        int stainFrame;

        public BubbleSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_bubble_attached_hd"), "1,1,124,124,1,127,146,169,1,298,149,144,1,444,147,143", "38,39,26,28,28,31,28,31", new Point(200, 200))
        {
            stainFrame = Util.R.Next(2) + 1;
        }


        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            sb.Draw(image, GetDrawPos(position, stainFrame), frames[stainFrame], Color.White);

            if (!Released)
            {
                sb.Draw(image, GetDrawPos(position, 0), frames[0], Color.White);
            }
        }

        public override void DrawMiniMap(SpriteBatch sb, Vector2 miniPos, float rotation)
        {
            currentFrame = stainFrame;
            base.DrawMiniMap(sb, miniPos, rotation);
            if (!Released)
            {
                currentFrame = 0;
                base.DrawMiniMap(sb, miniPos, rotation);
            }
        }

        private Vector2 GetDrawPos(Vector2 position, int frame)
        {
            return new Vector2(position.X + offsets[frame].X - fixedSize.X / 2, position.Y + offsets[frame].Y - fixedSize.Y / 2);
        }

    }
}
