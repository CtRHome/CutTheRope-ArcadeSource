using Microsoft.Xna.Framework;
using System;

namespace CTR_MonoGame
{
    class TripleLevelCam : Camera
    {
        public float MaxCameraY
        {
            get;
            protected set;
        }

        public TripleLevelCam(Point levelSize, GlobalState state)
            : base(levelSize, state)
        {
            MaxCameraY = SingleLevel.LEVEL_HEIGHT * (levelSize.Y - 1);
        }


        public override void Update(GameTime gameTime, GlobalState state)
        {
            Vector2 clampedCameraToCandy = Clamp(state.Candy.Position - ScreenSize / 2) - Position;
            Vector2 cameraToCandy = state.Candy.Position - (position + ScreenSize / 2);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float ignoreTouchDistance = 50 * SingleLevel.SCALE;
            if (clampedCameraToCandy.Length() < ignoreTouchDistance || State == CameraState.Static)
            {
                IgnoreTouches = false;
            }
            float hysteresisFactor = 0.1f;

            switch (State)
            {
                case CameraState.Static:
                    return;
                case CameraState.InitialScroll:
                    if (Position.Y > SingleLevel.LEVEL_HEIGHT / 2f)
                    {
                        speed = Math.Min(speed + elapsed * 200 * SingleLevel.SCALE, 300 * SingleLevel.SCALE);
                    }
                    else
                    {
                        speed = Math.Max(speed - elapsed * 200 * SingleLevel.SCALE, 50 * SingleLevel.SCALE);
                    }
                    position -= Vector2.UnitY * speed * elapsed;
                    if (Position.Y < 10 || (!IgnoreTouches && cameraToCandy.Length() > hysteresisFactor * SingleLevel.LEVEL_HEIGHT))
                    {
                        State = CameraState.Follow;
                    }
                    break;
                case CameraState.Follow:
                    float c2cL = cameraToCandy.Length();
                    if (c2cL > hysteresisFactor * SingleLevel.LEVEL_HEIGHT)
                    {
                        position += Vector2.Normalize(cameraToCandy) * (c2cL - hysteresisFactor * SingleLevel.LEVEL_HEIGHT) * 7 * elapsed;
                    }
                    break;
                default:
                    break;
            }

        }

        internal void SetPositionY(float p)
        {
            position.Y = p;
        }
    }
}
