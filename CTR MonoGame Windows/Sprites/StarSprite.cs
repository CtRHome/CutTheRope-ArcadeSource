using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class StarSprite : AnimatedSprite
    {
        const int IMG_OBJ_STAR_IDLE_glow = 0;
        const int IMG_OBJ_STAR_IDLE_idle_start = 1;
        const int IMG_OBJ_STAR_IDLE_idle_end = 18;
        const int IMG_OBJ_STAR_IDLE_timed_start = 19;
        const int IMG_OBJ_STAR_IDLE_timed_end = 55;
        const int IMG_OBJ_STAR_IDLE_gravity_down = 56;
        const int IMG_OBJ_STAR_IDLE_gravity_up = 57;
        const int IMG_OBJ_STAR_IDLE_window = 58;

        float life;


        public StarSprite(ContentManager content)
            :base(content.Load<Texture2D>("obj_star_idle_hd"), "1,1,188,178,191,1,61,62,191,65,56,62,254,1,51,62,254,65,46,62,307,1,41,62,307,65,37,62,350,1,32,62,350,65,27,62,384,1,23,63,384,66,23,63,409,1,29,62,440,1,34,62,476,1,39,62,517,1,45,62,564,1,50,62,616,1,55,62,673,1,60,62,735,1,66,62,803,1,142,142,1,181,142,142,145,181,142,142,289,181,142,142,433,181,142,142,577,181,142,142,721,181,142,142,865,181,142,142,1,325,142,142,145,325,142,142,289,325,142,142,433,325,142,142,577,325,142,142,721,325,142,142,865,325,142,142,1,469,142,142,145,469,142,142,289,469,142,142,433,469,142,142,577,469,142,142,721,469,142,142,865,469,142,142,1,613,142,142,145,613,142,142,289,613,142,142,433,613,142,142,577,613,142,142,721,613,142,142,865,613,142,142,1,757,142,142,145,757,142,142,289,757,142,142,433,757,142,142,577,757,142,142,721,757,142,142,865,757,142,142,1,901,142,142,1,1045,183,185,1,1232,183,185,186,1045,330,330",
            "129,133,191,193,194,193,196,193,199,193,201,193,203,193,205,193,208,193,210,193,210,193,207,193,205,193,202,193,199,193,197,193,194,193,192,193,189,193,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,151,155,129,136,129,136,55,58", new Point(444, 444))
        {
            AddAnimation(0, new Animation(0.05, IMG_OBJ_STAR_IDLE_idle_start, IMG_OBJ_STAR_IDLE_idle_end, Animation.LoopType.Repeat));
            currentFrame = Util.R.Next(17) + 1;
            currentAnimation = 0;
            life = -1;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            sb.Draw(image, GetGlowDrawPos(position), frames[IMG_OBJ_STAR_IDLE_glow], Color.White);
            if (life > 0)
            {
				DrawTimer(sb, position, 1 - life);
            }
            base.Draw(sb, position, rotation);
        }
		
		public void DrawTimer(SpriteBatch sb, Vector2 position, float timeLeft)
		{
            int lifeFrame = IMG_OBJ_STAR_IDLE_timed_start + (int)Math.Round((IMG_OBJ_STAR_IDLE_timed_end - IMG_OBJ_STAR_IDLE_timed_start) * (timeLeft));
            lifeFrame = Math.Min(IMG_OBJ_STAR_IDLE_timed_end, Math.Max(IMG_OBJ_STAR_IDLE_timed_start, lifeFrame));
            int cf = currentFrame;
            currentFrame = lifeFrame;
            base.Draw(sb, position, 0);
            currentFrame = cf;
		}

        private Vector2 GetGlowDrawPos(Vector2 position)
        {
            return new Vector2(position.X + offsets[IMG_OBJ_STAR_IDLE_glow].X - fixedSize.X / 2, position.Y + offsets[IMG_OBJ_STAR_IDLE_glow].Y - fixedSize.Y / 2);
        }

        internal void SetLife(float p)
        {
            life = p;
        }
    }
}
