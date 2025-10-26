using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class Spider : GameObject
    {
        public bool Active
        {
            get;
            protected set;
        }

        public bool GotCandy
        {
            get { return ropePosition < 0; }
        }

        bool dead;

        float deathRotSpeed;

        Vector2 lastPos;

        Candy attachedCandy;

        Rope rope;

        SoundFX wakeUp, gotCandy, deathSound;

        float ropePosition;

        public Spider(ContentManager content, Vector2 position)
        {
            sprite = new SpiderSprite(content);
            this.position = position;
            wakeUp = new SoundFX("spider_activate");
            gotCandy = new SoundFX("spider_win");
            deathSound = new SoundFX("spider_fall");
        }

        public void Activate(Rope r)
        {
            (sprite as SpiderSprite).Wake();
            Active = true;
            wakeUp.Play();
            rope = r;
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (dead)
            {
                if (attachedCandy == null)
                {
                    Vector2 a = state.Gravity * elapsed;
                    Vector2 delt = position - lastPos + a * elapsed;
                    lastPos = position;
                    position += delt;
                }
                else
                {
                    position = attachedCandy.Physics.position + 15 * Vector2.UnitY;
                }
                if (GotCandy)
                {
                    rotation = 0;
                }
                else
                {
                    rotation += deathRotSpeed;
                }
            }

            if (rope != null && Active && (sprite as SpiderSprite).Walking && ropePosition >= 0)
            {
                if (rope.HasBeenCut)
                {
                    if (!dead)
                    {
                        dead = true;
                        deathSound.Play();
                        (sprite as SpiderSprite).Die(false);
                        deathRotSpeed = ((float)Util.R.NextDouble() - 0.5f) / 2;
                        lastPos = position;
                        position -= 100 * Vector2.Normalize(state.Gravity) * elapsed * SingleLevel.SCALE;
                    }
                    return;
                }

                //float percentage = (float)((FCOptions.TotalTicketsOut * FCOptions.TicketValue) / (FCOptions.TotalGamesPlayed * FCOptions.GameCost + 0.1M) / FCOptions.PayoutPCT);

                float percentage = 1;

                if (CTRGame.LEVEL_TEST)
                {
                    percentage = 1;
                }

                percentage = Math.Max(0.6f, Math.Min(1.2f, percentage));

                ropePosition += 45 * SingleLevel.SCALE * elapsed * percentage;

                float checkingPos = 0;

                bool reachedCandy = false;

                for (int i = 0; i < rope.DrawPts.Count; i++)
                {
                    Vector2 c1 = rope.DrawPts[i];
                    Vector2 c2 = rope.DrawPts[i + 1];

                    float len = Math.Max(2 * Rope.SegmentLength / 3, (c1 - c2).Length());

                    if (ropePosition >= checkingPos && (ropePosition < checkingPos + len || i > rope.DrawPts.Count - 3))
                    {
                        float overlay = ropePosition - checkingPos;
                        Vector2 c3 = c2 - c1;
                        c3 = c3 * overlay / len;
                        position.X = c1.X + c3.X;
                        position.Y = c1.Y + c3.Y;

                        if (i > rope.DrawPts.Count - 3)
                        {
                            reachedCandy = true;
                        }

                        rotation = (float)(Math.Atan2(c3.Y, c3.X) + 3 * Math.PI / 2);

                        break;
                    }
                    else
                    {
                        checkingPos += len;
                    }

                }

                if (reachedCandy)
                {
                    ropePosition = -1;
                    gotCandy.Play();
                    dead = true;
                    (sprite as SpiderSprite).Die(true);
                }

            }
        }

        internal void GiveCandy(Candy c, GlobalState s)
        {
            attachedCandy = c;
            c.Physics.position -= 100 * Vector2.Normalize(s.Gravity) * 0.016f * SingleLevel.SCALE;
        }
    }
}
