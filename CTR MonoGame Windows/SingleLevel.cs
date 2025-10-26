using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CTR_MonoGame
{
    class SingleLevel : ILevel
    {
        public const float SCALE = 3.375f;
        public const int LEVEL_HEIGHT = 1620;
        public const int Y_OFFSET = 300;

        protected Box box;
        protected List<OMNOM> omnoms;
        protected List<Grab> grabs;
        protected List<Bubble> bubbles;
        protected List<Spike> spikes;
        protected List<Pump> pumps;
        protected List<MagicHat> hats;
        protected List<Bouncer> bouncers;
        protected List<GravityButton> gravityButtons;
        protected List<Record> records;
        protected List<TutorialText> tutorialTexts;
        protected Star[] stars;
        protected GlobalState state;
        protected OMNOM gotCandy;
        protected bool mouseDragging;
        protected bool[] touchDragging = new bool[Input.TOUCH_COUNT];
        protected List<Cut> cuts;
        protected Point levelSize;
        protected bool gameOver;
        protected Pollen pollen;
        public bool Victory
        {
            get;
            protected set;
        }
        public bool GameOver
        {
            get { return gameOver && endTimer <= 0; }
        }
        protected float endTimer;
        protected SoundFX[] starsGotSounds;

        protected float candyLinkLength;

        public int StarsGot
        {
            get
            {
                int sg = 0;
                foreach (Star s in stars)
                {
                    if (s.Got)
                    {
                        sg++;
                    }
                }
                return sg;
            }
        }

        protected bool TimeOut
        {
            get { return totalTime > FCOptions.TimeLimit; }
        }

        int boxNum, levelNum;
        protected ContentManager content;
        protected float totalTime;

        public bool Chewing
        {
            get { return gameOver && Victory; }
        }

        public bool Sad
        {
            get { return gameOver && !Victory; }
        }

        public float ElapsedTime
        {
            get { return totalTime; }
        }

        public float[] Times
        {
            get { return new float[] { ElapsedTime }; }
        }

        protected float percentage;

        public int LevelID
        {
            get { return (boxNum - 1) * 25 + levelNum - 1; }
        }

        public int BoxNum
        {
            get { return boxNum; }
        }

        public int LevelNum
        {
            get { return levelNum; }
        }

        public virtual int Score
        {
            get
            {
                return StarsGot * FCOptions.PointsPerStar + (gameOver && Victory ?
                (int)Math.Max(FCOptions.MaxTimeBonusPoints - Math.Max(ElapsedTime - (FCOptions.ParTimePerLevel[LevelID] + (float)FCOptions.ParTimeOffset), 0) * FCOptions.PointsLostPerSecond, 0)
                : 0);
            }
        }

        TextSprite timeOutFont, tutorialFont;

        protected Texture2D shade;
        protected HudStarSprite2[] hudStarSprites;

        protected TutorialGraphicSprite line, hand;
        protected float handPos;


        public SingleLevel(Input input, ContentManager content)
        {
            InitializeArrays(input, content);
        }

        public SingleLevel(Input input, ContentManager content, XDocument levelFile)
        {
            box = Box.GetBox(3);
            box.Load(content);
            InitializeArrays(input, content);

            XElement map = levelFile.Element("map");

            foreach (XElement layer in map.Elements("layer"))
            {
                if (layer.Attribute("name").Value == "settings")
                {
                    ProcessSettings(layer);
                }
                else if (layer.Attribute("name").Value == "Objects")
                {
                    ProcessObjects(layer, content);
                }
            }
            state.Camera = new Camera(levelSize, state);

            totalTime = 0;

            //percentage = (float)((FCOptions.TotalTicketsOut * FCOptions.TicketValue) / (FCOptions.TotalGamesPlayed * FCOptions.GameCost + 0.1M) / FCOptions.PayoutPCT);

            percentage = 1;

            if (CTRGame.LEVEL_TEST)
            {
                percentage = 1;
            }

            shade = content.Load<Texture2D>("menu_drawings_bigpage_markers_hd");

        }

        public SingleLevel(Input input, ContentManager content, int boxNum, int levelNum)
        {
            box = Box.GetBox(boxNum);
            box.Load(content);
            InitializeArrays(input, content);

            this.boxNum = boxNum;
            this.levelNum = levelNum;

            Console.WriteLine("Loading level " + boxNum + "-" + levelNum);

            XDocument levelFile = XDocument.Load("Content/Maps/" + boxNum + "_" + levelNum + ".xml");

            XElement map = levelFile.Element("map");

            foreach (XElement layer in map.Elements("layer"))
            {
                if (layer.Attribute("name").Value == "settings")
                {
                    ProcessSettings(layer);
                }
                else if (layer.Attribute("name").Value == "Objects")
                {
                    ProcessObjects(layer, content);
                }
            }
            state.Camera = new Camera(levelSize, state);

            totalTime = 0;

            shade = content.Load<Texture2D>("menu_drawings_bigpage_markers_hd");

            //percentage = (float)((FCOptions.TotalTicketsOut * FCOptions.TicketValue) / (FCOptions.TotalGamesPlayed * FCOptions.GameCost + 0.1M) / FCOptions.PayoutPCT);

            percentage = 1;

            if (CTRGame.LEVEL_TEST)
            {
                percentage = 1;
            }

            line = new TutorialGraphicSprite(content, TutorialGraphicSprite.Sign.CutLine);
            hand = new TutorialGraphicSprite(content, TutorialGraphicSprite.Sign.Finger);

        }

        public static List<HowToPopup.Obstacle> ShownObstacles = new List<HowToPopup.Obstacle>();

        public virtual ILevel Reset()
        {
            return new SingleLevel(state.Input, content, boxNum, levelNum);
        }

        protected void InitializeArrays(Input input, ContentManager content)
        {
            omnoms = new List<OMNOM>();
            cuts = new List<Cut>();
            grabs = new List<Grab>();
            state = new GlobalState();
            bubbles = new List<Bubble>();
            spikes = new List<Spike>();
            pumps = new List<Pump>();
            hats = new List<MagicHat>();
            bouncers = new List<Bouncer>();
            gravityButtons = new List<GravityButton>();
            records = new List<Record>();
            tutorialTexts = new List<TutorialText>();
            pollen = new Pollen(content);
            state.Gravity = Vector2.UnitY * 1568;
            state.Input = input;
            stars = new Star[3];
            endTimer = -1;
            candyLinkLength = -1;
            this.content = content;

            starsGotSounds = new SoundFX[] { new SoundFX("Content/star_1.ogg"),
            new SoundFX("Content/star_2.ogg"),
            new SoundFX("Content/star_3.ogg")};

            hudStarSprites = new HudStarSprite2[] { new HudStarSprite2(content), new HudStarSprite2(content), new HudStarSprite2(content) };

            timeOutFont = new TextSprite(content, true);
            tutorialFont = new TextSprite(content, false, 1.3f);
        }

        protected void ProcessObjects(XElement layer, ContentManager content)
        {
            LoadCandy(layer, content);
            LoadGrabs(layer, content);
            foreach (Grab g in grabs)
            {
                if (g.Mover != null)
                {
                    pollen.AddPollen(g.Mover);
                }
            }
            foreach (XElement child in layer.Elements("bubble"))
            {
                bubbles.Add(new Bubble(content, GetPosition(child)));
            }
            foreach (XElement child in layer.Elements("gravitySwitch"))
            {
                gravityButtons.Add(new GravityButton(content, GetPosition(child)));
            }
            LoadSpikes(layer, content);
            foreach (XElement child in layer.Elements().Where(e => e.Name.ToString().StartsWith("bouncer")))
            {
                bouncers.Add(new Bouncer(content, Mover.Parse(child, GetPosition(child)), GetPosition(child), Deg2Rad(ParseInt(child, "angle") ?? 0), ParseInt(child, "size") ?? 1));
            }
            foreach (XElement child in layer.Elements("electro"))
            {
                spikes.Add(new ElectroSpike(content, Mover.Parse(child, GetPosition(child)), GetPosition(child), Deg2Rad(ParseInt(child, "angle") ?? 0),
                    ParseFloat(child, "initialDelay") ?? 0, ParseFloat(child, "onTime") ?? 1, ParseFloat(child, "offTime") ?? 1));
            }
            LoadHats(layer, content);
            LoadOmNom(layer, content);
            LoadStars(layer, content);
            foreach (XElement child in layer.Elements("pump"))
            {
                pumps.Add(new Pump(content, GetPosition(child), Deg2Rad((ParseInt(child, "angle") ?? 0) + 90)));
            }
            foreach (XElement child in layer.Elements("rotatedCircle"))
            {
                records.Add(new Record(content, GetPosition(child), Parse(child, "size") ?? 100, Deg2Rad((ParseInt(child, "handleAngle") ?? 0)), ParseBool(child, "oneHandle") ?? false));
            }
            foreach (Record record in records)
            {
                record.Load(grabs);
                record.Load(bubbles);
                record.Load(pumps);
                record.SetRecordGroup(records);
            }
            foreach (XElement child in layer.Elements("tutorialText"))
            {
                tutorialTexts.Add(new TutorialText { Position = GetPosition(child), Text = tutorialFont.Wrap(child.Attribute("text").Value, Parse(child, "width") ?? -1) });
            }
        }

        protected virtual void LoadSpikes(XElement layer, ContentManager content)
        {
            foreach (XElement child in layer.Elements().Where(e => e.Name.ToString().StartsWith("spike")))
            {
                if (child.Attribute("toggled") == null || child.Attribute("toggled").Value == "false")
                {
                    spikes.Add(new Spike(content, Mover.Parse(child, GetPosition(child)), GetPosition(child), Deg2Rad(ParseInt(child, "angle") ?? 0), ParseInt(child, "size") ?? 1));
                }
                else
                {
                    spikes.Add(new RotoSpike(content, Mover.Parse(child, GetPosition(child)), GetPosition(child), Deg2Rad(ParseInt(child, "angle") ?? 0), ParseInt(child, "size") ?? 1, ParseInt(child, "toggled") ?? 1));
                }
            }
        }

        protected virtual void LoadHats(XElement layer, ContentManager content)
        {
            foreach (XElement child in layer.Elements("sock"))
            {
                hats.Add(new MagicHat(content, GetPosition(child), Deg2Rad(ParseInt(child, "angle") ?? 0), Mover.Parse(child, GetPosition(child)), ParseInt(child, "group") ?? 0));
            }
        }

        protected virtual void LoadOmNom(XElement layer, ContentManager content)
        {
            foreach (XElement child in layer.Elements("target"))
            {
                omnoms.Add(new OMNOM(content, box, GetPosition(child), ParseInt(child, "value") ?? 0));
            }
        }

        protected virtual void LoadGrabs(XElement layer, ContentManager content)
        {
            foreach (XElement child in layer.Elements("grab"))
            {
                grabs.Add(new Grab(content, Mover.Parse(child, GetPosition(child)), GetPosition(child),
                    Parse(child, "moveLength") ?? -1, bool.Parse(child.Attribute("moveVertical").Value),
                    Parse(child, "moveOffset") ?? 0, Parse(child, "radius") ?? -1, Parse(child, "length") ?? 30,
                    child.Attribute("spider") != null && bool.Parse(child.Attribute("spider").Value),
                    child.Attribute("wheel") != null && bool.Parse(child.Attribute("wheel").Value),
                    (child.Attribute("part") == null || child.Attribute("part").Value == "L") ? state.Candy : state.Candy2));
            }
        }

        protected virtual void LoadCandy(XElement layer, ContentManager content)
        {
            foreach (XElement child in layer.Elements("candy"))
            {
                state.Candy = new Candy(content, GetPosition(child), false, false);
            }
            foreach (XElement child in layer.Elements("candyL"))
            {
                state.Candy = new Candy(content, GetPosition(child), true, true);
            }
            foreach (XElement child in layer.Elements("candyR"))
            {
                state.Candy2 = new Candy(content, GetPosition(child), true, false);
            }
        }

        protected virtual void LoadStars(XElement layer, ContentManager content)
        {
            int star = 0;
            foreach (XElement child in layer.Elements("star"))
            {
                float timeout = ParseFloat(child, "timeout") ?? -1;
                if (timeout > 0)
                {
                    timeout *= 1.5f;
                    //timeout /= Clamp(percentage, 0.5f, 1.5f);
                }
                stars[star++] = new Star(content, Mover.Parse(child, GetPosition(child)), GetPosition(child), timeout);
            }
        }

        protected float Deg2Rad(float p)
        {
            return (float)(p * Math.PI / 180.0f);
        }

        protected float? Parse(XElement node, string attributeName)
        {
            if (node.Attribute(attributeName) != null)
            {
                return int.Parse(node.Attribute(attributeName).Value) * SCALE;
            }
            return null;
        }

        protected bool? ParseBool(XElement node, string attributeName)
        {
            if (node.Attribute(attributeName) != null)
            {
                return bool.Parse(node.Attribute(attributeName).Value);
            }
            return null;
        }

        protected int? ParseInt(XElement node, string attributeName)
        {
            if (node.Attribute(attributeName) != null)
            {
                return int.Parse(node.Attribute(attributeName).Value);
            }
            return null;
        }

        protected float? ParseFloat(XElement node, string attributeName)
        {
            if (node.Attribute(attributeName) != null)
            {
                return float.Parse(node.Attribute(attributeName).Value);
            }
            return null;
        }

        protected virtual Vector2 GetPosition(XElement node)
        {
            return new Vector2((float)int.Parse(node.Attribute("x").Value) * SCALE, (float)int.Parse(node.Attribute("y").Value) * SCALE + Y_OFFSET);
        }

        protected virtual void ProcessSettings(XElement layer)
        {
            foreach (XElement child in layer.Descendants())
            {
                if (child.Name == "map")
                {
                    levelSize = new Point(int.Parse(child.Attribute("width").Value) / 320, int.Parse(child.Attribute("height").Value) / 480);
                }
                if (child.Name == "gameDesign")
                {
                }
            }
        }

        public virtual void UpdateTransition(GameTime gameTime)
        {
            for (int i = 0; i < hudStarSprites.Length; i++)
            {
                hudStarSprites[i].Update(gameTime);
            }

            foreach (OMNOM omnom in omnoms)
            {
                omnom.Update(gameTime, state);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            state.Camera.Update(gameTime, state);

            if (!gameOver)
            {
                totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (TimeOut)
                {
                    SlayCandy(state.Candy);
                    SlayCandy(state.Candy2);
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

                float omnomRadius = 115; // Clamp(115 / percentage, 45, 200);

                if (gotCandy == null && !state.Candy.Spidered && !state.Candy.Half && (state.Candy.Position - omnom.Position).LengthSquared() <= omnomRadius * omnomRadius)
                {
                    CandyGot(omnom);
                }
            }


            foreach (Record record in records)
            {
                record.Update(gameTime, state);
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
                    BreakCandy(state.Candy);
                }
                if (state.Candy2 != null && spike.IntersectsCandy(state.Candy2.Position))
                {
                    BreakCandy(state.Candy2);
                }
            }

            foreach (Pump pump in pumps)
            {
                pump.Update(gameTime, state);
            }

            foreach (GravityButton button in gravityButtons)
            {
                button.Update(gameTime, state);
            }

            foreach (MagicHat hat in hats)
            {
                hat.Update(gameTime, state);
                if (hat.CoolDown <= 0)
                {
                    if (hat.IntersectsCandy(state.Candy))
                    {
                        TeleportFrom(state.Candy, hat);
                    }
                    if (state.Candy2 != null && hat.IntersectsCandy(state.Candy2))
                    {
                        TeleportFrom(state.Candy2, hat);
                    }
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
                    if (FallenOffScreen(state.Candy))
                    {
                        if (state.Candy.HasBubble && BoxNum <= 2)
                        {
                            state.Candy.PopBubble();
                        }
                        else
                        {
                            EndGame(false);
                        }
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
                        if (state.Candy2.HasBubble && BoxNum <= 2)
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

            if (LevelID == 0 || LevelID == 75)
            {
                handPos += (float)gameTime.ElapsedGameTime.TotalSeconds;
                handPos %= 3;
                hand.SetAlpha(Math.Min(1, 10 * (0.5f - Math.Abs(handPos - 0.5f))));
            }
        }

        protected virtual void ApplyInput()
        {
            if (Util.OnDevice)
            {
                for (int i = 0; i < Input.TOUCH_COUNT; i++)
                {
                    ApplyTouch(i);
                }
            }
            else
            {
                if (state.Input.MouseButtonDown && !state.Camera.IgnoreTouches)
                {
                    if (state.Input.MouseJustClicked())
                    {
                        // TODO: Check pumps, buttons, etc.

                        if (state.Candy.HasBubble && (state.Input.MousePos + state.Camera.Position - state.Candy.Position).LengthSquared() < 30 * 30 * SCALE * SCALE)
                        {
                            state.Candy.PopBubble();
                            state.Input.ConsumeClick();
                        }
                        if (state.Candy2 != null && state.Input.MouseJustClicked() && state.Candy2.HasBubble && (state.Input.MousePos + state.Camera.Position - state.Candy2.Position).LengthSquared() < 30 * 30 * SCALE * SCALE)
                        {
                            state.Candy2.PopBubble();
                            state.Input.ConsumeClick();
                        }
                        if (state.Input.MouseJustClicked())
                        {
                            mouseDragging = true;
                        }
                    }
                    else
                    {
                        if (mouseDragging)
                        {
                            if (state.Input.MousePos != state.Input.LastMousePos)
                            {
                                cuts.Add(new Cut(state.Input.LastMousePos + state.Camera.Position, state.Input.MousePos + state.Camera.Position));
                            }
                        }
                    }
                }
                else
                {
                    mouseDragging = false;
                }
            }
        }

        protected virtual void ApplyTouch(int touch)
        {
            if (state.Input.TouchDown(touch) && !state.Camera.IgnoreTouches)
            {
                if (state.Input.MouseJustClicked(touch))
                {
                    // TODO: Check pumps, buttons, etc.

                    if (state.Candy.HasBubble && (state.Input.TouchPos(touch) + state.Camera.Position - state.Candy.Position).LengthSquared() < 30 * 30 * SCALE * SCALE)
                    {
                        state.Candy.PopBubble();
                        state.Input.ConsumeClick(touch);
                    }
                    if (state.Candy2 != null && state.Input.MouseJustClicked(touch) && state.Candy2.HasBubble && (state.Input.TouchPos(touch) + state.Camera.Position - state.Candy2.Position).LengthSquared() < 30 * 30 * SCALE * SCALE)
                    {
                        state.Candy2.PopBubble();
                        state.Input.ConsumeClick(touch);
                    }
                    if (state.Input.MouseJustClicked(touch))
                    {
                        touchDragging[touch] = true;
                    }
                }
                else
                {
                    if (touchDragging[touch])
                    {
                        if (state.Input.TouchPos(touch) != state.Input.LastTouchPos(touch))
                        {
                            cuts.Add(new Cut(state.Input.LastTouchPos(touch) + state.Camera.Position, state.Input.TouchPos(touch) + state.Camera.Position));
                        }
                    }
                }
            }
            else
            {
                touchDragging[touch] = false;
            }
        }

        protected float Clamp(float val, float min, float max)
        {
            return Math.Max(min, Math.Min(max, val));
        }

        protected virtual void CandyGot(OMNOM omnom)
        {
            gotCandy = omnom;
            state.Candy.CandyGot();
            omnom.CandyGot();
            foreach (Grab g in grabs)
            {
                g.ReleaseAtEnd();
            }
            EndGame(true);
        }

        protected bool FallenOffScreen(Candy c)
        {
            int SagFactor = 100;

            foreach (Grab g in grabs)
            {
                if (g.AttachedCandy == c)
                {
                    return false;
                }
            }

            if (state.Gravity.Y > 0)
            {
                if (c.HasBubble)
                {
                    return c.Position.Y < -SagFactor;
                }
                else
                {
                    return c.Position.Y > LEVEL_HEIGHT * levelSize.Y + Y_OFFSET + SagFactor;
                }
            }
            else
            {
                if (!c.HasBubble)
                {
                    return c.Position.Y < -SagFactor;
                }
                else
                {
                    return c.Position.Y > LEVEL_HEIGHT * levelSize.Y + Y_OFFSET + SagFactor;
                }
            }
        }

        protected void Bounce(Candy c, Bouncer b)
        {
            if (TimeOut)
                return;
            Vector2 v = c.Physics.lastPos - c.Physics.position;
            Vector2 spos = Util.RotateVector(c.Physics.lastPos, -b.Rotation, b.Position);
            bool fromTop = spos.Y < b.Position.Y;
            int dir = (fromTop) ? -1 : 1;
            float m = Math.Max(v.Length() * 30, 900) * dir;
            Vector2 impulse = new Vector2(-(float)Math.Sin(b.Rotation), (float)Math.Cos(b.Rotation)) * m;

            c.Physics.position = Util.RotateVector(c.Physics.position, -b.Rotation, b.Position);
            c.Physics.lastPos = Util.RotateVector(c.Physics.lastPos, -b.Rotation, b.Position);
            c.Physics.lastPos.Y = c.Physics.position.Y;
            c.Physics.position = Util.RotateVector(c.Physics.position, b.Rotation, b.Position);
            c.Physics.lastPos = Util.RotateVector(c.Physics.lastPos, b.Rotation, b.Position);

            c.Physics.position += impulse * 0.016f;
            b.Bounce();
        }

        protected void TeleportFrom(Candy c, MagicHat from)
        {
            if (TimeOut)
                return;
            foreach (MagicHat to in hats)
            {
                if (to.Group == from.Group && to != from)
                {
                    from.Teleport(true);
                    to.Teleport(false);

                    Vector2 off = new Vector2(0.0f, -8.0f) * SCALE;

                    off = Util.RotateVector(off, to.Rotation);

                    c.Physics.position = to.Position + off;

                    Vector2 newV = Util.RotateVector(Vector2.UnitY, to.Rotation) * c.Velocity.Length() * 0.9f;

                    c.Physics.lastPos = c.Physics.position + newV / 60;

                    foreach (Grab grab in grabs)
                    {
                        if (grab.AttachedCandy == c)
                        {
                            grab.ReleaseAtEnd();
                        }
                    }

                    break;
                }
            }
        }

        protected void SpiderWin(Grab g)
        {
            Candy c = g.AttachedCandy;
            if (c == null)
                return;
            SlayCandy(c);
            g.GiveSpiderCandy(c, state);
        }

        protected void SlayCandy(Candy c)
        {
            if (c != null)
            {
                c.SpiderGot();
                foreach (Grab grab in grabs)
                {
                    if (grab.AttachedCandy == c)
                    {
                        grab.ReleaseAtEnd();
                    }
                }
            }
        }

        protected void BreakCandy(Candy c)
        {
            c.PopBubble();
            foreach (Grab grab in grabs)
            {
                if (grab.AttachedCandy == c)
                {
                    grab.ReleaseAtEnd();
                }
            }
            c.Break();
            EndGame(false);
        }

        protected virtual void EndGame(bool victory)
        {
            if (!gameOver)
            {
                Victory = victory;
                gameOver = true;
                if (victory)
                {
                    endTimer = 2;
                }
                else
                {
                    endTimer = 1;
                    foreach (OMNOM omnom in omnoms)
                    {
                        omnom.Lose();
                    }
                }
                RecordData(victory);
            }
        }

        protected virtual void RecordData(bool victory)
        {
            if (LevelID >= 0)
            {
                FCOptions.SuspendWrites = true;

                if (!victory)
                {
                    int[] dpl = FCOptions.DeathsPerLevel;
                    dpl[LevelID]++;
                    FCOptions.DeathsPerLevel = dpl;
                }

                double[] tpl = FCOptions.TimePerLevel;
                tpl[LevelID] += totalTime;
                FCOptions.TimePerLevel = tpl;

                int[] spl = FCOptions.StarsPerLevel;
                spl[LevelID] += StarsGot;
                FCOptions.StarsPerLevel = spl;

                int[] ppl = FCOptions.PlaysPerLevel;
                ppl[LevelID]++;
                FCOptions.PlaysPerLevel = ppl;

                if (victory && StarsGot >= 3 && (totalTime < FCOptions.FastestPerfectTimePerLevel[LevelID] || FCOptions.FastestPerfectTimePerLevel[LevelID] <= 0))
                {
                    double[] ftpl = FCOptions.FastestPerfectTimePerLevel;
                    ftpl[LevelID] = totalTime;
                    FCOptions.FastestPerfectTimePerLevel = ftpl;
                }

                long[] popl = FCOptions.PointsPerLevel;
                popl[LevelID] += Score;
                FCOptions.PointsPerLevel = popl;

                FCOptions.SuspendWrites = false;
            }
        }

        protected void GotStar(Candy c, Star s)
        {
            c.Blink();
            s.Got = true;
            foreach (OMNOM omnom in omnoms)
            {
                omnom.Excite();
            }
            int starsGot = 0;
            foreach (Star star in stars)
            {
                if (star.Got)
                {
                    starsGot++;
                }
            }
            hudStarSprites[starsGot - 1].Earn();
            starsGotSounds[starsGot - 1].Play();
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            DrawBackground(spriteBatch);
            foreach (Record record in records)
            {
                record.Draw(spriteBatch, state.Camera.Position);
            }
            pollen.Draw(spriteBatch, state.Camera.Position);
            foreach (OMNOM omnom in omnoms)
            {
                omnom.Draw(spriteBatch, state.Camera.Position);
            }
            foreach (Bubble bubble in bubbles)
            {
                bubble.Draw(spriteBatch, state.Camera.Position);
            }
            foreach (MagicHat hat in hats)
            {
                hat.Draw(spriteBatch, state.Camera.Position);
            }
            foreach (Bouncer bouncer in bouncers)
            {
                bouncer.Draw(spriteBatch, state.Camera.Position);
            }
            foreach (Spike spike in spikes)
            {
                spike.Draw(spriteBatch, state.Camera.Position);
            }
            foreach (Pump pump in pumps)
            {
                pump.Draw(spriteBatch, state.Camera.Position);
            }
            foreach (GravityButton button in gravityButtons)
            {
                button.Draw(spriteBatch, state.Camera.Position);
            }
            foreach (Grab grab in grabs)
            {
                grab.Draw(spriteBatch, state.Camera.Position);
            }
            state.Candy.Draw(spriteBatch, state.Camera.Position);
            if (state.Candy2 != null)
            {
                state.Candy2.Draw(spriteBatch, state.Camera.Position);
            }
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].Draw(spriteBatch, state.Camera.Position);
            }
            if (TimeOut)
            {
                spriteBatch.Draw(shade, new Rectangle(0, 0, 1080, 1920), new Color(Color.Black, 0.2f));
                timeOutFont.Draw(spriteBatch, "Time Out", new Vector2(540, 960), TextSprite.Alignment.Center);
            }
            //timeOutFont.Draw(spriteBatch, state.Input.touchesActive.Where(a=>a).Count().ToString(), new Vector2(10, 1800));
            if (LevelID == 0)
            {
                line.Draw(spriteBatch, new Vector2(475, 340), 0);
                line.Draw(spriteBatch, new Vector2(345, 340), 0);
                hand.Draw(spriteBatch, new Vector2(220 + 280 * handPos, 800), 0);
            }
            if (LevelID == 75)
            {
                line.Draw(spriteBatch, new Vector2(0, 440), 0);
                line.Draw(spriteBatch, new Vector2(130, 440), 0);
                hand.Draw(spriteBatch, new Vector2(-125 + 280 * handPos, 900), 0);
            }
            DrawHudStars(spriteBatch);
            foreach (TutorialText tt in tutorialTexts)
            {
                tutorialFont.Draw(spriteBatch, tt.Text, tt.Position);
            }
            spriteBatch.End();
            
            // Draw cuts with proper scaling
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            foreach (Cut cut in cuts)
            {
                Vector2 startPos = cut.start - state.Camera.Position;
                Vector2 endPos = cut.end - state.Camera.Position;
                GLDrawer.DrawAntialiasedLine(startPos, endPos, cut.width * cut.lifetime * 10, Color.White);
            }
            spriteBatch.End();
        }

        protected virtual void DrawHudStars(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < hudStarSprites.Length; i++)
            {
                hudStarSprites[i].Draw(spriteBatch, new Vector2(430 + 80 * i, 70), 0);
            }
        }

        public virtual void DrawMiniMap(GameTime gameTime, SpriteBatch spriteBatch)
        { }

        protected virtual void DrawBackground(SpriteBatch spriteBatch)
        {
            box.Background.Draw(spriteBatch, state.Camera.Position, levelSize);
        }

        protected virtual void DrawBackgroundMini(SpriteBatch spriteBatch)
        { }
    }

    class TutorialText
    {
        public Vector2 Position;
        public string Text;
    }
}
