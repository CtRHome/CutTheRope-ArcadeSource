using Microsoft.Xna.Framework;
using System;

namespace CTR_MonoGame
{
    class Camera
    {
        public enum CameraState { Static, InitialScroll, Follow };

        public CameraState State
        {
            get;
            protected set;
        }

        protected Vector2 position;

        public Vector2 Position
        {
            get
            {
                return Clamp(position);
            }
        }

        public bool IgnoreTouches
        {
            get;
            protected set;
        }

        Vector2 cameraTarget;

        protected float speed;
        protected readonly Vector2 ScreenSize = new Vector2(1080, 1920);
        protected Vector2 worldSize;
        float initialCameraToCandyDistance;

        public Camera(Point levelSize, GlobalState state)
        {
            worldSize = new Vector2(1080 * levelSize.X, SingleLevel.LEVEL_HEIGHT * levelSize.Y + SingleLevel.Y_OFFSET);
            Vector2 startPosition = Vector2.Zero;
            State = CameraState.InitialScroll;
            IgnoreTouches = true;

            if (levelSize.Y > 1)
            {
                if (state.Candy.Position.Y < worldSize.Y / 2)
                {
                    startPosition.Y = worldSize.Y - 1920;
                }
            }
            else if (levelSize.X > 1)
            {
                if (state.Candy.Position.X < worldSize.X / 2)
                {
                    startPosition.X = worldSize.X - 1080;
                }
            }
            else
            {
                State = CameraState.Static;
                IgnoreTouches = false;
            }

            position = startPosition;
            initialCameraToCandyDistance = (startPosition - Clamp(state.Candy.Position - ScreenSize / 2)).Length();
            cameraTarget = Clamp(state.Candy.Position - ScreenSize / 2) + ScreenSize / 2;
        }

        public virtual void Update(GameTime gameTime, GlobalState state)
        {
            Vector2 cameraToCandy = Clamp(state.Candy.Position - ScreenSize / 2) - position;
            Vector2 cameraToTarget = cameraTarget - (position + ScreenSize / 2);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float ignoreTouchDistance = 50 * SingleLevel.SCALE;
            if (cameraToCandy.Length() < ignoreTouchDistance || State == CameraState.Static)
            {
                IgnoreTouches = false;
            }

            switch (State)
            {
                case CameraState.Static:
                    return;
                case CameraState.InitialScroll:
                    if (cameraToCandy.Length() > initialCameraToCandyDistance / 3)
                    {
                        speed = Math.Min(speed + elapsed * 200 * SingleLevel.SCALE, 300 * SingleLevel.SCALE);
                    }
                    else
                    {
                        speed = Math.Max(speed - elapsed * 200 * SingleLevel.SCALE, 50 * SingleLevel.SCALE);
                    }
                    position += Vector2.Normalize(cameraToTarget) * speed * elapsed;
                    if (cameraToTarget.LengthSquared() < 4 || (!IgnoreTouches && cameraToCandy.Length() > ignoreTouchDistance))
                    {
                        State = CameraState.Follow;
                    }
                    break;
                case CameraState.Follow:
                    position += cameraToCandy * 7 * elapsed;
                    break;
                default:
                    break;
            }

        }

        protected Vector2 Clamp(Vector2 position)
        {
            if (position.X > worldSize.X - ScreenSize.X)
            {
                position.X = worldSize.X - ScreenSize.X;
            }
            if (position.X < 0)
            {
                position.X = 0;
            }
            if (position.Y > worldSize.Y - ScreenSize.Y)
            {
                position.Y = worldSize.Y - ScreenSize.Y;
            }
            if (position.Y < 0)
            {
                position.Y = 0;
            }
            return position;
        }
    }
}
