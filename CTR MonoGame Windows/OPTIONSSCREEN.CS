using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Tao.Sdl;

namespace CTR_MonoGame
{
    class OptionsScreen
    {
        const float ROW_HEIGHT = 35, WIDTH = 340, INC_WIDTH = 100;
        protected Texture2D shade;

        TextSprite titleFont, littleFont, tinyFont;
        GiantNumberTextSprite bigFont;
        public bool Done
        {
            get
            {
                return exitButton.Pressed;
            }
        }
        GlobalState state;
        ContentManager content;
        Input input;
        List<MenuNode> menu;
        ExitAuditButton exitButton;
        float holdTimer;
        int activeCategory;
        bool touchTest, confirmReset, baseCurrencyChanged;
        Texture2D payoutChart;
        YesNoButton yes, no;

        public OptionsScreen(ContentManager content, Input input)
        {

            titleFont = new TextSprite(content, true, 1.5f);
            bigFont = new GiantNumberTextSprite(content);
            littleFont = new TextSprite(content, false);
            tinyFont = new TextSprite(content, false, 0.5f);

            shade = content.Load<Texture2D>("menu_drawings_bigpage_markers_hd");


            state = new GlobalState();
            state.Candy = new Candy(content, Vector2.Zero, false, false);
            state.Input = input;
            state.Camera = new Camera(new Point(1, 1), state);

            this.content = content;
            this.input = input;

            payoutChart = content.Load<Texture2D>("payoutDisplay");

            activeCategory = -1;

            yes = new YesNoButton(content, new Vector2(340, 1200), true);
            no = new YesNoButton(content, new Vector2(740, 1200), false);

            BuildMenu();

            exitButton = new ExitAuditButton(content, new Vector2(222, 1804), ExitAuditButton.ButtonType.Exit);
        }

        private void BuildMenu()
        {
            menu = new List<MenuNode>();
            string ticketString = FCOptions.TicketName + "s";

            menu.Add(new SectionNode("Audits"));
            if (activeCategory == 0)
            {

                if (FCOptions.UseTickets && !FCOptions.FixedTickets)
                {
                    if (FCOptions.FreePlay)
                    {
                        menu.Add(new InfoNode("Target Payout", ticketString, (FCOptions.PayoutPCT * 100).ToString("###.")));
                        string payout = "Not Enough Data";
                        try
                        {
                            payout = ((FCOptions.TotalTicketsOut * (decimal)FCOptions.TicketMultiplier) / FCOptions.TotalGamesPlayed).ToString("###0.##");
                        }
                        catch (Exception)
                        { }
                        menu.Add(new InfoNode("Actual Payout", ticketString, payout));
                    }
                    else
                    {
                        menu.Add(new InfoNode("Target Payout", "%", (FCOptions.PayoutPCT * 100).ToString("###.")));
                        string payout = "Not Enough Data";
                        try
                        {
                            payout = ((FCOptions.TotalTicketsOut * FCOptions.TicketValue * FCOptions.TicketMultiplier) / (FCOptions.TotalGamesPlayed * FCOptions.GameCost) * 100).ToString("###0.##");
                        }
                        catch (Exception)
                        { }
                        menu.Add(new InfoNode("Actual Payout", "%", payout));
                    }
                }
                menu.Add(new InfoNode("Games Played", "", FCOptions.TotalGamesPlayed.ToString()));
                string timePerGame = "00:00";
                try
                {
                    float time = (float)(FCOptions.TimePerLevel.Sum() / FCOptions.TotalGamesPlayed);
                    if (!float.IsNaN(time))
                    {
                        timePerGame = FormatTime(time);
                    }
                }
                catch (Exception)
                { }
                menu.Add(new InfoNode("Average Time per Game", "M:S", timePerGame));
                menu.Add(new InfoNode("Pulses In", "", FCOptions.PulsesIn.ToString()));
                menu.Add(new InfoNode("Value In", GetCurrencyUnit(), (FCOptions.TotalGamesPlayed * FCOptions.GameCost).ToString("###########0.00")));
                if (FCOptions.UseTickets)
                {
                    menu.Add(new InfoNode("Physical " + ticketString + " Out", "", FCOptions.TotalTicketsOut.ToString()));
                    if (!FCOptions.FreePlay)
                    {
                        menu.Add(new InfoNode("Value Out", GetCurrencyUnit(), (FCOptions.TotalTicketsOut * (ulong)FCOptions.TicketMultiplier * FCOptions.TicketValue).ToString("####0.00")));
                    }
                    if (!FCOptions.FixedTickets)
                    {
                        menu.Add(new InfoNode("Average Physical " + ticketString + " per Game", "", FCOptions.TotalGamesPlayed > 0 ? (FCOptions.TotalTicketsOut / (ulong)FCOptions.TotalGamesPlayed).ToString("###0") : "0"));
                        menu.Add(new InfoNode("Bonuses Won", "", (FCOptions.SmallBonusCount + FCOptions.MediumBonusCount + FCOptions.BigBonusCount).ToString()));
                    }
                }
            }
            menu.Add(new SectionNode("Audio"));
            if (activeCategory == 1)
            {
                menu.Add(new ValueNode<int>("In Game", "%", FCOptions.GameVolume, () => { return FCOptions.GameVolume.ToString(); }, (i) => { FCOptions.GameVolume = i; SetVolume(i); }, Enumerable.Range(0, 101).ToArray()));
                menu.Add(new ValueNode<int>("Attract", "%", FCOptions.AttractVolume, () => { return FCOptions.AttractVolume.ToString(); }, (i) => { FCOptions.AttractVolume = i; SetVolume(i); }, Enumerable.Range(0, 101).ToArray()));
                menu.Add(new ValueNode<AttractMusicFrequency>("Attract Music Frequency", FCOptions.AttractMusicFrequency == AttractMusicFrequency.Always || FCOptions.AttractMusicFrequency == AttractMusicFrequency.Off ? "" : "Minutes", FCOptions.AttractMusicFrequency, () => { return FCOptions.AttractMusicFrequency.ToString(); }, (i) => { FCOptions.AttractMusicFrequency = i; BuildMenu(); }, (AttractMusicFrequency[])Enum.GetValues(typeof(AttractMusicFrequency))));
            }
            menu.Add(new SectionNode("Cost"));
            if (activeCategory == 2)
            {
                menu.Add(new ValueNode<bool>("Freeplay", "", FCOptions.FreePlay, () => { return FCOptions.FreePlay ? "YES" : "NO"; }, (i) => { FCOptions.FreePlay = i; BuildMenu(); }, true, false));
                if (!FCOptions.FreePlay)
                {
                    menu.Add(new ValueNode<decimal>("Game Cost", GetCurrencyUnit(), FCOptions.GameCost, () => { return FCOptions.GameCost.ToString("#.00"); }, (i) => { FCOptions.GameCost = i; }, Enumerable.Range(1, 99).Select(x => x * FCOptions.Coin1).ToArray()));
                    menu.Add(new ValueNode<Currency>("Currency", "", true, FCOptions.Currency, () => { return FCOptions.Currency.ToString(); }, (i) => { FCOptions.Currency = i; ResetCurrencyDefaults(); BuildMenu(); }, (Currency[])Enum.GetValues(typeof(Currency))));
                    menu.Add(new ValueNode<bool>("Swipe Card", "", true, FCOptions.SwipeCard, () => { return FCOptions.SwipeCard ? "YES" : "NO"; }, (i) => { FCOptions.SwipeCard = i; BuildMenu(); }, true, false));
                    if (FCOptions.SwipeCard)
                    {
                        menu.Add(new ValueNode<decimal>("Coin Line 1", "", FCOptions.Coin1, () => { return FCOptions.Coin1.ToString("######0.00"); }, (i) => { FCOptions.Coin1 = i; }, Enumerable.Range(1, 100).Select(x => x * FCOptions.CurrencyUnitValue).ToArray()));
                    }
                    else
                    {
                        menu.Add(new ValueNode<decimal>("Currency Unit Value", "", FCOptions.CurrencyUnitValue, () => { return FCOptions.CurrencyUnitValue.ToString("######0.00"); }, (i) => { FCOptions.CurrencyUnitValue = i; baseCurrencyChanged = true; }, Enumerable.Range(1, 100).Select(x => x * GetCurrencyResolution()).ToArray()));
                        menu.Add(new ValueNode<decimal>("Coin Line 1", "", FCOptions.Coin1, () => { return FCOptions.Coin1.ToString("######0.00"); }, (i) => { FCOptions.Coin1 = i; }, Enumerable.Range(1, 100).Select(x => x * FCOptions.CurrencyUnitValue).ToArray()));
                        menu.Add(new ValueNode<decimal>("Coin Line 2", "", FCOptions.Coin2, () => { return FCOptions.Coin2.ToString("######0.00"); }, (i) => { FCOptions.Coin2 = i; }, Enumerable.Range(1, 100).Select(x => x * FCOptions.CurrencyUnitValue).ToArray()));
                        menu.Add(new ValueNode<decimal>("Coin Line 3 / DBV", "", FCOptions.DBV, () => { return FCOptions.DBV.ToString("######0.00"); }, (i) => { FCOptions.DBV = i; }, Enumerable.Range(1, 100).Select(x => x * FCOptions.CurrencyUnitValue).ToArray()));
                    }
                }
            }
            menu.Add(new SectionNode("Tickets"));
            if (activeCategory == 3)
            {
                menu.Add(new ValueNode<bool>("Use " + ticketString, "", FCOptions.UseTickets, () => { return FCOptions.UseTickets ? "YES" : "NO"; }, (i) => { FCOptions.UseTickets = i; BuildMenu(); }, true, false));
                if (FCOptions.UseTickets)
                {
                    menu.Add(new ValueNode<bool>("Fixed " + ticketString, "", FCOptions.FixedTickets, () => { return FCOptions.FixedTickets ? "YES" : "NO"; }, (i) => { FCOptions.FixedTickets = i; BuildMenu(); }, true, false));
                    if (FCOptions.FixedTickets)
                    {
                        menu.Add(new ValueNode<int>("Physical " + ticketString + " Per Game", "", FCOptions.FixedTicketsPerGame, () => { return FCOptions.FixedTicketsPerGame.ToString(); }, (i) => { FCOptions.FixedTicketsPerGame = i; }, Enumerable.Range(1, 100).ToArray()));
                        menu.Add(new ValueNode<decimal>("Physical " + FCOptions.TicketName + " Value", "", FCOptions.TicketValue * FCOptions.TicketMultiplier, () => { return (FCOptions.TicketValue * FCOptions.TicketMultiplier).ToString("0.00##"); }, (i) => { FCOptions.TicketValue = i / FCOptions.TicketMultiplier; }, Enumerable.Range(1, 10000).Select(x => x * 0.0001M).ToArray()));
                        menu.Add(new ValueNode<int>("Ticket Multiplier", "X", FCOptions.TicketMultiplier, () => { return FCOptions.TicketMultiplier.ToString(); }, (i) => { FCOptions.TicketMultiplier = i; BuildMenu(); }, 1, 2));
                        menu.Add(new ValueNode<bool>("Instant Payout", "", true, FCOptions.InstantPayout, () => { return FCOptions.InstantPayout ? "YES" : "NO"; }, (i) => { FCOptions.InstantPayout = i; }, true, false));
                        menu.Add(new ValueNode<TicketName>("Description", "", true, FCOptions.TicketName, () => { return FCOptions.TicketName.ToString(); }, (i) => { FCOptions.TicketName = i; }, (TicketName[])Enum.GetValues(typeof(TicketName))));
                    }
                    else
                    {
                        menu.Add(new ValueNode<decimal>("Physical " + FCOptions.TicketName + " Value", "", FCOptions.TicketValue * FCOptions.TicketMultiplier, () => { return (FCOptions.TicketValue * FCOptions.TicketMultiplier).ToString("0.00##"); }, (i) => { FCOptions.TicketValue = i / FCOptions.TicketMultiplier; }, Enumerable.Range(1, 10000).Select(x => x * 0.0001M).ToArray()));
                        menu.Add(new ValueNode<int>("Ticket Multiplier", "X", FCOptions.TicketMultiplier, () => { return FCOptions.TicketMultiplier.ToString(); }, (i) => { FCOptions.TicketMultiplier = i; BuildMenu(); }, 1, 2));
                        menu.Add(new ValueNode<int>("Mercy Tickets", "", FCOptions.MercyTickets, () => { return FCOptions.MercyTickets.ToString(); }, (i) => { FCOptions.MercyTickets = i; }, FCOptions.TicketMultiplier == 2 ? new int[] { 0, 2, 4 } : new int[] { 0, 1, 2, 3, 4 }));
                        menu.Add(new ValueNode<bool>("Instant Payout", "", true, FCOptions.InstantPayout, () => { return FCOptions.InstantPayout ? "YES" : "NO"; }, (i) => { FCOptions.InstantPayout = i; }, true, false));
                        menu.Add(new ValueNode<TicketName>("Description", "", true, FCOptions.TicketName, () => { return FCOptions.TicketName.ToString(); }, (i) => { FCOptions.TicketName = i; }, (TicketName[])Enum.GetValues(typeof(TicketName))));
                    }
                }
            }
            menu.Add(new SectionNode("Game"));
            if (activeCategory == 4)
            {
                if (!FCOptions.FixedTickets && FCOptions.UseTickets)
                {
                    if (FCOptions.FreePlay)
                    {
                        menu.Add(new ValueNode<decimal>("Payout Target", ticketString, FCOptions.PayoutPCT, () => { return (FCOptions.PayoutPCT * 100).ToString("###."); }, (i) => { FCOptions.PayoutPCT = i; OptimizePayout(); }, Enumerable.Range(1, 100).Select(x => x * 0.01M).ToArray()));
                    }
                    else
                    {
                        menu.Add(new ValueNode<decimal>("Payout Target", "%", FCOptions.PayoutPCT, () => { return (FCOptions.PayoutPCT * 100).ToString("###."); }, (i) => { FCOptions.PayoutPCT = i; OptimizePayout(); }, Enumerable.Range(1, 100).Select(x => x * 0.01M).ToArray()));
                    }
                }
                menu.Add(new ValueNode<string>("Bonus Values", FCOptions.UseTickets && !FCOptions.FixedTickets ? ticketString : "", FCOptions.BigBonusValue + "-" + FCOptions.MediumBonusValue + "-" + FCOptions.SmallBonusValue, () => { return FCOptions.BigBonusValue + "-" + FCOptions.MediumBonusValue + "-" + FCOptions.SmallBonusValue; }, s =>
                    {
                        string[] vals = s.Split('-');
                        FCOptions.BigBonusValue = int.Parse(vals[0]);
                        FCOptions.MediumBonusValue = int.Parse(vals[1]);
                        FCOptions.SmallBonusValue = int.Parse(vals[2]);
                        OptimizePayout();
                    }, FCOptions.TicketMultiplier == 2 ?
                    new string[] { "1000-200-100", "1000-100-50", "1000-20-10", "500-100-50", "500-20-10", "250-100-50", "250-20-10", "200-50-10", "200-20-10", "100-20-10", "50-20-10", "20-10-2" }.Reverse().ToArray() :
                    new string[] { "1000-200-100", "1000-100-50", "1000-20-10", "500-100-50", "500-20-10", "250-100-50", "250-20-10", "200-50-10", "200-20-10", "100-20-10", "50-20-10", "50-10-5", "25-10-5", "20-10-5", "20-10-2", "10-5-2" }.Reverse().ToArray()));
                menu.Add(new ValueNode<int>("Max Credits Allowed", "", FCOptions.MaxCreditsAllowed, () => { return FCOptions.MaxCreditsAllowed < 0 ? "Off" : FCOptions.MaxCreditsAllowed.ToString(); }, (i) => { FCOptions.MaxCreditsAllowed = i; }, new int[] { -1 }.Concat(Enumerable.Range(1, 99)).ToArray()));
                if (!FCOptions.FixedTickets && FCOptions.UseTickets)
                {
                    menu.Add(new EstimatedPayoutChartNode());
                }
            }
            menu.Add(new SectionNode("Testing"));
            if (activeCategory == 5)
            {
                menu.Add(new TestNode("Coin Meter", "", "Pulse", () => { input.IOBoard.AddCoinCounter(1); }));
                menu.Add(new TestNode("Ticket Meter", "", "Pulse", () => { input.IOBoard.AddTicketCounter(1); }));
                menu.Add(new TestNode("Ticket Dispenser", "", "Pulse", () => { input.IOBoard.GiveTickets(1); }));
                menu.Add(new TestNode("Sound", "", "Playing...", () => { CTRGame.PlayMenuMusic(); }, () => { return SdlMixer.Mix_PlayingMusic() > 0 ? "Playing..." : ""; }));
                menu.Add(new TestNode("Touch Panel", "", "", () => { touchTest = true; }));
            }
            menu.Add(new SectionNode("Resets"));
            if (activeCategory == 6)
            {
                menu.Add(new ResetNode("Credits", "In", true, () => { return FCOptions.Credits.ToString("####0.00"); }, () => { FCOptions.Credits = 0; }));
                menu.Add(new ResetNode("Physical Tickets", "Owed", true, () => { return FCOptions.TicketsOwed.ToString(); }, () => { input.IOBoard.ClearTicketsToGive(); }));
                menu.Add(new ResetNode("Audits", "", true, () => { return ""; }, () =>
                {
                    FCOptions.SuspendWrites = true;
                    FCOptions.TotalGamesPlayed = 0;
                    FCOptions.PlaysPerLevel = new int[CTRGame.MAX_LOADABLE_LEVEL];
                    FCOptions.TimePerLevel = new double[CTRGame.MAX_LOADABLE_LEVEL];
                    FCOptions.FastestPerfectTimePerLevel = new double[CTRGame.MAX_LOADABLE_LEVEL];
                    FCOptions.StarsPerLevel = new int[CTRGame.MAX_LOADABLE_LEVEL];
                    FCOptions.DeathsPerLevel = new int[CTRGame.MAX_LOADABLE_LEVEL];
                    FCOptions.PointsPerLevel = new long[CTRGame.MAX_LOADABLE_LEVEL];
                    FCOptions.PerfectPlaysPerLevel = new int[CTRGame.MAX_LOADABLE_LEVEL];
                    FCOptions.RepeatGames = 0;
                    FCOptions.ThreeFedOmNoms = 0;
                    FCOptions.SmallBonusCount = 0;
                    FCOptions.MediumBonusCount = 0;
                    FCOptions.BigBonusCount = 0;
                    FCOptions.PulsesIn = 0;
                    FCOptions.TotalTicketsOut = 0;
                    FCOptions.SuspendWrites = false;
                }));
                menu.Add(new MenuNode(""));
                menu.Add(new ResetNode("Factory Settings", "", false, () => { return ""; }, () => { confirmReset = true; }));
            }


            float yPos = 207;
            for (int i = 0; i < menu.Count; i++)
            {
                if (menu[i] is ValueNode)
                {
                    (menu[i] as ValueNode).BuildButtons(content, yPos + 67);
                }
                else if (menu[i] is ResetNode)
                {
                    (menu[i] as ResetNode).BuildButtons(content, yPos + 67);
                }
                else if (menu[i] is SectionNode)
                {
                    (menu[i] as SectionNode).BuildButtons(content, yPos + 67);
                }
                yPos += menu[i].Height;
            }
        }

        private decimal GetCurrencyResolution()
        {
            switch (FCOptions.Currency)
            {
                case Currency.Dollar:
                case Currency.Euro:
                case Currency.Pound:
                case Currency.Guilder:
                case Currency.Franc:
                case Currency.Coin:
                case Currency.Token:
                default:
                    return 0.01M;
                case Currency.Yen:
                case Currency.Won:
                case Currency.Krona:
                    return 1;
                case Currency.Ruble:
                    return 0.5M;
                case Currency.Real:
                case Currency.Rand:
                    return 0.05M;
                case Currency.Peso:
                    return 0.1M;
            }
        }

        private void ResetCurrencyDefaults()
        {
            FCOptions.GameCost = GetCostDefault();
        }

        private decimal GetCostDefault()
        {
            switch (FCOptions.Currency)
            {
                case Currency.Dollar:
                case Currency.Euro:
                case Currency.Pound:
                case Currency.Real:
                case Currency.Guilder:
                case Currency.Franc:
                case Currency.Coin:
                case Currency.Token:
                default:
                    return 1;
                case Currency.Yen:
                    return 100;
                case Currency.Won:
                    return 1000;
                case Currency.Ruble:
                    return 30;
                case Currency.Peso:
                case Currency.Rand:
                    return 10;
                case Currency.Krona:
                    return 5;
            }

        }

        private string GetCurrencyUnit()
        {
            switch (FCOptions.Currency)
            {
                case Currency.Dollar:
                default:
                    return "$";
                case Currency.Euro:
                    return "€";
                case Currency.Pound:
                    return "£";
                case Currency.Yen:
                    return "¥";
                case Currency.Won:
                    return "₩";
                case Currency.Ruble:
                    return "руб";
                case Currency.Real:
                    return "R$";
                case Currency.Peso:
                    return "$";
                case Currency.Rand:
                    return "R";
                case Currency.Krona:
                    return "kr";
                case Currency.Guilder:
                    return "ƒ";
                case Currency.Franc:
                    return "Fr";
                case Currency.Coin:
                    return "";
                case Currency.Token:
                    return "";
            }
        }

        private void SetVolume(int i)
        {
            SdlMixer.Mix_Volume(-1, (int)(1.28f * i));
            SdlMixer.Mix_VolumeMusic((int)(1.28f * i));
        }

        public virtual void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            int section = 0;
            if (touchTest)
            {
                if (input.KeyJustPressed(Keys.Q) || input.IOBoard.Diag)
                {
                    touchTest = false;
                }
            }
            else
            {
                bool buttonHeld = false;
                for (int i = 0; i < menu.Count; i++)
                {
                    if (menu[i] is ValueNode)
                    {
                        ValueNode vn = menu[i] as ValueNode;
                        vn.IncrementButton.Update(gameTime, state);
                        if (vn.IncrementButton.Touched)
                        {
                            vn.Increment();
                            holdTimer = 1.5f;
                            buttonHeld = true;
                        }
                        else if (vn.IncrementButton.Held)
                        {
                            buttonHeld = true;
                            if (holdTimer > 0)
                            {
                                holdTimer -= delta;
                            }
                            else
                            {
                                holdTimer += 0.1f;
                                vn.Increment();
                            }
                        }
                        vn.DecrementButton.Update(gameTime, state);
                        if (vn.DecrementButton.Touched)
                        {
                            vn.Decrement();
                            holdTimer = 1.5f;
                            buttonHeld = true;
                        }
                        else if (vn.DecrementButton.Held)
                        {
                            buttonHeld = true;
                            if (holdTimer > 0)
                            {
                                holdTimer -= delta;
                            }
                            else
                            {
                                holdTimer += 0.1f;
                                vn.Decrement();
                            }
                        }
                    }
                    else if (menu[i] is ResetNode)
                    {
                        ResetNode vn = menu[i] as ResetNode;
                        vn.Button.Update(gameTime, state);
                        if (vn.Button.Pressed)
                        {
                            vn.Reset();
                        }
                    }
                    else if (menu[i] is SectionNode)
                    {
                        SectionNode sn = menu[i] as SectionNode;
                        sn.Button.Update(gameTime, state);
                        if (sn.Button.Pressed)
                        {
                            if (activeCategory == section)
                            {
                                activeCategory = -1;
                            }
                            else
                            {
                                activeCategory = section;
                            }
                            if (activeCategory != 6)
                            {
                                confirmReset = false;
                            }
                            BuildMenu();
                        }
                        (sn.Button as BluePlusMinusButton).Toggle(activeCategory == section);
                        section++;
                    }
                    if (confirmReset)
                    {
                        yes.Update(gameTime, state);
                        if (yes.Pressed)
                        {
                            FCOptions.RebuildOptions();
                            confirmReset = false;
                        }
                        no.Update(gameTime, state);
                        if (no.Pressed)
                        {
                            confirmReset = false;
                        }
                    }
                }
                if (!buttonHeld && baseCurrencyChanged)
                {
                    baseCurrencyChanged = false;
                    BuildMenu();
                    BuildMenu();
                }
            }
            exitButton.Update(gameTime, state);
        }

        private static void OptimizePayout()
        {
            int[] pptValues = new int[] { 50, 60, 75, 90, 100, 150, 180, 200, 250, 300, 400, 500, 600, 750, 900, 1200, 2000, 3000, 4500 };
            int targetValue = -1;
            decimal distanceFromTargetPayout = decimal.MaxValue;
            for (int i = 0; i < pptValues.Length; i++)
            {
                long ticketsOut = AverageBonusTicketsPerGame() + AverageNonBonusTicketsPerGame(pptValues[i]);
                decimal payoutPCT = ticketsOut * FCOptions.TicketValue / FCOptions.GameCost;
                if (FCOptions.FreePlay)
                {
                    payoutPCT = ticketsOut / 100M;
                }
                if (distanceFromTargetPayout > Math.Abs(payoutPCT - FCOptions.PayoutPCT))
                {
                    distanceFromTargetPayout = Math.Abs(payoutPCT - FCOptions.PayoutPCT);
                    targetValue = i;
                }
            }
            FCOptions.PointsPerTicket = pptValues[targetValue];
        }


        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            if (touchTest)
            {
                titleFont.Draw(spriteBatch, "Touch Panel Diagnostic", new Vector2(540, 13), TextSprite.Alignment.Center);
                littleFont.SetScale(1);
                littleFont.Draw(spriteBatch, "press Service button to exit", new Vector2(540, 1700), TextSprite.Alignment.Center);
                for (int i = 0; i < 5; i++)
                {
                    if (input.TouchDown(i))
                    {
                        spriteBatch.Draw(shade, input.TouchPos(i) - Vector2.One * 15, null, Color.Green, 0, Vector2.Zero, 30, SpriteEffects.None, 1);
                    }
                }
            }
            else
            {
                titleFont.Draw(spriteBatch, "Operator Menu", new Vector2(540, 13), TextSprite.Alignment.Center);
                littleFont.SetScale(1.5f);
                littleFont.Draw(spriteBatch, "Version   1.4", new Vector2(540, 100), TextSprite.Alignment.Center);

                float yPos = 207;
                for (int i = 0; i < menu.Count; i++)
                {
                    if (menu[i] is ValueNode)
                    {
                        ValueNode vn = menu[i] as ValueNode;
                        vn.IncrementButton.Draw(spriteBatch, Vector2.Zero);
                        vn.DecrementButton.Draw(spriteBatch, Vector2.Zero);
                        littleFont.SetScale(0.8f);
                        littleFont.Draw(spriteBatch, vn.Name, new Vector2(300, yPos + 15));
                        littleFont.Draw(spriteBatch, vn.Value, new Vector2(870, yPos + 15), TextSprite.Alignment.Right);
                        littleFont.Draw(spriteBatch, vn.Units, new Vector2(880, yPos + 15));
                    }
                    else if (menu[i] is InfoNode)
                    {
                        InfoNode vn = menu[i] as InfoNode;
                        littleFont.SetScale(0.8f);
                        littleFont.Draw(spriteBatch, vn.Name, new Vector2(100, yPos + 15));
                        littleFont.Draw(spriteBatch, vn.Value, new Vector2(870, yPos + 15), TextSprite.Alignment.Right);
                        littleFont.Draw(spriteBatch, vn.Units, new Vector2(880, yPos + 15));
                    }
                    else if (menu[i] is ResetNode)
                    {
                        ResetNode vn = menu[i] as ResetNode;
                        vn.Button.Draw(spriteBatch, Vector2.Zero);
                        littleFont.SetScale(0.8f);
                        littleFont.Draw(spriteBatch, vn.Name, new Vector2(300, yPos + 15));
                        littleFont.Draw(spriteBatch, vn.Value, new Vector2(870, yPos + 15), TextSprite.Alignment.Right);
                        littleFont.Draw(spriteBatch, vn.Units, new Vector2(880, yPos + 15));
                    }
                    else if (menu[i] is SectionNode)
                    {
                        SectionNode sn = menu[i] as SectionNode;
                        sn.Button.Draw(spriteBatch, Vector2.Zero);
                        littleFont.SetScale(1.5f);
                        littleFont.Draw(spriteBatch, sn.Name, new Vector2(140, yPos - 30));
                    }
                    else if (menu[i] is EstimatedPayoutChartNode)
                    {
                        spriteBatch.Draw(payoutChart, new Vector2(50, yPos + 30), Color.White);
                        littleFont.SetScale(1f);
                        littleFont.Draw(spriteBatch, FCOptions.MercyTickets.ToString(), new Vector2(155, yPos + 130), TextSprite.Alignment.Center);
                        littleFont.Draw(spriteBatch, (FCOptions.MercyTickets + 18000 / FCOptions.PointsPerTicket + FCOptions.BigBonusValue).ToString(), new Vector2(960, yPos + 130), TextSprite.Alignment.Center);
                        littleFont.SetScale(2f);
                        littleFont.Draw(spriteBatch, AverageNonBonusTicketsPerGame().ToString("####0."), new Vector2(390, yPos + 340), TextSprite.Alignment.Center);
                        littleFont.Draw(spriteBatch, AverageBonusTicketsPerGame().ToString("####0."), new Vector2(740, yPos + 340), TextSprite.Alignment.Center);
                        bigFont.SetScale(0.75f);
                        bigFont.Draw(spriteBatch, (AverageBonusTicketsPerGame() + AverageNonBonusTicketsPerGame()).ToString("####0."), new Vector2(565, yPos + 40), TextSprite.Alignment.Center);
                    }
                    yPos += menu[i].Height;
                }
                if (confirmReset)
                {
                    littleFont.SetScale(1.4f);
                    littleFont.Draw(spriteBatch, "Do you wish to restore?", new Vector2(540, 1000), TextSprite.Alignment.Center);
                    yes.Draw(spriteBatch, Vector2.Zero);
                    no.Draw(spriteBatch, Vector2.Zero);
                }
                exitButton.Draw(spriteBatch, Vector2.Zero);
            }
            spriteBatch.End();
        }

        private static long AverageNonBonusTicketsPerGame()
        {
            return AverageNonBonusTicketsPerGame(FCOptions.PointsPerTicket);
        }

        private static long AverageNonBonusTicketsPerGame(int ppt)
        {
            return ((168000L + FCOptions.PointsPerLevel.Sum()) / ppt) / ((FCOptions.TotalGamesPlayed + 15));
        }

        private static long AverageBonusTicketsPerGame()
        {
            return (FCOptions.BigBonusValue * (FCOptions.BigBonusCount + 1) + FCOptions.MediumBonusValue * (FCOptions.MediumBonusCount + 2) + FCOptions.SmallBonusValue * (FCOptions.SmallBonusCount + 2)) / ((15 + FCOptions.TotalGamesPlayed));
        }

        public static decimal CurrentPayoutPCT()
        {
            try
            {
                return (FCOptions.TotalTicketsOut * FCOptions.TicketValue * FCOptions.TicketMultiplier) / (FCOptions.TotalGamesPlayed * FCOptions.GameCost);
            }
            catch
            {
                return FCOptions.PayoutPCT;
            }
        }

        private string FormatTime(float time)
        {
            string rVal = "";
            rVal += time < 0 ? "-" : "+";
            rVal += Math.Abs(time / 60).ToString("00");
            rVal += ":";
            rVal += Math.Abs(time % 60).ToString("00");
            return rVal;
        }

        class MenuNode
        {
            public readonly string Name;

            public virtual float Height
            {
                get { return 53; }
            }

            public MenuNode(string name)
            {
                Name = name;
            }

        }

        class InfoNode : MenuNode
        {
            public readonly string Value;

            public readonly string Units;

            public InfoNode(string name, string units, string value)
                : base(name)
            {
                Units = units;
                Value = value;
            }
        }

        abstract class ValueNode : MenuNode
        {
            protected Func<string> getValue;

            public string Value
            {
                get { return getValue(); }
            }

            protected bool Wrap;

            public readonly string Units;

            public ValueNode(string name, string units, bool wrap)
                : base(name)
            {
                Units = units;
                Wrap = wrap;
            }

            public Button IncrementButton, DecrementButton;

            public void BuildButtons(ContentManager content, float yPos)
            {
                IncrementButton = new PlusMinusButton(content, new Vector2(191, yPos), true);
                DecrementButton = new PlusMinusButton(content, new Vector2(257, yPos), false);
            }

            public abstract void Increment();
            public abstract void Decrement();
        }

        class ValueNode<T> : ValueNode
        {
            Action<T> setValue;

            T[] allowedValues;
            int selectedValue;

            public ValueNode(string name, string units, T currentValue, Func<string> getValue, Action<T> setValue, params T[] values)
                : this(name, units, false, currentValue, getValue, setValue, values)
            { }

            public ValueNode(string name, string units, bool wrap, T currentValue, Func<string> getValue, Action<T> setValue, params T[] values)
                : base(name, units, wrap)
            {
                allowedValues = values;

                this.getValue = getValue;
                this.setValue = setValue;

                selectedValue = -1;
                for (int i = 0; i < allowedValues.Length; i++)
                {
                    if (allowedValues[i].Equals(currentValue))
                    {
                        selectedValue = i;
                    }
                }
                if (selectedValue < 0)
                {
                    selectedValue = 0;
                    setValue(allowedValues[selectedValue]);
                }
            }

            public override void Increment()
            {
                selectedValue++;
                if (selectedValue >= allowedValues.Length)
                {
                    if (Wrap)
                    {
                        selectedValue = 0;
                    }
                    else
                    {
                        selectedValue = allowedValues.Length - 1;
                    }
                }
                setValue(allowedValues[selectedValue]);
            }

            public override void Decrement()
            {
                selectedValue--;
                if (selectedValue < 0)
                {
                    if (Wrap)
                    {
                        selectedValue += allowedValues.Length;
                    }
                    else
                    {
                        selectedValue = 0;
                    }
                }
                setValue(allowedValues[selectedValue]);
            }
        }

        class ResetNode : MenuNode
        {
            protected bool clear, test;

            public ResetClearButton Button;

            protected Func<string> getValue;

            protected Action reset;

            public virtual string Value
            {
                get { return getValue(); }
            }

            public readonly string Units;

            public ResetNode(string name, string units, bool clear, Func<string> getValue, Action reset)
                : base(name)
            {
                this.Units = units;
                this.clear = clear;
                this.getValue = getValue;
                this.reset = reset;
                test = false;
            }

            public void Reset()
            {
                reset();
            }

            public void BuildButtons(ContentManager content, float yPos)
            {
                Button = new ResetClearButton(content, new Vector2(224, yPos), !clear, test);
            }
        }

        class EstimatedPayoutChartNode : MenuNode
        {
            public override float Height
            {
                get
                {
                    return 554;
                }
            }

            public EstimatedPayoutChartNode()
                : base("")
            {
            }
        }

        class TestNode : ResetNode
        {
            string testWord;

            public override string Value
            {
                get
                {
                    if (getValue == null)
                    {
                        return Button.Pressed ? testWord : "";
                    }
                    else
                    {
                        return getValue();
                    }
                }
            }

            public TestNode(string name, string units, string testWord, Action reset)
                : this(name, units, testWord, reset, null)
            { }

            public TestNode(string name, string units, string testWord, Action reset, Func<string> getValue)
                : base(name, units, false, null, reset)
            {
                this.getValue = getValue;
                this.testWord = testWord;
                test = true;
            }
        }

        class SectionNode : MenuNode
        {
            public override float Height
            {
                get
                {
                    return 75;
                }
            }

            public BluePlusMinusButton Button;

            public SectionNode(string name)
                : base(name)
            {

            }

            public void BuildButtons(ContentManager content, float yPos)
            {
                Button = new BluePlusMinusButton(content, new Vector2(50, yPos));
            }
        }

    }
}
