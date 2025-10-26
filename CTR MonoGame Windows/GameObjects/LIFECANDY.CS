using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tao.Sdl;

namespace CTR_MonoGame
{
    class LifeCandy : GameObject
    {
        Vector2[] bitPositions, bitSpeeds;
        float[] bitRotations, bitRotSpeeds;
        bool broken;

        public LifeCandy(ContentManager content, Vector2 position)
        {
            this.position = position;
            sprite = new CandySprite(content);
            (sprite as CandySprite).Alpha = 0.5f;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, GlobalState state)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (broken)
            {
                for (int i = 0; i < 5; i++)
                {
                    bitPositions[i] += bitSpeeds[i] * elapsed;
                    bitRotations[i] += bitRotSpeeds[i] * elapsed;
                }
                (sprite as CandySprite).Alpha -= 1f / 60f;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            if ((sprite as CandySprite).Alpha > 0)
            {
                if (broken)
                {
                    CandySprite cs = sprite as CandySprite;
                    for (int i = 0; i < 5; i++)
                    {
                        cs.Bit = i;
                        sprite.Draw(sb, bitPositions[i] - cameraPosition, bitRotations[i]);
                    }
                }
                else
                {
                    base.Draw(sb, cameraPosition);
                }
            }
        }

        public void Break()
        {
            if (!broken)
            {
                broken = true;
                bitPositions = new Vector2[5];
                bitSpeeds = new Vector2[5];
                bitRotations = new float[5];
                bitRotSpeeds = new float[5];
                for (int i = 0; i < 5; i++)
                {
                    bitPositions[i] = position;
                    bitRotations[i] = Util.RandomAngle();

                    float a = Util.RandomAngle();
                    bitSpeeds[i] = new Vector2((float)Math.Sin(a), (float)Math.Cos(a)) * 30;

                    bitRotSpeeds[i] = (float)(Util.R.NextDouble() - 0.5);
                }
            }
        }

        public void Reset()
        {
            broken = false;
            (sprite as CandySprite).Alpha = 0.5f;
            (sprite as CandySprite).Bit = -1;
        }
    }
}
