using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class HowToPopup : GameObject
    {
        public enum Obstacle { Bubble, AutoGrab, Spikes, Pump, Spider, Timer };

        TextSprite font;

        Obstacle obstacle;
        TutorialGraphicSprite tgs1, tgs2;
        BubbleSprite bubbleSprite;
        Grab grab;
        SpikeSprite spike;
        PumpSprite pump;
        SpiderSprite spider;
        StarSprite star;
        float age;
		bool bg;
		
		public Obstacle Ob
		{
			get
			{
				return obstacle;
			}
		}
		
        public bool Dead
        {
            get { return age < 0; }
        }

        Vector2 slide;

        public HowToPopup(ContentManager content, Obstacle o)
         :this (content, o, true){}
		
        public HowToPopup(ContentManager content, Obstacle o, bool bg)
        {		
            sprite = new PopupFrameSprite(content);
            obstacle = o;
            age = 4f;
            font = new TextSprite(content, false, 0.6f);
            slide = Vector2.Zero;
            rotation = 0;
			this.bg = bg;
            switch (o)
            {
                case Obstacle.Bubble:
                    position = new Vector2(250, 540);
                    bubbleSprite = new BubbleSprite(content);
                    tgs1 = new TutorialGraphicSprite(content, TutorialGraphicSprite.Sign.Pop);
                    tgs2 = new TutorialGraphicSprite(content, TutorialGraphicSprite.Sign.Finger);
                    break;
                case Obstacle.AutoGrab:
                    position = new Vector2(250, 1100);
                    grab = new Grab(content, null, new Vector2(250, 1100), -1, false, 0, 150, -1, false, false, null);
                    break;
                case Obstacle.Spikes:
                    position = new Vector2(250, 1660);
                    spike = new SpikeSprite(content, 1, false, false);
                    tgs1 = new TutorialGraphicSprite(content, TutorialGraphicSprite.Sign.Warning);
                    break;
                case Obstacle.Pump:
                    position = new Vector2(830, 540);
                    pump = new PumpSprite(content);
                    tgs1 = new TutorialGraphicSprite(content, TutorialGraphicSprite.Sign.Finger);
                    break;
                case Obstacle.Spider:
                    position = new Vector2(830, 1100);
                    spider = new SpiderSprite(content);
                    spider.Wake();
                    tgs1 = new TutorialGraphicSprite(content, TutorialGraphicSprite.Sign.Warning);
                    break;
                case Obstacle.Timer:
                    position = new Vector2(830, 1660);
                    star = new StarSprite(content);
                    star.SetLife(0.75f);
                    break;
                default:
                    break;
            }
        }
		
		public void Reset()
		{
			age = 5;
		}

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);

            age -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			
			if(state.Input.MouseJustClicked())
			{
				age /= 2f;
			}

            if (age < 1 && bg)
            {
                slide = new Vector2(450, 0) * (1 - age);
            }			
			else if(age > 4)
			{
                slide = new Vector2(450, 0) * (age - 4);
			}
			else
			{
				slide = Vector2.Zero;
			}

            switch (obstacle)
            {
                case Obstacle.Bubble:
                    bubbleSprite.Update(gameTime);
                    tgs1.Update(gameTime);
                    tgs2.Update(gameTime);
                    slide.X *= -1;
                    break;
                case Obstacle.AutoGrab:
                    slide.X *= -1;
                    break;
                case Obstacle.Spikes:
                    spike.Update(gameTime);
                    tgs1.Update(gameTime);
                    slide.X *= -1;
                    break;
                case Obstacle.Pump:
                    pump.Update(gameTime);
                    tgs1.Update(gameTime);
                    break;
                case Obstacle.Spider:
                    spider.Update(gameTime);
                    tgs1.Update(gameTime);
                    break;
                case Obstacle.Timer:
                    star.Update(gameTime);
                    star.SetLife(age / 5f);
                    break;
                default:
                    break;
            }
        }

        public override void Draw(SpriteBatch sb, Vector2 cameraPosition)
        {
			if(bg)
			{
            	base.Draw(sb, cameraPosition - slide);
			}

            switch (obstacle)
            {
                case Obstacle.Bubble:
                    bubbleSprite.Draw(sb, position + 60 * Vector2.UnitY + slide, 0);
                    tgs1.Draw(sb, new Vector2(400, 240) + slide, 0);
                    tgs2.Draw(sb, new Vector2(50, 690) + slide, 0);
                    font.Draw(sb, "The bubble will lift the candy up.", new Vector2(50, 660) + slide);
                    font.Draw(sb, "Pop the bubble with your finger.", new Vector2(50, 280) + slide);
                    break;
                case Obstacle.AutoGrab:
                    grab.Draw(sb, cameraPosition - slide);
                    font.Draw(sb, "Automatic ropes appear when\ncandy gets into their area.", new Vector2(50, 855) + slide);
                    break;
                case Obstacle.Spikes:
                    spike.Draw(sb, position + slide, 0);
                    tgs1.Draw(sb, new Vector2(50, 1900) + slide, 0);
                    font.Draw(sb, "Keep the candy away from spikes.", new Vector2(50, 1410) + slide);
                    break;
                case Obstacle.Pump:
                    pump.Draw(sb, position + slide, 0);
                    tgs1.Draw(sb, new Vector2(650, 800) + slide, 0);
                    font.Draw(sb, "Tap the Air Cushion to blow candy.", new Vector2(620, 290) + slide);
                    break;
                case Obstacle.Spider:
                    spider.Draw(sb, position + slide, 0);
                    tgs1.Draw(sb, new Vector2(660, 1350) + slide, 0);
                    font.Draw(sb, "Cut the rope before the spider\nreaches the candy.", new Vector2(620, 855) + slide);
                    break;
                case Obstacle.Timer:
					if(age > 0)
					{
						star.Draw(sb, position + slide, 0);
					}
                    font.Draw(sb, "Some stars require fast actions\nto be collected.", new Vector2(620, 1410) + slide);
                    break;
                default:
                    break;
            }
        }
    }
}
