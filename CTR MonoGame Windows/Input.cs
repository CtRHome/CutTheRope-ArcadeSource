using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CTR_MonoGame
{
    public class Input
    {
        KeyboardState currentKeyboardState, lastKeyboardState;
        MouseState currentMouseState, lastMouseState;
        protected bool clickConsumed;
        public IOBoard IOBoard = new IOBoard();
        int stuckCounter;
        bool mouseWasDown;
        public const int TOUCH_COUNT = 5;
        volatile Vector2[] latestTouches = new Vector2[TOUCH_COUNT];
        Vector2[] touches = new Vector2[TOUCH_COUNT];
        Vector2[] lastTouches = new Vector2[TOUCH_COUNT];
        volatile bool[] latestTouchesActive = new bool[TOUCH_COUNT];
        bool[] touchesActive = new bool[TOUCH_COUNT];
        bool[] lastTouchesActive = new bool[TOUCH_COUNT];
        bool[] touchConsumed = new bool[TOUCH_COUNT];
        FileStream hidFile;

        public KeyboardState CurrentKeyboardState
        {
            get { return currentKeyboardState; }
        }

        public MouseState CurrentMouseState
        {
            get { return currentMouseState; }
        }

        public virtual void Update()
        {
            mouseWasDown = MouseButtonDown;

            lastKeyboardState = currentKeyboardState;
            lastMouseState = currentMouseState;

            lastTouches = (Vector2[])touches.Clone();
            lastTouchesActive = (bool[])touchesActive.Clone();

            touches = (Vector2[])latestTouches.Clone();
            touchesActive = (bool[])latestTouchesActive.Clone();


            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            if (CurrentMouseState.LeftButton == ButtonState.Pressed && MousePos == LastMousePos)
            {
                stuckCounter++;
            }
            else
            {
                stuckCounter = 0;
            }

            IOBoard.Update();

            if (Util.OnDevice)
            {
                if (hidFile == null)
                {
                    new Task(() =>
                    {
                        int n = 0;
                        while (!File.ReadAllText("/sys/class/hidraw/hidraw" + n
                                            + "/device/uevent").Contains("Baanto"))
                        {
                            n++;
                        }
                        while (true)
                        {
                            try
                            {
                                hidFile = File.OpenRead("/dev/hidraw" + n);
                                while (true)
                                {
                                    byte[] buf = new byte[256];

                                    hidFile.Read(buf, 0, 256);

                                    if (buf[0] == 0x0F)
                                    {
                                        for (int i = 0; i < touches.Length; i++)
                                        {
                                            int x = buf[6 * i + 1] * 256 + buf[6 * i + 2];
                                            int y = buf[6 * i + 3] * 256 + buf[6 * i + 4];
                                            if (x == 4095 && y == 4095)
                                            {
                                                latestTouchesActive[i] = false;
                                            }
                                            else
                                            {
                                                Vector2 newV = new Vector2(1080f - y * 1080f / 4094f, x * 1920f / 4094f);
                                                if (newV.X > 0 && newV.X < 1080 && newV.Y > 0 && newV.Y < 1920)
                                                {
                                                    latestTouches[i] = newV;
                                                }
                                                latestTouchesActive[i] = true;
                                            }
                                        }
                                    }
                                    else
                                    if (buf[0] == 0x0B && buf[1] == 0x1D)
                                    {
                                        //calibration
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            Thread.Sleep(1000);
                        }
                    }).Start();
                }


            }

            clickConsumed = false;

            for (int i = 0; i < touchConsumed.Length; i++)
            {
                touchConsumed[i] = false;
            }

        }

        public bool IsKeyDown(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public bool KeyJustPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
        }

        public void ConsumeClick()
        {
            clickConsumed = true;
        }

        public void ConsumeClick(int touch)
        {
            if (touch < 0)
            {
                ConsumeClick();
                return;
            }
            touchConsumed[touch] = true;
        }

        public virtual bool MouseJustClicked()
        {
            return !clickConsumed && MouseButtonDown && !mouseWasDown;
        }

        public virtual bool MouseJustClicked(int touch)
        {
            if (touch < 0)
                return MouseJustClicked();
            return !touchConsumed[touch] && touchesActive[touch] && !lastTouchesActive[touch];
        }

        public virtual bool MouseButtonDown
        {
            get { return stuckCounter < 5 && CurrentMouseState.LeftButton == ButtonState.Pressed; }
        }

        public virtual Vector2 LastMousePos
        {
            get
            {
                if (Util.OnDevice)
                {
                    return new Vector2(1080f - lastMouseState.Y * 1080f / 1920f, lastMouseState.X * 1920f / 1080f);
                }
                // Scale mouse position by 2x to match game world coordinates (540x960 window -> 1080x1920 game)
                return new Vector2(lastMouseState.X * 2, lastMouseState.Y * 2);
            }
        }

        public virtual Vector2 MousePos
        {
            get
            {
                if (Util.OnDevice)
                {
                    return new Vector2(1080f - currentMouseState.Y * 1080f / 1920f, currentMouseState.X * 1920f / 1080f);
                }
                // Scale mouse position by 2x to match game world coordinates (540x960 window -> 1080x1920 game)
                return new Vector2(currentMouseState.X * 2, currentMouseState.Y * 2);
            }
        }

        public virtual bool TouchDown(int touch)
        {
            if (touch < 0)
                return MouseButtonDown;
            return touchesActive[touch];
        }

        public virtual Vector2 TouchPos(int touch)
        {
            if (touch < 0)
                return MousePos;
            return touches[touch];
        }

        public virtual Vector2 LastTouchPos(int touch)
        {
            if (touch < 0)
                return LastMousePos;
            return lastTouches[touch];
        }

    }
}
