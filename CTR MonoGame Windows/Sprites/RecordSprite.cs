using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class RecordSprite : AnimatedSprite
    {
        const int discFrame = 0;
        const int highlightFrame = 1;
        const int stickerFrame = 2;
        const int centerFrame = 3;
        const int activeHandleFrame = 4;
        const int handleFrame = 5;


        float size, scale, stickerScale, handleScale, centerScale;
        bool singleHandleOnly;
        int activeHandle;
        Texture2D recordHighlight;
        Vector2 recordHighlightSize;

        public RecordSprite(ContentManager content, float size, bool singleHandle)
            : base(content.Load<Texture2D>("obj_vinil_hd"), "2,2,426,426,431,1,426,398,859,1,116,233,1,431,31,31,859,236,164,141,859,379,148,125",
            "72,550,72,550,381,433,482,534,414,867,422,875", new Point(800, 1200))
        {
            this.size = size;

            scale = size / (float)(frames[discFrame].Width);
            stickerScale = Math.Max(0.4f, scale);
            handleScale = Math.Max(0.75f, scale);
            centerScale = 1.0f - (1.0f - stickerScale) * 0.5f;

            singleHandleOnly = singleHandle;

            recordHighlight = content.Load<Texture2D>("recordHighlight");
            recordHighlightSize = new Vector2(438, 438);
        }

        public void SetHighlight(int handle)
        {
            activeHandle = handle;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            if (activeHandle > 0)
            {
                sb.Draw(recordHighlight, position, null, Color.White, 0, new Vector2(recordHighlightSize.X, 0), scale, SpriteEffects.None, 1);
                sb.Draw(recordHighlight, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 1);
                sb.Draw(recordHighlight, position, null, Color.White, 0, new Vector2(0, recordHighlightSize.Y), scale, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 1);
                sb.Draw(recordHighlight, position, null, Color.White, 0, recordHighlightSize, scale, SpriteEffects.FlipVertically, 1);
            }

            Vector2 frameSize = PtoV(fixedSize) / 2 - PtoV(offsets[discFrame]);
            frameSize = new Vector2(frames[discFrame].Width, frames[discFrame].Height);
            sb.Draw(image, position, frames[discFrame], Color.White, 0, new Vector2(frameSize.X, 0), scale, SpriteEffects.None, 1);
            sb.Draw(image, position, frames[discFrame], Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 1);
            sb.Draw(image, position, frames[discFrame], Color.White, 0, new Vector2(0, frameSize.Y), scale, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 1);
            sb.Draw(image, position, frames[discFrame], Color.White, 0, frameSize, scale, SpriteEffects.FlipVertically, 1);

            sb.Draw(image, position, frames[highlightFrame], Color.White, 0, new Vector2(frameSize.X, 0), scale, SpriteEffects.None, 1);
            sb.Draw(image, position, frames[highlightFrame], Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 1);

            frameSize = new Vector2(frames[stickerFrame].Width, frames[stickerFrame].Height / 2);
            sb.Draw(image, position, frames[stickerFrame], Color.White, rotation, frameSize, stickerScale, SpriteEffects.None, 1);
            frameSize = new Vector2(0, frames[stickerFrame].Height / 2);
            sb.Draw(image, position, frames[stickerFrame], Color.White, rotation, frameSize, stickerScale, SpriteEffects.FlipHorizontally, 1);

            frameSize = new Vector2(frames[centerFrame].Width, frames[centerFrame].Height);
            sb.Draw(image, position, frames[centerFrame], Color.White, 0, frameSize / 2, centerScale, SpriteEffects.None, 1);

            frameSize = new Vector2(frames[handleFrame].Width, frames[handleFrame].Height) / 2;
            Vector2 highlightFrameSize = new Vector2(frames[activeHandleFrame].Width, frames[activeHandleFrame].Height) / 2;
            Vector2 handleOffset = Util.RotateVector(Vector2.UnitY * (size - 38 * handleScale), rotation);

            sb.Draw(image, position + handleOffset, frames[handleFrame], Color.White, rotation, frameSize, handleScale, SpriteEffects.None, 1);
            if (activeHandle == 1)
            {
                sb.Draw(image, position + handleOffset, frames[activeHandleFrame], Color.White, rotation, highlightFrameSize, handleScale, SpriteEffects.None, 1);
            }            
            if (!singleHandleOnly)
            {
                sb.Draw(image, position - handleOffset, frames[handleFrame], Color.White, rotation + (float)Math.PI, frameSize, handleScale, SpriteEffects.None, 1);
                if (activeHandle == 2)
                {
                    sb.Draw(image, position - handleOffset, frames[activeHandleFrame], Color.White, rotation + (float)Math.PI, highlightFrameSize, handleScale, SpriteEffects.None, 1);
                }
            }
        }
    }
}
