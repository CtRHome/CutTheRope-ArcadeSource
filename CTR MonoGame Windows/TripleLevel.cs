using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Xml.Linq;

namespace CTR_MonoGame
{
    class TripleLevel : SingleLevel
    {
        Box box2, box3;
        Point[] sizes;
        Vector2 positionOffset;
        int starCount;
        int loadingLevel;
        Scrollbar scrollbar;
        int difficulty;

        int[] levels;

        public TripleLevel(Input input, ContentManager content)
            : this(input, content, new int[] { Util.R.Next(50), Util.R.Next(50), Util.R.Next(50) }, 0)
        { }

        public TripleLevel(Input input, ContentManager content, int[] levels, int difficulty)
            : this(input, content, new int[] { levels[0] / 25 + 1, levels[1] / 25 + 1, levels[2] / 25 + 1 },
            new int[] { levels[0] % 25 + 1, levels[1] % 25 + 1, levels[2] % 25 + 1 }, difficulty)
        {
            this.levels = levels;
        }

        private TripleLevel(Input input, ContentManager content, int[] boxen, int[] levelNums, int difficulty)
            : base(input, content)
        {
            levelSize = Point.Zero;
            positionOffset = Vector2.Zero;
            starCount = 0;

            sizes = new Point[3];

            box = Box.GetBox(boxen[0]);
            box.Load(content);
            loadingLevel = 0;


            for (int i = 0; i < 3; i++)
            {
                XDocument levelFile = XDocument.Load("Content/Maps/" + boxen[i] + "_" + levelNums[i] + ".xml");

                loadingLevel = i;

                if (i == 1)
                {
                    box2 = Box.GetBox(boxen[i]);
                    box2.Load(content);
                }
                if (i == 2)
                {
                    box3 = Box.GetBox(boxen[i]);
                    box3.Load(content);
                }

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
                positionOffset = Vector2.UnitY * LEVEL_HEIGHT * levelSize.Y;
            }

            for (int i = 0; i < 3; i++)
            {
                sizes[i].X = levelSize.X;
            }

            state.Camera = new TripleLevelCam(levelSize, state);

            scrollbar = new Scrollbar(content);

            this.difficulty = difficulty;
        }

        protected override void ProcessSettings(XElement layer)
        {
            foreach (XElement child in layer.Descendants())
            {
                if (child.Name == "map")
                {
                    sizes[loadingLevel] = new Point(int.Parse(child.Attribute("width").Value) / 320, int.Parse(child.Attribute("height").Value) / 480);
                    levelSize = new Point(Math.Max(levelSize.X, sizes[loadingLevel].X), levelSize.Y + sizes[loadingLevel].Y);
                }
                if (child.Name == "gameDesign")
                {
                }
            }
        }

        public override ILevel Reset()
        {
            return new TripleLevel(state.Input, content, levels, difficulty);
        }

        protected override void LoadHats(XElement layer, ContentManager content)
        {
            foreach (XElement child in layer.Elements("sock"))
            {
                hats.Add(new MagicHat(content, GetPosition(child), Deg2Rad(ParseInt(child, "angle") ?? 0), Mover.Parse(child, GetPosition(child)), (ParseInt(child, "group") ?? 0) + 2 * loadingLevel));
            }
        }

        protected override void LoadStars(System.Xml.Linq.XElement layer, ContentManager content)
        {
            Star[] tempStars = new Star[3];
            int star = 0;
            foreach (XElement child in layer.Elements("star"))
            {
                float timeOut = ParseFloat(child, "timeout") ?? -1 * 1.5f;
                if (timeOut > 0)
                {
                    timeOut += 10 * loadingLevel;
                }
                tempStars[star++] = new Star(content, Mover.Parse(child, GetPosition(child)), GetPosition(child), timeOut);
            }
            stars[starCount++] = tempStars[Util.R.Next(3)];
        }

        protected override void LoadOmNom(XElement layer, ContentManager content)
        {
            if (loadingLevel == 2)
            {
                foreach (XElement child in layer.Elements("target"))
                {
                    omnoms.Add(new OMNOM(content, box3, GetPosition(child)));
                }
            }
        }

        protected override void LoadCandy(XElement layer, ContentManager content)
        {
            if (loadingLevel == 0)
            {
                base.LoadCandy(layer, content);
            }
            else if (state.Candy2 == null)
            {
                foreach (XElement child in layer.Elements("candyR"))
                {
                    state.Candy.SetHalf(content, true, true);
                    state.Candy2 = new Candy(content, GetPosition(child), true, false);
                }
            }
        }

        protected override void LoadGrabs(XElement layer, ContentManager content)
        {
            foreach (XElement child in layer.Elements("grab"))
            {
                float length = Parse(child, "length") ?? 30;
                float radius = Parse(child, "radius") ?? -1;
                grabs.Add(new Grab(content, Mover.Parse(child, GetPosition(child)), GetPosition(child),
                    Parse(child, "moveLength") ?? -1, bool.Parse(child.Attribute("moveVertical").Value),
                    Parse(child, "moveOffset") ?? 0, loadingLevel > 0 && radius < 0 ? length : radius, length,
                    child.Attribute("spider") != null && bool.Parse(child.Attribute("spider").Value),
                    child.Attribute("wheel") != null && bool.Parse(child.Attribute("wheel").Value),
                    (child.Attribute("part") == null || child.Attribute("part").Value == "L") ? state.Candy : state.Candy2));
            }
        }

        protected override Vector2 GetPosition(XElement node)
        {
            return base.GetPosition(node) + positionOffset;
        }

        public override void Update(GameTime gameTime)
        {
            scrollbar.Update(gameTime, state);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            scrollbar.Draw(spriteBatch, state.Camera.Position);
            spriteBatch.End();
        }

        protected override void DrawBackground(SpriteBatch spriteBatch)
        {
            box.Background.Draw(spriteBatch, state.Camera.Position, sizes[0]);
            box2.Background.Draw(spriteBatch, state.Camera.Position - Vector2.UnitY * LEVEL_HEIGHT * sizes[0].Y, sizes[1]);
            box3.Background.Draw(spriteBatch, state.Camera.Position - Vector2.UnitY * LEVEL_HEIGHT * (sizes[0].Y + sizes[1].Y), sizes[2]);
        }

        protected override void DrawBackgroundMini(SpriteBatch spriteBatch)
        {
            box.Background.DrawMini(spriteBatch, sizes[0], 0);
            box2.Background.DrawMini(spriteBatch, sizes[1], sizes[0].Y);
            box3.Background.DrawMini(spriteBatch, sizes[2], sizes[0].Y + sizes[1].Y);
        }
    }
}
