using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class Background
    {
        Texture2D image;
        Rectangle main, vPatch, hPatch;
        Point patchOffsets;

        public Background(Texture2D image, Rectangle main, Rectangle hPatch, Rectangle vPatch, Point patchOffsets)
        {
            this.image = image;
            this.main = main;
            this.vPatch = vPatch;
            this.hPatch = hPatch;
            this.patchOffsets = patchOffsets;
        }

        internal void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, Point worldSize)
        {
            int LEVEL_WIDTH = 1080;
            int LEVEL_HEIGHT = 1920;

            Vector2 camPos = new Vector2((int)cameraPosition.X, (int)cameraPosition.Y);

            spriteBatch.Draw(image, new Rectangle(-(int)cameraPosition.X, -(int)cameraPosition.Y, LEVEL_WIDTH, LEVEL_HEIGHT), main, Color.White);

            float wScale = LEVEL_WIDTH / 800f;

            for (int i = 1; i < worldSize.X; i++)
            {
                spriteBatch.Draw(image, new Rectangle(-(int)cameraPosition.X + i * LEVEL_WIDTH, -(int)cameraPosition.Y, LEVEL_WIDTH, LEVEL_HEIGHT), main, Color.White);
                spriteBatch.Draw(image, new Rectangle(-(int)cameraPosition.X + (i - 1) * LEVEL_WIDTH + (int)(patchOffsets.X * wScale), -(int)cameraPosition.Y, (int)(hPatch.Width * wScale), LEVEL_HEIGHT), hPatch, Color.White);
            }

            float vScale = LEVEL_HEIGHT / 1280f;

            for (int i = 1; i < worldSize.Y; i++)
            {
                spriteBatch.Draw(image, new Rectangle(-(int)cameraPosition.X, -(int)cameraPosition.Y + i * LEVEL_HEIGHT, LEVEL_WIDTH, LEVEL_HEIGHT), main, Color.White);
                spriteBatch.Draw(image, new Rectangle(-(int)cameraPosition.X, -(int)cameraPosition.Y + (i - 1) * LEVEL_HEIGHT + (int)(patchOffsets.Y * vScale), LEVEL_WIDTH, (int)(vPatch.Height * vScale)), vPatch, Color.White);
            }
        }

        public void DrawFullScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, new Rectangle(0, 0, 1080, 1920), main, Color.White);
        }

        public void DrawMini(SpriteBatch spriteBatch, Point worldSize, int yOffset)
        {
            float vScale = 180f / 1280f;

            spriteBatch.Draw(image, new Rectangle(0, yOffset * 180 + 100, 120, 180), main, Color.White);

            for (int i = 1; i < worldSize.Y; i++)
            {
                spriteBatch.Draw(image, new Rectangle(0, 180 * (yOffset + i) + 100, 120, 180), main, Color.White);
                spriteBatch.Draw(image, new Rectangle(0, 180 * (yOffset + i - 1) + 100 + (int)(patchOffsets.Y * vScale), 120, (int)(vPatch.Height * vScale)), vPatch, Color.White);
            }
        }

    }
}
