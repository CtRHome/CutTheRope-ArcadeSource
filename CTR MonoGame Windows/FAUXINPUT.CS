using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class FauxInput : Input
    {
        Vector2 mousePos, lastMousePos;
        bool mouseButton, lastMouseButton;

        public override void Update()
        {
            lastMouseButton = mouseButton;
            lastMousePos = mousePos;
        }

        public void SetMouse(Vector2 position, bool button)
        {
            mousePos = position;
            mouseButton = button;
        }

        public override Vector2 LastMousePos
        {
            get
            {
                return lastMousePos;
            }
        }

        public override Vector2 MousePos
        {
            get
            {
                return mousePos;
            }
        }

        public override bool MouseJustClicked()
        {
            return mouseButton && !lastMouseButton;
        }

        public override bool MouseButtonDown
        {
            get
            {
                return mouseButton;
            }
        }

        public override bool MouseJustClicked(int touch)
        {
            return touch == 0 && MouseJustClicked();
        }

        public override Vector2 LastTouchPos(int touch)
        {
            return lastMousePos;
        }

        public override bool TouchDown(int touch)
        {
            return touch == 0 && MouseButtonDown;
        }

        public override Vector2 TouchPos(int touch)
        {
            return mousePos;
        }
    }
}
