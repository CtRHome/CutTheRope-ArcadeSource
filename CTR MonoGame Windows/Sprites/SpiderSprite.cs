using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class SpiderSprite : AnimatedSprite
    {
        const int IMG_OBJ_SPIDER_busted = 11;
        const int IMG_OBJ_SPIDER_stealing = 12;

        public SpiderSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_spider_hd"), "1,1,71,68,1,71,37,122,40,71,85,119,1,195,63,129,66,195,75,124,143,195,71,126,127,71,37,122,1,326,116,116,119,326,111,114,1,444,116,116,119,444,118,114,1,562,130,113,1,677,104,125",
            "189,153,206,97,182,100,193,90,187,95,189,93,206,97,171,177,176,186,171,177,169,186,163,143,170,220", new Point(444, 444))
        {
            AddAnimation(0, new Animation(0.05, 0, 5, Animation.LoopType.Stop));
            AddAnimation(1, new Animation(0.05, 6, 6, Animation.LoopType.Stop));
            AddAnimation(2, new Animation(0.1, 7, 10, Animation.LoopType.Repeat));
            AddAnimation(3, new Animation(0.5, 11, 11, Animation.LoopType.Stop));
            AddAnimation(4, new Animation(0.5, 12, 12, Animation.LoopType.Stop));
            AddAnimation(5, new Animation(0.5, 11, 11, Animation.LoopType.Stop));
            AddAnimation(6, new Animation(0.5, 12, 12, Animation.LoopType.Stop));

            SetNextAnimation(1, 0, 0.4);
            SetNextAnimation(2, 1, 0.05);

            SetNextAnimation(5, 3, 0.5);
            SetNextAnimation(6, 4, 0.5);
            SetNextAnimation(3, 5, 0.5);
            SetNextAnimation(4, 6, 0.5);

            currentFrame = 0;
        }

        public void Wake()
        {
            SetAnimation(0);
        }

        public void Die(bool happy)
        {
            SetAnimation(happy ? 4 : 3);
        }

        public bool Walking
        {
            get { return currentAnimation == 2; }
        }

    }
}
