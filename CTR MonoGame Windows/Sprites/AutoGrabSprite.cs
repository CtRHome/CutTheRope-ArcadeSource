using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class AutoGrabSprite : GrabSprite
    {
        public AutoGrabSprite(ContentManager content)
            : base(content.Load<Texture2D>("obj_hook_auto_hd"), "1,1,112,115,1,118,33,30", "56,49,95,95", new Point(220, 220))
        {}
    }
}
