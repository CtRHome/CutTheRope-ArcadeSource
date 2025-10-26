using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
	class SkipButtonSprite : ButtonSprite
	{
		public float Alpha;
		
        public SkipButtonSprite(ContentManager content)
            :base(content.Load<Texture2D>("skipButton"),
            new Rectangle[] { new Rectangle(0, 0, 506, 186), new Rectangle(506, 0, 506, 186) },
            new Point[] {Point.Zero, Point.Zero }, new Point(132, 47), 0, 1)
        {}
		
		public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            currentFrame = (pressed ? pressedFrame : releasedFrame);
            sb.Draw(image, position, frames[currentFrame], new Color(Color.White, Alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), 1, SpriteEffects.None, 1);
        }
	}
}

