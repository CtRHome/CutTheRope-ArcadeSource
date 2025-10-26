using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class BubblePopSprite : AnimatedSprite
    {
        public BubblePopSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_bubble_pop_hd"), "1,1,247,229,250,1,243,226,495,1,240,225,737,1,238,222,1,232,235,221,1,455,235,219,238,232,237,217,477,232,238,216,1,676,191,181,717,232,194,178,238,451,197,177,437,451,200,175",
            "67,66,70,67,72,69,73,72,75,75,74,79,72,84,71,89,82,96,78,103,74,111,69,119", new Point(361, 359))
        {
            AddAnimation(0, new Animation(0.05, 0, 11, Animation.LoopType.Vanish));
            SetAnimation(0);
        }
    }
}
