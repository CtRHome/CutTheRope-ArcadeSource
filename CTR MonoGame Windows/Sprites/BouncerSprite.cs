using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class BouncerSprite : AnimatedSprite
    {
        public float Width
        {
            get { return frames[0].Width; }
        }

        public BouncerSprite(ContentManager content, int size)
            : base(content.Load<Texture2D>("obj_bouncer_0" + size + "_hd"), size == 1 ? "1,1,156,103,1,106,161,83,1,191,164,77,1,270,155,114,1,386,156,89"
            : "1,1,243,99,246,1,256,80,246,83,259,76,1,102,235,109,246,161,243,89",
            size == 1 ? "257,55,255,65,253,70,257,44,257,62" :
            "214,55,208,67,206,69,218,46,214,61",
            new Point(670, 200))
        {
            AddAnimation(0, new Animation(0.04, 0, 4, Animation.LoopType.Stop));
            currentFrame = 0;
        }

        public void Bounce()
        {
            SetAnimation(0);
        }
    }
}
