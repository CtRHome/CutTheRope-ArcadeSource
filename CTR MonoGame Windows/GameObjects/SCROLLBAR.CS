using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class Scrollbar : GameObject
    {
        const float BAR_LENGTH = 1536;

        bool dragging;
        float moveOffset;

        Vector2 AdjustedPostion
        {
            get
            {
                return position + moveOffset * Vector2.UnitY;
            }
        }


        public Scrollbar(ContentManager content)
        {
            sprite = new MovableGrabSprite(content, true, BAR_LENGTH);
            (sprite as MovableGrabSprite).Alpha = 0.25f;
            rotation = 0;
            position = new Vector2(50, 192);
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);

            TripleLevelCam tlc = state.Camera as TripleLevelCam;
            if (tlc == null)
            {
                return;
            }

            if (state.Input.MouseJustClicked())
            {
                if ((state.Input.MousePos - AdjustedPostion).LengthSquared() < 30 * 30 * SingleLevel.SCALE * SingleLevel.SCALE)
                {
                    dragging = true;
                    state.Input.ConsumeClick();
                }
            }
            else if (state.Input.MouseButtonDown && dragging)
            {
                moveOffset += state.Input.MousePos.Y - state.Input.LastMousePos.Y;
                moveOffset = Math.Max(0, Math.Min(BAR_LENGTH, moveOffset));

                tlc.SetPositionY(moveOffset / BAR_LENGTH * tlc.MaxCameraY);
            }
            else
            {
                dragging = false;

                moveOffset = tlc.Position.Y / tlc.MaxCameraY * BAR_LENGTH;
            }

            (sprite as MovableGrabSprite).SetGrabPosition(moveOffset);
            (sprite as MovableGrabSprite).SetHighlight(dragging);
            (sprite as MovableGrabSprite).Alpha = 0.9f * (sprite as MovableGrabSprite).Alpha + 0.1f * (dragging ? 1 : 0.25f);
        }

        public override void Draw(SpriteBatch sb, Vector2 cameraPosition)
        {
            sprite.Draw(sb, position, rotation);
            (sprite as MovableGrabSprite).DrawTop(sb, position, rotation);
        }
    }
}
