using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class CardboardBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_01_hd"), new Rectangle(0, 0, 800, 1280), Rectangle.Empty, new Rectangle(0, 1280, 800, 652), new Point(0, 1099));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_01_hd"), new Rectangle(1, 1, 234, 243), new Point(139, 220), new Point(514, 514));
        }
    }

    class FabricBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_02_hd"), new Rectangle(0, 0, 800, 1280), new Rectangle(800, 0, 594, 1280), new Rectangle(0, 1280, 800, 610), new Point(608, 1048));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_02_hd"), new Rectangle(1, 1, 291, 328), new Point(110, 126), new Point(514, 514));
        }
    }

    class FoilBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_03_hd"), new Rectangle(0, 0, 800, 1280), new Rectangle(800, 0, 448, 1280), new Rectangle(0, 1280, 800, 670), new Point(631, 1032));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_03_hd"), new Rectangle(1, 1, 302, 250), new Point(107, 166), new Point(514, 514));
        }
    }

    class MagicBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_04_hd"), new Rectangle(0, 0, 800, 1280), new Rectangle(800, 0, 335, 1280), new Rectangle(0, 1280, 800, 534), new Point(706, 1031));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_04_hd"), new Rectangle(1, 1, 325, 396), new Point(91, 87), new Point(514, 514));
        }
    }

    class ValentineBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_05_hd"), new Rectangle(0, 0, 800, 1280), Rectangle.Empty, Rectangle.Empty, Point.Zero);
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_05_hd"), new Rectangle(1, 1, 300, 265), new Point(108, 193), new Point(514, 514));
        }
    }

    class GiftBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_06_hd"), new Rectangle(0, 0, 800, 1280), new Rectangle(800, 0, 563, 1280), new Rectangle(0, 1280, 800, 538), new Point(591, 1055));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_06_hd"), new Rectangle(1, 1, 252, 269), new Point(122, 190), new Point(514, 514));
        }
    }

    class CosmicBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_07_hd"), new Rectangle(0, 0, 800, 1280), new Rectangle(800, 0, 421, 1280), new Rectangle(1221, 0, 800, 786), new Point(623, 887));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_07_hd"), new Rectangle(1, 1, 263, 268), new Point(123, 196), new Point(514, 514));
        }
    }

    class ToyBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_08_hd"), new Rectangle(0, 0, 800, 1280), new Rectangle(800, 0, 600, 1280), Rectangle.Empty, new Point(504, 0));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_08_hd"), new Rectangle(1, 1, 281, 269), new Point(119, 194), new Point(514, 514));
        }
    }

    class ToolBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_09_hd"), new Rectangle(0, 0, 800, 1280), new Rectangle(800, 0, 627, 1280), new Rectangle(0, 1280, 800, 763), new Point(601, 873));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_09_hd"), new Rectangle(1, 1, 318, 270), new Point(97, 188), new Point(514, 514));
        }
    }

    class BuzzBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_10_hd"), new Rectangle(0, 0, 800, 1280), Rectangle.Empty, new Rectangle(0, 1280, 800, 517), new Point(0, 1099));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_10_hd"), new Rectangle(1, 1, 220, 130), new Point(145, 316), new Point(514, 514));
        }
    }

    class DJBox : Box
    {
        public override void Load(ContentManager content)
        {
            Background = new Background(content.Load<Texture2D>("bgr_11_hd"), new Rectangle(0, 0, 800, 1280), new Rectangle(800, 0, 431, 1280), new Rectangle(0, 1280, 800, 659), new Point(633, 943));
            Platform = new PlatformSprite(content.Load<Texture2D>("char_support_11_hd"), new Rectangle(1, 1, 316, 270), new Point(88, 179), new Point(514, 514));
        }
    }
}
