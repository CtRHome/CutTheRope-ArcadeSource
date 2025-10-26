using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    static class GLDrawer
    {
        private static Texture2D pixelTexture;
        private static SpriteBatch spriteBatch;

        public static void Initialize(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            GLDrawer.spriteBatch = spriteBatch;
            
            // Create a 1x1 white pixel texture for drawing lines
            pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
        }

        public static void DrawAntialiasedLine(Vector2 p1, Vector2 p2, float size, Color color)
        {
            DrawLine(p1, p2, size, color);
        }

        public static void DrawAttachedAntialiasedLine(Vector2 p1, Vector2 p2, float size, Color color, ref Vector2? last_vlp, ref Vector2? last_vlpn)
        {
            DrawLine(p1, p2, size, color);
            // For attached lines, we don't need to maintain the last_vlp and last_vlpn state
            // since MonoGame handles this differently
        }

        private static void DrawLine(Vector2 p1, Vector2 p2, float thickness, Color color)
        {
            if (spriteBatch == null || pixelTexture == null) return;

            Vector2 direction = p2 - p1;
            float length = direction.Length();
            
            if (length == 0) return;

            direction.Normalize();
            
            // Calculate rotation angle
            float rotation = (float)System.Math.Atan2(direction.Y, direction.X);
            
            // Draw the line as a stretched rectangle
            // Note: scaling is handled by SpriteBatch transform matrix
            spriteBatch.Draw(
                pixelTexture,
                p1,
                null,
                color,
                rotation,
                Vector2.Zero,
                new Vector2(length, thickness),
                SpriteEffects.None,
                0f
            );
        }

        public static void Dispose()
        {
            if (pixelTexture != null)
            {
                pixelTexture.Dispose();
            }
        }
    }
}