using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class StarDisappearSprite : AnimatedSprite
    {
        public StarDisappearSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_star_disappear_hd"), "1,1,198,216,201,1,270,298,1,219,165,187,473,1,248,285,201,301,311,371,201,674,261,321,723,1,270,265,514,301,239,213,1,408,194,186,1,596,167,175,1,773,165,166,755,301,161,157,514,516,158,107",
            "117,94,61,31,134,106,91,45,61,-1,91,28,70,66,82,92,103,100,119,105,120,108,122,111,123,114", new Point(444, 444))
        {
            AddAnimation(0, new Animation(0.05, 0, 12, Animation.LoopType.Vanish));
            currentAnimation = 0;
        }
    }
}
