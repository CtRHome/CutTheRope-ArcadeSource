using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class SpikeRotateButton : Button
    {
        public bool Flipped
        {
            get { return (sprite as SpikeRotateButtonSprite).Flipped; }
            set { (sprite as SpikeRotateButtonSprite).Flipped = value; }
        }

        public SpikeRotateButton(ContentManager content, Vector2 position, int group, bool bonus)
            :base(position, 15 * SingleLevel.SCALE)
        {
            sprite = new SpikeRotateButtonSprite(content, group, bonus);
        }

        public void SetRotation(float rotation)
        {
            this.rotation = rotation;
        }
    }
}
