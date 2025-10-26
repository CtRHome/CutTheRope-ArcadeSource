using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CTR_MonoGame
{
    class Victory : ITransitionable
    {
        enum Mode { CountTickets, CountBonus, ShowTotal, DispenseTickets, Done };

        Mode mode;
        TextSprite font;
        GiantNumberTextSprite bigFont;
        float countTimer, modeTimer;
        int countedScore, totalScore, countedTickets, countedBonus, totalBonus, dispensedTickets;
        bool bonus;
        Input input;
        string ticketString;

        Texture2D bg, logo, greatJob, numbers, omnom, points, pointsEq, spot, shadow, tickets;

        float countDelay;

        public bool Done
        {
            get { return mode == Mode.Done || mode == Mode.DispenseTickets; }
        }

        public Victory(ContentManager content, Input input, int stars, float time)
            : this(content, input)
        {
        }

        public Victory(ContentManager content, Input input)
        {
            this.input = input;

            ticketString = FCOptions.TicketName.ToString();

            font = new TextSprite(content, true);
            bigFont = new GiantNumberTextSprite(content);

            bg = content.Load<Texture2D>("TallyScreen/BACKGRND TALLEY");
            logo = content.Load<Texture2D>("TallyScreen/CTR LOGO TALLEY");
            greatJob = content.Load<Texture2D>("TallyScreen/GREAT JOB TALLEY");
            numbers = content.Load<Texture2D>("TallyScreen/NUMBERS LG TALLEY SCREEN");
            omnom = content.Load<Texture2D>("TallyScreen/OMNOM TALLEY");
            points = content.Load<Texture2D>("TallyScreen/POINTS LG TALLEY");
            pointsEq = content.Load<Texture2D>("TallyScreen/POINTS SM");
            spot = content.Load<Texture2D>("TallyScreen/SPOT LIGHT TALLEY");
            shadow = content.Load<Texture2D>("TallyScreen/SQUARE SHADOW TALLEY");
            if (FCOptions.TicketName == TicketName.Ticket)
            {
                tickets = content.Load<Texture2D>("TallyScreen/TICKETS LG TALLEY");
            }
            else
            {
                tickets = content.Load<Texture2D>("TallyScreen/COUPON LG TALLEY");
            }

            countDelay = 0.05f;

            dispensedTickets = 0;

            modeTimer = 1;
        }

        public void AddScore(int stars, int score, bool victory, int stage)
        {
            totalScore += score;

            bonus = totalBonus > 0;
        }

        internal void AddBonus(int p)
        {
            totalBonus += p;
            bonus = totalBonus > 0;
        }

        public void SetDispensedTickets(int t)
        {
            dispensedTickets = t;
        }

        public void UpdateTransition(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (modeTimer > 0)
            {
                modeTimer -= delta;
                if (mode == Mode.ShowTotal)
                {
                    if (input.IOBoard.Coin1 || input.IOBoard.Coin2 || input.IOBoard.DBV)
                    {
                        modeTimer = 0;
                    }
                    if (input.MouseJustClicked() && FCOptions.Credits >= FCOptions.GameCost)
                    {
                        modeTimer = 0;
                    }
                }
            }
            else
            {
                switch (mode)
                {
                    case Mode.CountTickets:
                        if (FCOptions.UseTickets && !FCOptions.InstantPayout && dispensedTickets == 0)
                        {
                            dispensedTickets = totalScore / (FCOptions.PointsPerTicket * FCOptions.TicketMultiplier) + totalBonus / FCOptions.TicketMultiplier + FCOptions.MercyTickets / FCOptions.TicketMultiplier;
                            if (dispensedTickets > 0)
                            {
                                input.IOBoard.GiveTickets(dispensedTickets);
                                FCOptions.TotalTicketsOut += (ulong)(dispensedTickets);
                            }
                        }

                        countedScore = (int)Math.Min(totalScore, countedScore + totalScore / 120f);
                        countedTickets = countedScore / (FCOptions.PointsPerTicket * FCOptions.TicketMultiplier) + FCOptions.MercyTickets / FCOptions.TicketMultiplier;
                        if (FCOptions.FixedTickets)
                        {
                            countedTickets = FCOptions.FixedTicketsPerGame;
                        }
                        if (countedScore == totalScore)
                        {
                            mode = Mode.CountBonus;
                            modeTimer = 1;
                        }
                        break;
                    case Mode.CountBonus:
                        if (countTimer > 0)
                        {
                            countTimer -= delta;
                        }
                        else
                        {
                            countTimer += countDelay;
                            countedBonus++;
                            countedTickets = totalScore / (FCOptions.PointsPerTicket * FCOptions.TicketMultiplier) + countedBonus / FCOptions.TicketMultiplier + FCOptions.MercyTickets / FCOptions.TicketMultiplier;
                            if (FCOptions.FixedTickets)
                            {
                                countedTickets = FCOptions.FixedTicketsPerGame;
                            }
                            if (countedBonus >= totalBonus)
                            {
                                mode = Mode.ShowTotal;
                                if (FCOptions.UseTickets)
                                {
                                    if (countedTickets > 0 && countedTickets > dispensedTickets)
                                    {
                                        input.IOBoard.GiveTickets(countedTickets - dispensedTickets);
                                        FCOptions.TotalTicketsOut += (ulong)(countedTickets - dispensedTickets);
                                    }
                                }
                                modeTimer = 15;
                            }
                        }
                        break;
                    case Mode.ShowTotal:
                        mode = Mode.DispenseTickets;
                        break;
                    case Mode.DispenseTickets:
                        if (FCOptions.TicketsOwed <= 0 || input.IOBoard.TicketError)
                        {
                            mode = Mode.Done;
                            modeTimer = 0f;
                        }
                        break;
                    default:
                    case Mode.Done:
                        break;
                }
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));

            float sinT = (float)Math.Sin(gameTime.TotalRealTime.TotalSeconds);

            spriteBatch.Draw(bg, Vector2.Zero, Color.White);
            spriteBatch.Draw(shadow, new Vector2(540, 960), null, Color.White, (float)gameTime.TotalRealTime.TotalSeconds / 10f, Center(shadow), 1, SpriteEffects.None, 1);
            spriteBatch.Draw(omnom, Vector2.Zero, Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Matrix.CreateScale(0.5f));
            spriteBatch.Draw(spot, new Vector2(359, 1103) + new Vector2(2f * sinT, (float)Math.Cos(gameTime.TotalRealTime.TotalSeconds)) * 40f, new Color(Color.White, 0.5f));
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));

            spriteBatch.Draw(logo, Vector2.Zero, Color.White);
            spriteBatch.Draw(greatJob, new Vector2(540, 600), null, Color.White, 0, Center(greatJob), 1 + (sinT * sinT / 15f), SpriteEffects.None, 1);
            spriteBatch.Draw(points, new Vector2(208, 786), Color.White);
            font.SetScale(1.3f);
            font.Draw(spriteBatch, countedScore.ToString(), new Vector2(590, 790));
            font.SetScale(1f);
            if (FCOptions.UseTickets)
            {
                spriteBatch.Draw(tickets, new Vector2(432, 1507), Color.White);
                spriteBatch.Draw(pointsEq, new Vector2(282, 911), Color.White);
                font.Draw(spriteBatch, (FCOptions.PointsPerTicket * FCOptions.TicketMultiplier).ToString(), new Vector2(300, 910), TextSprite.Alignment.Right);
                font.Draw(spriteBatch, FCOptions.TicketMultiplier + " " + FCOptions.TicketName + (FCOptions.TicketMultiplier > 1 ? "s" : ""), new Vector2(580, 910));
            }
            if (bonus)
            {
                font.SetScale(1.3f);
                font.Draw(spriteBatch, "Bonus: " + countedBonus + " " + FCOptions.TicketName + "s", new Vector2(540, 1045), TextSprite.Alignment.Center);
            }
            if (FCOptions.UseTickets)
            {
                bigFont.Draw(spriteBatch, (countedTickets * FCOptions.TicketMultiplier).ToString(), new Vector2(660, 1273), TextSprite.Alignment.Center, Color.White, -0.2f);
            }
            spriteBatch.End();
        }

        private Vector2 Center(Texture2D tex)
        {
            return new Vector2(tex.Width, tex.Height) / 2f;
        }
    }
}
