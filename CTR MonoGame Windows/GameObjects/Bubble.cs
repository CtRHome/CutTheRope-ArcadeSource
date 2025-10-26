using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class Bubble : OnRecord
    {
        public bool Released
        {
            get { return (sprite as BubbleSprite).Released; }
        }

        BubblePopSprite bps;
        bool popped;
        Vector2 popPosition;

        public Bubble(ContentManager content, Vector2 position)
        {
            this.position = position;
            sprite = new BubbleSprite(content);
            bps = new BubblePopSprite(content);
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);
            if (popped)
            {
                bps.Update(gameTime);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            base.Draw(sb, cameraPosition);
            if (popped)
            {
                bps.Draw(sb, popPosition - cameraPosition, rotation);
            }
        }

        public void Release()
        {
            (sprite as BubbleSprite).Released = true;
        }

        public void Pop(Vector2 position)
        {
            popped = true;
            popPosition = position;
        }
    }
}
