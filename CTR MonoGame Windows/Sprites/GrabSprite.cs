using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class GrabSprite : AnimatedSprite
    {
        public GrabSprite(ContentManager content)
            :this(content, Util.R.Next(2) + 1)
        {}

        public GrabSprite(ContentManager content, int type)
            : base(content.Load<Texture2D>("obj_hook_0" + type + "_hd"), "1,1,100,101,1,104,30,28", type == 1 ? "63,61,98,97" : "60,61,95,97", new Point(220, 220))
        {}

        protected GrabSprite(Texture2D image, string magicFrameBoundsString, string magicOffsetsString, Point fixedSize)
            : base(image, magicFrameBoundsString, magicOffsetsString, fixedSize)
        {}

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            currentFrame = 0;
            base.Draw(sb, position, 0);
        }

        protected void DrawFrame(SpriteBatch sb, Vector2 position, float rotation, int frame)
        {
            currentFrame = frame;
            base.Draw(sb, position, 0);
        }

        public virtual void DrawTop(SpriteBatch sb, Vector2 position, float rotation)
        {
            currentFrame = 1;
            base.Draw(sb, position, 0);
        }
    }
}
