using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class SkipButton : Button
    {		
        public SkipButton(ContentManager content)
            :base(new Vector2(620, 1750), 506, 186)
        {
            sprite = new SkipButtonSprite(content);
        }
		
		public SkipButton(ContentManager content, float alpha)
			:this(content)
		{
			(sprite as SkipButtonSprite).Alpha = alpha;
		}
    }
}
