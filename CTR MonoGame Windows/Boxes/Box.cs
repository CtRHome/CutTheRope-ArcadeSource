using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    abstract class Box
    {
        public Background Background
        {
            get;
            set;
        }

        public Sprite Platform
        {
            get;
            set;
        }

        public abstract void Load(ContentManager content);

        public static Box GetBox(int boxNumber)
        {
            switch (boxNumber)
            {
                case 1:
                default:
                    return new CardboardBox();
                case 2:
                    return new FabricBox();
                case 3:
                    return new FoilBox();
                case 4:
                    return new MagicBox();
                case 5:
                    return new ValentineBox();
                case 6:
                    return new GiftBox();
                case 7:
                    return new CosmicBox();
                case 8:
                    return new ToyBox();
                case 9:
                    return new ToolBox();
                case 10:
                    return new BuzzBox();
                case 11:
                    return new DJBox();
            }
        }
    }
}
