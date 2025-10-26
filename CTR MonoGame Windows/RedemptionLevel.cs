using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CTR_MonoGame
{
    class RedemptionLevel : SingleLevel
    {
        float pumpTimer, spikeTimer;
        int nextPump, nextSpike;
        List<int> spikeGroups;
        int level;
        static int randLevel;
        float introAnimationTimer;
        TextSprite bigFont, font, littleFont;
        OMNOM centerOmNom;
        TopBannerSprite tbs;

        public int OmNomValue
        {
            get { return gotCandy == null ? 0 : gotCandy.Value; }
        }

        public override int Score
        {
            get
            {
                return StarsGot * FCOptions.PointsPerStar + OmNomValue;
            }
        }

        public RedemptionLevel(Input input, ContentManager content, string levelText)
            : this(input, content, XDocument.Parse(levelText), true)
        { }

        public RedemptionLevel(Input input, ContentManager content)
            : this(input, content, false)
        { }

        public RedemptionLevel(Input input, ContentManager content, bool skipIntro)
            : this(input, content, GetRandomRedemptionLevel(), skipIntro)
        {
            level = randLevel;
        }

        private RedemptionLevel(Input input, ContentManager content, XDocument levelFile, bool skipIntro)
            : base(input, content, levelFile)
        {
            spikeGroups = new List<int>();
            foreach (Spike spike in spikes)
            {
                if (spike is RotoSpike && !spikeGroups.Contains((spike as RotoSpike).Group))
                {
                    spikeGroups.Add((spike as RotoSpike).Group);
                }
            }

            percentage = (float)((FCOptions.TotalTicketsOut * FCOptions.TicketValue) / (FCOptions.TotalGamesPlayed * FCOptions.GameCost + 0.1M) / FCOptions.PayoutPCT);

            int i = 0;

            foreach (OMNOM omnom in omnoms)
            {
                if (omnom.Value == 5000)
                {
                    omnom.Value = FCOptions.BigBonusValue;
                    centerOmNom = omnom;
                    if (Util.OnDevice)
                    {
                        omnom.SetType(content, OMNOM.SpecialCharacter.Prehistoric);
                    }
                }
                else if (omnom.Value == 3000)
                {
                    omnom.Value = FCOptions.MediumBonusValue;
                    if (Util.OnDevice)
                    {
                        if (i % 2 == 0)
                        {
                            omnom.SetType(content, OMNOM.SpecialCharacter.Pirate);
                        }
                        else
                        {
                            omnom.SetType(content, OMNOM.SpecialCharacter.Pharaoh);
                        }
                    }
                    i++;
                }
                else if (omnom.Value == 1000)
                {
                    omnom.Value = FCOptions.SmallBonusValue;
                    if (Util.OnDevice)
                    {
                        if (i % 2 == 0)
                        {
                            omnom.SetType(content, OMNOM.SpecialCharacter.Caesar);
                        }
                        else
                        {
                            omnom.SetType(content, OMNOM.SpecialCharacter.Viking);
                        }
                    }
                    i++;
                }
            }

            if (!skipIntro)
            {
                introAnimationTimer = 4;
            }

            bigFont = new TextSprite(content, true, 4);

            littleFont = new TextSprite(content, false, 0.75f);

            font = new TextSprite(content, true);

            tbs = new TopBannerSprite(content);


            stars = new Star[0];
        }

        private static XDocument GetRandomRedemptionLevel()
        {
            randLevel++;
            if (randLevel > 3)
            {
                randLevel = 1;
            }
            return XDocument.Load("Content/RedemptionMaps/" + randLevel + ".xml");
        }

        protected override void LoadSpikes(XElement layer, ContentManager content)
        {
            foreach (XElement child in layer.Elements().Where(e => e.Name.ToString().StartsWith("spike")))
            {
                if (child.Attribute("toggled") == null || child.Attribute("toggled").Value == "false")
                {
                    spikes.Add(new Spike(content, Mover.Parse(child, GetPosition(child)), GetPosition(child), Deg2Rad(ParseInt(child, "angle") ?? 0), ParseInt(child, "size") ?? 1, true));
                }
                else
                {
                    spikes.Add(new RotoSpike(content, Mover.Parse(child, GetPosition(child)), GetPosition(child), Deg2Rad(ParseInt(child, "angle") ?? 0), ParseInt(child, "size") ?? 1, ParseInt(child, "toggled") ?? 1, true));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            float dT = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (introAnimationTimer > 2)
            {
                CTRGame.PlayGameMusic();
                introAnimationTimer = 2;
            }

            if (introAnimationTimer > 0)
            {
                introAnimationTimer -= dT * 2;
                return;
            }

            state.Camera.Update(gameTime, state);

            if (!gameOver)
            {
                totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (TimeOut)
                {
                    CutRope(false);
                }
            }

            for (int i = 0; i < hudStarSprites.Length; i++)
            {
                hudStarSprites[i].Update(gameTime);
            }

            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] == null)
                {
                    continue;
                }
                stars[i].Update(gameTime, state);
                if (!state.Candy.Spidered && !stars[i].Gone && !stars[i].Got && (state.Candy.Position - stars[i].Position).LengthSquared() <= 30 * 30 * SCALE * SCALE)
                {
                    GotStar(state.Candy, stars[i]);
                }
                else if (!stars[i].Gone && !stars[i].Got && state.Candy2 != null && !state.Candy2.Spidered && (state.Candy2.Position - stars[i].Position).LengthSquared() <= 30 * 30 * SCALE * SCALE)
                {
                    GotStar(state.Candy2, stars[i]);
                }
            }

            pollen.Update(gameTime, state);

            foreach (OMNOM omnom in omnoms)
            {
                omnom.Update(gameTime, state);

                float omnomRadius = 115;

                if (omnom == centerOmNom)
                {
                    omnomRadius = Clamp(115 / percentage, 45, 200);
                }

                if (gotCandy == null && !state.Candy.Spidered && !state.Candy.Half && (state.Candy.Position - omnom.Position).LengthSquared() <= omnomRadius * omnomRadius)
                {
                    CandyGot(omnom);
                }
            }


            foreach (Record record in records)
            {
                record.Update(gameTime, state);
                record.Rotate(0.01f);
            }

            foreach (Bubble bubble in bubbles)
            {
                bubble.Update(gameTime, state);
                if (!bubble.Released && (state.Candy.Position - bubble.Position).LengthSquared() <= (900 * SCALE * SCALE))
                {
                    bubble.Release();
                    state.Candy.ActivateBubble(bubble);
                }
                if (!bubble.Released && state.Candy2 != null && (state.Candy2.Position - bubble.Position).LengthSquared() <= (900 * SCALE * SCALE))
                {
                    bubble.Release();
                    state.Candy2.ActivateBubble(bubble);
                }
            }

            foreach (Spike spike in spikes)
            {
                spike.Update(gameTime, state);
                if (spike is RotoSpike)
                {
                    if ((spike as RotoSpike).ButtonPressed)
                    {
                        foreach (Spike sub in spikes)
                        {
                            if (sub is RotoSpike && (sub as RotoSpike).Group == (spike as RotoSpike).Group)
                            {
                                (sub as RotoSpike).Rotate();
                            }
                        }
                    }
                }
                if (spike.IntersectsCandy(state.Candy.Position))
                {
                    if (spike.Size > 1)
                    {
                        state.Candy.PopBubble();
                    }
                    SoftBounce(state.Candy, spike);
                }
                if (state.Candy2 != null && spike.IntersectsCandy(state.Candy2.Position))
                {
                    if (spike.Size > 1)
                    {
                        state.Candy2.PopBubble();
                    }
                    SoftBounce(state.Candy2, spike);
                }
            }

            if (spikeGroups.Count > 0)
            {
                spikeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (spikeTimer < 0)
                {
                    foreach (Spike sub in spikes)
                    {
                        if (sub is RotoSpike && (sub as RotoSpike).Group == spikeGroups[nextSpike])
                        {
                            (sub as RotoSpike).Rotate();
                        }
                    }
                    spikeTimer += 1f / spikeGroups.Count;
                    nextSpike++;
                    nextSpike %= spikeGroups.Count;
                }
            }

            foreach (Pump pump in pumps)
            {
                pump.Update(gameTime, state);
            }

            if (pumps.Count > 0)
            {
                pumpTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (pumpTimer < 0)
                {
                    pumps[nextPump].OperatePump(state, -1);
                    pumpTimer += 1f / pumps.Count;
                    nextPump++;
                    nextPump %= pumps.Count;
                }
            }

            foreach (GravityButton button in gravityButtons)
            {
                button.Update(gameTime, state);
            }

            foreach (MagicHat hat in hats)
            {
                hat.Update(gameTime, state);
                if (hat.IntersectsCandy(state.Candy))
                {
                    TeleportFrom(state.Candy, hat);
                }
                if (state.Candy2 != null && hat.IntersectsCandy(state.Candy2))
                {
                    TeleportFrom(state.Candy2, hat);
                }
            }

            foreach (Bouncer bouncer in bouncers)
            {
                bouncer.Update(gameTime, state);
                if (bouncer.IntersectsCandy(state.Candy.Position))
                {
                    Bounce(state.Candy, bouncer);
                }
                if (state.Candy2 != null && bouncer.IntersectsCandy(state.Candy2.Position))
                {
                    Bounce(state.Candy2, bouncer);
                }
            }

            if (gameTime.ElapsedGameTime.TotalSeconds < 0.5)
            {
                if (gotCandy != null)
                {
                    state.Candy.UpdateGot(gameTime, gotCandy.Position);
                }
                else
                {
                    state.Candy.Update(gameTime, state);
                    if (state.Candy.Position.X > 1080 && state.Candy.Velocity.X > 0)
                    {
                        state.Candy.ReflectLR();
                        state.Candy.PopBubble();
                    }
                    if (state.Candy.Position.X < 0 && state.Candy.Velocity.X < 0)
                    {
                        state.Candy.ReflectLR();
                        state.Candy.PopBubble();
                    }
                    if (FallenOffScreen(state.Candy))
                    {
                        if (state.Candy.HasBubble)
                        {
                            state.Candy.PopBubble();
                        }
                        else
                        {
                            GiveCandyToNearestOmNom(state.Candy);
                        }
                    }
                    if (state.Candy.Position.Y > centerOmNom.Position.Y && !state.Candy.HasBubble)
                    {
                        GiveCandyToNearestOmNom(state.Candy);
                    }
                }
                if (state.Candy2 != null)
                {
                    state.Candy2.Update(gameTime, state);
                    float candySep = (state.Candy2.Position - state.Candy.Position).Length();
                    if (candyLinkLength > 0)
                    {
                        candyLinkLength -= 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        candyLinkLength = Math.Min(candyLinkLength, candySep);
                        if (candyLinkLength < 0)
                        {
                            state.Candy.JoinWith(state.Candy2);
                            foreach (Grab grab in grabs)
                            {
                                if (grab.AttachedCandy == state.Candy2)
                                {
                                    grab.SetAttachedCandy(state.Candy);
                                }
                            }
                            state.Candy2 = null;
                        }
                        else
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                Vector2 delta = state.Candy2.Physics.position - state.Candy.Physics.position;
                                if (delta == Vector2.Zero)
                                {
                                    delta = Vector2.One;
                                }

                                float deltaLength = delta.Length();
                                if (deltaLength < candyLinkLength)
                                {
                                    break;
                                }
                                float restLength = candyLinkLength;

                                float diff = (deltaLength - restLength) / (Math.Max(1.0f, deltaLength) * (1.0f / state.Candy.Physics.mass + 1.0f / state.Candy2.Physics.mass));

                                Vector2 other = delta;
                                other *= diff / state.Candy2.Physics.mass;
                                state.Candy2.Physics.position -= other;


                                delta *= diff / state.Candy.Physics.mass;
                                state.Candy.Physics.position += delta;
                            }
                        }
                    }
                    else if (candySep <= (30 * SCALE))
                    {
                        candyLinkLength = candySep;
                    }
                    if (state.Candy2 != null && FallenOffScreen(state.Candy2))
                    {
                        if (state.Candy2.HasBubble)
                        {
                            state.Candy2.PopBubble();
                        }
                        else
                        {
                            EndGame(false);
                        }
                    }
                }

                for (int i = cuts.Count - 1; i >= 0; i--)
                {
                    cuts[i].lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (cuts[i].lifetime <= 0)
                    {
                        cuts.RemoveAt(i);
                    }
                }

                foreach (Grab grab in grabs)
                {
                    grab.Update(gameTime, state);
                    foreach (Cut cut in cuts)
                    {
                        grab.ApplyCut(cut);
                    }
                    if (grab.SpiderGotCandy)
                    {
                        SpiderWin(grab);
                    }
                }
            }
            if (endTimer > 0)
            {
                endTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            ApplyInput();
        }

        private void GiveCandyToNearestOmNom(Candy candy)
        {
            OMNOM nearest = omnoms[0];
            foreach (OMNOM omnom in omnoms)
            {
                if (omnom != centerOmNom
                    && (omnom.Position - candy.Position).LengthSquared() < (nearest.Position - candy.Position).LengthSquared())
                {
                    nearest = omnom;
                }
            }
            CandyGot(nearest);
        }


        public void CutRope(bool fromAttract)
        {
            foreach (Grab grab in grabs)
            {
                if (grab.AttachedCandy == state.Candy)
                {
                    grab.ReleaseAtEnd();
                }
            }
            state.Candy.PopBubble();
            if (fromAttract)
            {
                level = -1;
            }
        }

        protected override void RecordData(bool victory)
        {
            if (level >= 0)
            {
                if (victory)
                {
                    if (gotCandy.Value == FCOptions.SmallBonusValue)
                    {
                        FCOptions.SmallBonusCount++;
                    }
                    else if (gotCandy.Value == FCOptions.MediumBonusValue)
                    {
                        FCOptions.MediumBonusCount++;
                    }
                    else
                    {
                        FCOptions.BigBonusCount++;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            tbs.Draw(spriteBatch, Vector2.Zero, 0);
            font.SetScale(0.8f);
            string bonusText = "Win up to " + FCOptions.BigBonusValue + " " + (FCOptions.FixedTickets || !FCOptions.UseTickets ? "Point" : FCOptions.TicketName.ToString()) + "s!";
            font.Draw(spriteBatch, bonusText, new Vector2(330, 10));
            littleFont.Draw(spriteBatch, "One Cut to Win Bonus!", new Vector2(10, 140), TextSprite.Alignment.Left, Color.Black, -0.10f);
            if (introAnimationTimer > 0)
            {
                if (introAnimationTimer > 1)
                {
                    bigFont.Draw(spriteBatch, "Bonus!", new Vector2(540, 600), TextSprite.Alignment.Center, new Color(Color.White, introAnimationTimer - 1));
                }
                else
                {
                    bigFont.Draw(spriteBatch, "GO!", new Vector2(540, 800), TextSprite.Alignment.Center, new Color(Color.White, introAnimationTimer));
                }
            }
            spriteBatch.End();
        }

        protected override void DrawHudStars(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
        }

        protected void SoftBounce(Candy c, Spike b)
        {
            Vector2 v = c.Physics.lastPos - c.Physics.position;
            Vector2 spos = Util.RotateVector(c.Physics.lastPos, -b.Rotation, b.Position);
            bool fromTop = spos.Y < b.Position.Y;
            int dir = (fromTop) ? -1 : 1;
            float m = Math.Max(v.Length() * 30, 100) * dir;
            Vector2 impulse = new Vector2(-(float)Math.Sin(b.Rotation), (float)Math.Cos(b.Rotation)) * m;

            if (b as RotoSpike == null || !(b as RotoSpike).Rotating)
            {
                c.Physics.position = Util.RotateVector(c.Physics.position, -b.Rotation, b.Position);
                c.Physics.lastPos = Util.RotateVector(c.Physics.lastPos, -b.Rotation, b.Position);
                c.Physics.lastPos.Y = c.Physics.position.Y;
                c.Physics.position = Util.RotateVector(c.Physics.position, b.Rotation, b.Position);
                c.Physics.lastPos = Util.RotateVector(c.Physics.lastPos, b.Rotation, b.Position);
            }

            c.Physics.position += impulse * 0.016f;
        }

    }
}
