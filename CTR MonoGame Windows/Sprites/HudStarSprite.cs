using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class HudStarSprite : AnimatedSprite
    {
        public HudStarSprite(ContentManager content)
            : base(content.Load<Texture2D>("hud_star_hd"), "1,1,68,68,71,1,47,68,1,71,27,68,30,71,10,68,42,71,22,68,66,71,36,68,1,141,47,68,50,141,56,68,1,211,63,68,1,281,67,68,1,351,68,68",
            "4,4,15,4,25,4,33,4,27,4,20,4,15,4,10,4,7,4,5,4,4,4", new Point(80, 80))
        {
            AddAnimation(0, new Animation(0.05, 0, 10, Animation.LoopType.Stop));
            currentFrame = 0;
        }

        public void Earn()
        {
            SetAnimation(0);
        }
    }
}
