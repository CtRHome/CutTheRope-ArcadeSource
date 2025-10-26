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
    class VictoryCandy : GameObject
    {
        public RopeSegment Physics
        {
            get;
            protected set;
        }

        public Vector2 Velocity
        {
            get;
            protected set;
        }

        public bool Expired
        {
            get
            {
                if (!broken)
                {
                    return false;
                }
                for (int i = 0; i < 5; i++)
                {
                    if (bitPositions[i].Y < 1940)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        float rotateSpeed;

        Vector2[] bitPositions, bitSpeeds;
        float[] bitRotations, bitRotSpeeds;
        bool broken;

        public VictoryCandy(ContentManager content, Vector2 position)
        {
            this.position = position;
            sprite = new CandySprite(content);
            Vector2 target = new Vector2(540, 000);
            Physics = new RopeSegment(position, Rope.SegmentLength, 1);
            Physics.lastPos = Vector2.Normalize(position - target) * Util.R.Next(15, 20) + position;
            rotateSpeed = (0.5f - (float)Util.R.NextDouble()) * 0.2f;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, GlobalState state)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 a = state.Gravity * elapsed;
            Vector2 delt = Physics.position - Physics.lastPos + a * elapsed;
            Physics.lastPos = Physics.position;
            Physics.position += delt;

            Velocity = delt / elapsed;

            if (!broken && Velocity.Y > 400)
            {
                Break();
            }

            if (broken)
            {
                for (int i = 0; i < 5; i++)
                {
                    bitSpeeds[i] += a;
                    bitPositions[i] += bitSpeeds[i] * elapsed;
                    bitRotations[i] += bitRotSpeeds[i] * elapsed;
                }
            }
            else
            {
                position = Physics.position;
            }

            rotation += rotateSpeed;

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

                    //float a = Util.RandomAngle();
                    bitSpeeds[i] = Util.RandomInsideUnitCircle() * 400;// new Vector2((float)Math.Sin(a), (float)Math.Cos(a)) * 200;

                    bitRotSpeeds[i] = (float)(Util.R.NextDouble() - 0.5);
                }
            }
        }
    }
}
