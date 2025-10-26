using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class SpikeRotateButtonSprite : ButtonSprite
    {
        public bool Flipped;

        public SpikeRotateButtonSprite(ContentManager content, int group, bool bonus)
            : base(content.Load<Texture2D>("obj_rotatable_spikes_button" + (bonus ? "_bonus" : "") + "_hd"), "1,1,81,82,1,85,81,82,1,169,81,82,1,253,81,82", "295,59,294,59,295,59,295,59",
            new Point(668, 200), group * 2 + 0, group * 2 + 1)
        {
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            currentFrame = (pressed ? pressedFrame : releasedFrame);
            sb.Draw(image, position, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), 1, Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
        }
    }
}
