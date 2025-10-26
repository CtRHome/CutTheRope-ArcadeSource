using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class CandyBlinkSprite : AnimatedSprite
    {
        public enum Animations { CANDY_BLINK_INITIAL, CANDY_BLINK_STAR, CANDY_PART_FX };

        const int IMG_OBJ_CANDY_FX_HD_part_fx_start = 0;
        const int IMG_OBJ_CANDY_FX_HD_part_fx_end = 4;
        const int IMG_OBJ_CANDY_FX_HD_highlight_start = 5;
        const int IMG_OBJ_CANDY_FX_HD_highlight_end = 14;
        const int IMG_OBJ_CANDY_FX_HD_glow = 15;

        float scale = 0.71f;

        public CandyBlinkSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_candy_fx_hd"), "1,1,203,216,206,1,223,245,206,248,298,318,506,248,310,319,506,569,304,311,1,219,118,118,1,339,123,130,1,471,123,133,1,606,125,136,1,744,130,141,431,1,128,140,1,887,128,128,561,1,121,121,561,124,116,121,684,1,110,118,206,568,243,243",
            "55,66,40,30,5,-1,0,-4,6,1,95,103,93,93,93,90,93,90,93,90,95,93,98,108,105,115,110,115,113,118,38,45", new Point(316,336))
        {
            AddAnimation((int)Animations.CANDY_BLINK_INITIAL, new Animation(0.07, IMG_OBJ_CANDY_FX_HD_highlight_start, IMG_OBJ_CANDY_FX_HD_highlight_end, Animation.LoopType.Vanish));

            AddAnimation((int)Animations.CANDY_BLINK_STAR, new Animation(0.3, IMG_OBJ_CANDY_FX_HD_glow, IMG_OBJ_CANDY_FX_HD_glow, Animation.LoopType.Vanish));

            AddAnimation((int)Animations.CANDY_PART_FX, new Animation(0.05, IMG_OBJ_CANDY_FX_HD_part_fx_start, IMG_OBJ_CANDY_FX_HD_part_fx_end, Animation.LoopType.Vanish));
        }

        internal void SetAnimation(Animations animation)
        {
            base.SetAnimation((int)animation);
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            if (currentFrame < 0 || currentFrame > frames.Count)
            {
                return;
            }
            sb.Draw(image, position, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), scale, SpriteEffects.None, 1);
        }
    }
}
