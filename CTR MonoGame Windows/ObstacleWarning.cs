using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CTR_MonoGame
{
    class ObstacleWarning : ITransitionable
    {
        TextSprite bigFont, littleFont;

        TutorialGraphicSprite hand;
        float handPos;
        Box box;

        GlobalState state;

        HorizLogoSprite logo;
        float handAlpha;

        float initialDelay;
        List<HowToPopup> HowTo;

        public bool Done
        {
            get;
            protected set;
        }

        public ObstacleWarning(ContentManager content, Input input)
        {
            box = Box.GetBox(1);
            box.Load(content);

            bigFont = new TextSprite(content, true, 1.5f);
            littleFont = new TextSprite(content, false);

            hand = new TutorialGraphicSprite(content, TutorialGraphicSprite.Sign.Finger);

            logo = new HorizLogoSprite(content, false);
            Done = false;

            state = new GlobalState();
            state.Candy = new Candy(content, new Vector2(270, 95), false, false);
            state.Gravity = Vector2.UnitY * 784;
            state.Camera = new Camera(new Point(1, 1), state);
            state.Input = input;

            initialDelay = 0;

            HowTo = new List<HowToPopup>();
            HowTo.Add(new HowToPopup(content, HowToPopup.Obstacle.Bubble, false));
            HowTo.Add(new HowToPopup(content, HowToPopup.Obstacle.Pump, false));
            HowTo.Add(new HowToPopup(content, HowToPopup.Obstacle.AutoGrab, false));
            HowTo.Add(new HowToPopup(content, HowToPopup.Obstacle.Spider, false));
            HowTo.Add(new HowToPopup(content, HowToPopup.Obstacle.Spikes, false));
            HowTo.Add(new HowToPopup(content, HowToPopup.Obstacle.Timer, false));

            for (int i = 0; i < HowTo.Count(); i++)
            {
                HowTo[i].Reset();
            }
        }

        public void UpdateTransition(GameTime gameTime)
        {
            Update(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            handPos += delta;
            handPos %= 2;
            handAlpha = 20 * (0.25f - Math.Abs(handPos - 0.25f));
            hand.SetAlpha(Math.Min(1, handAlpha));

            initialDelay += delta;

            for (int i = 0; i < Math.Min((int)initialDelay * 2, HowTo.Count()); i++)
            {
                HowTo[i].Update(gameTime, state);
                if (HowTo[i].Dead)
                {
                    //HowTo[i].Reset();
                }
            }

            if (state.Input.MouseJustClicked() || state.Input.KeyJustPressed(Keys.Space) || initialDelay > 8)
            {
                Done = true;
            }
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            box.Background.DrawFullScreen(spriteBatch);
            logo.Draw(spriteBatch, new Vector2(540, 200), 0);
            //hand.Draw(spriteBatch, new Vector2(350 - 3 * handAlpha, 1500 - 3 * handAlpha), 0);
            for (int i = 0; i < Math.Min((int)initialDelay * 2, HowTo.Count()); i++)
            {
                HowTo[i].Draw(spriteBatch, Vector2.Zero);
            }
            bigFont.Draw(spriteBatch, "Tap the screen to play", new Vector2(540, 705), TextSprite.Alignment.Center,
                         new Color(Color.White,
                      Math.Min((float)Math.Abs(2 * Math.Sin(gameTime.TotalGameTime.TotalSeconds)), 1)));
            spriteBatch.End();
        }
    }
}
