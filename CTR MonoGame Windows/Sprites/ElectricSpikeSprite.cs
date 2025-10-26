using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class ElectricSpikeSprite : AnimatedSprite
    {
        public float Width
        {
            get { return frames[0].Width; }
        }

        public ElectricSpikeSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_electrodes_hd"), "1,1,408,68,1,71,408,80,1,153,408,80,1,235,408,77,1,314,408,74",
            "131,66,131,59,131,57,131,57,131,60", new Point(670,200))
        {
            AddAnimation(0, new Animation(0.05, 0, 0, Animation.LoopType.Repeat));
            AddAnimation(1, new Animation(0.05, 1, 4, Animation.LoopType.Repeat));
            SetAnimation(0);
        }

        public void SetPower(bool power)
        {
            SetAnimation(power ? 1 : 0);
        }
    }
}
