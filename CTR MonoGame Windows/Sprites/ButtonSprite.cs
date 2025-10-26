using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    abstract class ButtonSprite : AnimatedSprite
    {
        protected bool pressed;

        protected int releasedFrame, pressedFrame;

        public ButtonSprite(Texture2D image, string magicFrameBoundsString, string magicOffsetsString, Point fixedSize, int releasedFrame, int pressedFrame)
            : base(image, magicFrameBoundsString, magicOffsetsString, fixedSize)
        {
            this.releasedFrame = releasedFrame;
            this.pressedFrame = pressedFrame;
        }

        protected ButtonSprite(Texture2D image, Rectangle[] frames, Point[] offsets, Point fixedSize, int releasedFrame, int pressedFrame)
            : base(image, new List<Rectangle>(frames), new List<Point>(offsets), fixedSize)
        {
            this.releasedFrame = releasedFrame;
            this.pressedFrame = pressedFrame;
        }

        public void SetPressed(bool pressed)
        {
            this.pressed = pressed;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            currentFrame = (pressed ? pressedFrame : releasedFrame);
            base.Draw(sb, position, rotation);
        }

        public override void DrawMiniMap(SpriteBatch sb, Vector2 miniPos, float rotation)
        {
            currentFrame = (pressed ? pressedFrame : releasedFrame);
            base.DrawMiniMap(sb, miniPos, rotation);
        }

    }
}
