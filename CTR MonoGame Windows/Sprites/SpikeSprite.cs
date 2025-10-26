using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class SpikeSprite : Sprite
    {
        public float Width
        {
            get { return frame.Width; }
        }

        public SpikeSprite(ContentManager content, int size, bool rotatable, bool bonus)
            : base(content.Load<Texture2D>("obj_" + (rotatable ? "rotatable_" : "") +"spikes_0" + size + (bonus ? "_bonus" : "") + "_hd"), GetFrame(size, rotatable), GetOffset(size, rotatable), new Point(rotatable ? 668 : 670,200))
        {

        }

        private static Point GetOffset(int size, bool rotatable)
        {
            if (rotatable)
            {
                switch (size)
                {
                    default:
                    case 1:
                        return new Point(253, 70);
                    case 2:
                        return new Point(208, 70);
                    case 3:
                        return new Point(154, 63);
                    case 4:
                        return new Point(110, 62);
                }
            }
            else
            {
                switch (size)
                {
                    default:
                    case 1:
                        return new Point(249, 64);
                    case 2:
                        return new Point(202, 64);
                    case 3:
                        return new Point(154, 61);
                    case 4:
                        return new Point(107, 61);
                }
            }
        }

        private static Rectangle GetFrame(int size, bool rotatable)
        {
            if (rotatable)
            {
                switch (size)
                {
                    default:
                    case 1:
                        return new Rectangle(1, 1, 162, 64);
                    case 2:
                        return new Rectangle(1, 1, 256, 66);
                    case 3:
                        return new Rectangle(1, 1, 361, 74);
                    case 4:
                        return new Rectangle(1, 1, 448, 71);
                }
            }
            else
            {
                switch (size)
                {
                    default:
                    case 1:
                        return new Rectangle(1, 1, 171, 74);
                    case 2:
                        return new Rectangle(1, 1, 268, 74);
                    case 3:
                        return new Rectangle(1, 1, 364, 74);
                    case 4:
                        return new Rectangle(1, 1, 455, 74);
                }
            }
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            sb.Draw(image, position, frame, Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offset), 1, SpriteEffects.None, 1f);
        }//*/
    }
}
