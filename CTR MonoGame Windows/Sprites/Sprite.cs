using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class Sprite
    {
        protected Texture2D image;
        protected Rectangle frame;
        protected Point offset;
        protected Point fixedSize;

        public const float MINI_SCALE = 0.1f;

        public Sprite()
        {}

        public Sprite(Texture2D image, Rectangle frame, Point offset, Point fixedSize)
        {
            this.image = image;
            this.frame = frame;
            this.offset = offset;
            this.fixedSize = fixedSize;
        }

        public Sprite(Texture2D image, string magicFrameBoundsString, string magicOffsetsString, Point fixedSize)
        {
            this.image = image;
            string[] frameBoundsList = magicFrameBoundsString.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            frame = new Rectangle(int.Parse(frameBoundsList[0]),
                int.Parse(frameBoundsList[1]),
                int.Parse(frameBoundsList[2]),
                int.Parse(frameBoundsList[3]));


            string[] offsetList = magicOffsetsString.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            offset = new Point(int.Parse(offsetList[0]), int.Parse(offsetList[1]));

            this.fixedSize = fixedSize;
        }

        public virtual void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            sb.Draw(image, position, frame, Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offset), 1, SpriteEffects.None, 1);
        }

        public virtual void DrawMiniMap(SpriteBatch sb, Vector2 miniPos, float rotation)
        {
            sb.Draw(image, miniPos, frame, Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offset), MINI_SCALE, SpriteEffects.None, 1);
        }

        protected Vector2 PtoV(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        public virtual void Update(GameTime gameTime)
        {}
    }
}
