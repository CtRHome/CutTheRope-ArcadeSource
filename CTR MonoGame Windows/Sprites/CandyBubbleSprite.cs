using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class CandyBubbleSprite:AnimatedSprite
    {

        public CandyBubbleSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_bubble_flight_hd"), "1,1,112,137,115,1,114,136,231,1,119,130,352,1,125,125,1,140,131,119,134,140,135,114,271,140,138,112,1,261,136,114,1,377,133,116,1,495,127,122,1,619,123,128,1,749,118,132,1,883,114,136,126,619,112,137",
            "44,32,43,33,41,36,38,38,35,41,33,44,31,45,32,44,34,43,37,40,39,37,41,35,43,33,44,32", new Point(200,200))
        {
            AddAnimation(0, new Animation(0.05, 0, 12, Animation.LoopType.Repeat));
            SetAnimation(0);
        }
    }
}
