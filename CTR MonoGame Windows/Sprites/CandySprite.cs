using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class CandySprite : AnimatedSprite
    {
        const int IMG_OBJ_CANDY_01_HD_candy_bottom = 0;
        const int IMG_OBJ_CANDY_01_HD_candy_main = 1;
        const int IMG_OBJ_CANDY_01_HD_candy_top = 2;
        const int IMG_OBJ_CANDY_01_HD_piece_01 = 3;
        const int IMG_OBJ_CANDY_01_HD_piece_02 = 4;
        const int IMG_OBJ_CANDY_01_HD_piece_03 = 5;
        const int IMG_OBJ_CANDY_01_HD_piece_04 = 6;
        const int IMG_OBJ_CANDY_01_HD_piece_05 = 7;
        const int IMG_OBJ_CANDY_01_HD_part_1 = 8;
        const int IMG_OBJ_CANDY_01_HD_part_2 = 9;

        bool half, leftHalf;
        float scale = 0.71f;
        public float Alpha = 1;
        public int Bit = -1;

        public CandySprite(ContentManager content)
            : this(content, false, false)
        {}

        public CandySprite(ContentManager content, bool half, bool leftHalf)
            : base(content.Load<Texture2D>("obj_candy_01_hd"), "1,1,175,181,1,184,121,121,1,307,126,126,124,184,78,50,178,1,39,37,178,40,40,47,178,89,44,46,204,184,42,48,1,435,88,128,129,307,78,126",
            "83,105,98,107,96,106,117,141,135,146,137,143,135,146,129,141,114,106,124,106", new Point(316, 336))
        {
            this.half = half;
            this.leftHalf = leftHalf;
        }

        public void Merge()
        {
            half = false;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            if (Bit >= 0)
            {
                sb.Draw(image, position, frames[IMG_OBJ_CANDY_01_HD_piece_01 + Bit], new Color(Color.White, Alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offsets[IMG_OBJ_CANDY_01_HD_piece_01 + Bit]), scale * Alpha, SpriteEffects.None, 1f);
            }
            else
            {
                if (half)
                {
                    if (leftHalf)
                    {
                        sb.Draw(image, position, frames[IMG_OBJ_CANDY_01_HD_part_1], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[IMG_OBJ_CANDY_01_HD_part_1]), scale, SpriteEffects.None, 1f);
                    }
                    else
                    {
                        sb.Draw(image, position, frames[IMG_OBJ_CANDY_01_HD_part_2], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[IMG_OBJ_CANDY_01_HD_part_2]), scale, SpriteEffects.None, 1f);
                    }
                }
                else
                {
                    sb.Draw(image, position, frames[IMG_OBJ_CANDY_01_HD_candy_bottom], new Color(Color.White, Alpha), 0f, PtoV(fixedSize) / 2 - PtoV(offsets[IMG_OBJ_CANDY_01_HD_candy_bottom]), scale * Alpha, SpriteEffects.None, 1f);
                    sb.Draw(image, position, frames[IMG_OBJ_CANDY_01_HD_candy_main], new Color(Color.White, Alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offsets[IMG_OBJ_CANDY_01_HD_candy_main]), scale * Alpha, SpriteEffects.None, 1f);
                    sb.Draw(image, position, frames[IMG_OBJ_CANDY_01_HD_candy_top], new Color(Color.White, Alpha), 0f, PtoV(fixedSize) / 2 - PtoV(offsets[IMG_OBJ_CANDY_01_HD_candy_top]), scale * Alpha, SpriteEffects.None, 1f);
                }
            }
        }

        public override void DrawMiniMap(SpriteBatch sb, Vector2 miniPos, float rotation)
        {
            if (Bit > 0)
            {
                currentFrame = IMG_OBJ_CANDY_01_HD_piece_01 + Bit;
                sb.Draw(image, miniPos, frames[currentFrame], new Color(Color.White, Alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), MINI_SCALE * scale * Alpha, SpriteEffects.None, 1);
            }
            else
            {
                if (half)
                {
                    if (leftHalf)
                    {
                        currentFrame = IMG_OBJ_CANDY_01_HD_part_1;
                        sb.Draw(image, miniPos, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), MINI_SCALE * scale, SpriteEffects.None, 1);
                    }
                    else
                    {
                        currentFrame = IMG_OBJ_CANDY_01_HD_part_2;
                        sb.Draw(image, miniPos, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), MINI_SCALE * scale, SpriteEffects.None, 1);
                    }
                }
                else
                {
                    currentFrame = IMG_OBJ_CANDY_01_HD_candy_bottom;
                    sb.Draw(image, miniPos, frames[currentFrame], new Color(Color.White, Alpha), 0, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), MINI_SCALE * scale * Alpha, SpriteEffects.None, 1);
                    currentFrame = IMG_OBJ_CANDY_01_HD_candy_main;
                    sb.Draw(image, miniPos, frames[currentFrame], new Color(Color.White, Alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), MINI_SCALE * scale * Alpha, SpriteEffects.None, 1);
                    currentFrame = IMG_OBJ_CANDY_01_HD_candy_top;
                    sb.Draw(image, miniPos, frames[currentFrame], new Color(Color.White, Alpha), 0, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), MINI_SCALE * scale * Alpha, SpriteEffects.None, 1);
                }
            }
        }
    }
}
