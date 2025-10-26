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
    class Candy : GameObject
    {
        Vector2[] debugCircleVertices;


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

        bool ropeRotating;
        float ropeRotateSpeed;
        CandyBlinkSprite blinkSprite;
        CandyBubbleSprite bubbleSprite;
        public bool Half
        {
            protected set;
            get;
        }
        float gotTimer;
        Vector2 gotPosition;
        Bubble bubble;
        public bool HasBubble
        {
            get { return bubble != null; }
        }

        public bool Spidered
        {
            get;
            protected set;
        }

        Vector2[] bitPositions, bitSpeeds;
        float[] bitRotations, bitRotSpeeds;
        bool broken;
        SoundFX bubblePopSound, bubbleGetSound, candyBreakSound, candyLinkSound;

        public Candy(ContentManager content, Vector2 position, bool half, bool leftHalf)
        {
            this.position = position;
            Half = half;
            Physics = new RopeSegment(position, Rope.SegmentLength, 1);
            sprite = new CandySprite(content, half, leftHalf);
            blinkSprite = new CandyBlinkSprite(content);
            blinkSprite.SetAnimation(CandyBlinkSprite.Animations.CANDY_BLINK_INITIAL);
            bubbleSprite = new CandyBubbleSprite(content);
            if (Util.DebugDraw)
                calcCircle();

            bubblePopSound = new SoundFX("Content/bubble_break.ogg");
            bubbleGetSound = new SoundFX("Content/bubble.ogg");
            candyBreakSound = new SoundFX("Content/candy_break.ogg");
            candyLinkSound = new SoundFX("candy_link");
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, GlobalState state)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 a = state.Gravity * elapsed;
            Vector2 delt = Physics.position - Physics.lastPos + a * elapsed;
            Physics.lastPos = Physics.position;
            Physics.position += delt;

            Velocity = delt / elapsed;

            if (HasBubble)
            {
                float yImpulse = -48.0f;
                float rd = 20.0f;

                Physics.position -= delt / rd;
                Physics.position += Vector2.UnitY * yImpulse * Math.Sign(state.Gravity.Y) * elapsed;
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

            if (ropeRotating)
            {
                rotation += ropeRotateSpeed;
                ropeRotating = false;
            }
            else
            {
                ropeRotateSpeed = 0.98f * ropeRotateSpeed;
                rotation += Math.Min(5, Math.Abs(ropeRotateSpeed)) * Math.Sign(ropeRotateSpeed);
            }


            blinkSprite.Update(gameTime);
            bubbleSprite.Update(gameTime);
            if (Util.DebugDraw)
                calcCircle();
        }

        public void CandyGot()
        {
            gotTimer = 0.25f;
            gotPosition = position;
            PopBubble();
        }

        public void ReflectLR()
        {
            Physics.lastPos.X += 2 * (Physics.position - Physics.lastPos).X;
        }

        public void UpdateGot(GameTime gameTime, Vector2 omNomPos)
        {
            gotTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (gotTimer < 0)
            {
                gotTimer = 0;
            }
            (sprite as CandySprite).Alpha = 0.71f * gotTimer * 4;
            position = Physics.position = omNomPos + (gotPosition - omNomPos) * gotTimer * 4;
        }

        public void Blink()
        {
            blinkSprite.SetAnimation(CandyBlinkSprite.Animations.CANDY_BLINK_STAR);
        }

        internal void RopeRotate(float p)
        {
            if (!ropeRotating)
            {
                ropeRotateSpeed = 0;
            }
            ropeRotating = true;
            ropeRotateSpeed += p;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
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
            else if ((sprite as CandySprite).Alpha > 0)
            {
                base.Draw(sb, cameraPosition);
                blinkSprite.Draw(sb, position - cameraPosition, 0);
                if (HasBubble)
                {
                    bubbleSprite.Draw(sb, position - cameraPosition, 0);
                }
            }
            if (Util.DebugDraw)
            {
                sb.End();
                for (int i = 0; i < debugCircleVertices.Length - 1; i++)
                {
                    GLDrawer.DrawAntialiasedLine(debugCircleVertices[i], debugCircleVertices[i + 1], 1, Color.Yellow);
                }
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            }
        }

        public override void DrawMiniMap(SpriteBatch sb, int levelY)
        {
            if (!broken && (sprite as CandySprite).Alpha > 0)
            {
                base.DrawMiniMap(sb, levelY);
            }
        }

        internal void ActivateBubble(Bubble b)
        {
            PopBubble();
            bubble = b;
            bubbleGetSound.Play();
        }

        public void PopBubble()
        {
            if (bubble != null)
            {
                bubble.Pop(position);
                bubble = null;
                bubblePopSound.Play();
            }
        }

        private void calcCircle()
        {
             float radius = 14;
            int vertexCount = (int)(radius * SingleLevel.SCALE);

            if ((vertexCount % 2) != 0) vertexCount++;

            debugCircleVertices = new Vector2[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                debugCircleVertices[i] = position + (radius * SingleLevel.SCALE) * new Vector2((float)Math.Cos(i * 2 * Math.PI / vertexCount), (float)Math.Sin(i * 2 * Math.PI / vertexCount));
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
                candyBreakSound.Play();
            }
        }

        internal void JoinWith(Candy c)
        {
            Half = false;
            candyLinkSound.Play();
            if (bubble == null)
            {
                bubble = c.bubble;
            }
            blinkSprite.SetAnimation(CandyBlinkSprite.Animations.CANDY_PART_FX);
            (sprite as CandySprite).Merge();
        }

        internal void SetHalf(ContentManager content, bool half, bool leftHalf)
        {
            sprite = new CandySprite(content, half, leftHalf);
            Half = half;
        }

        internal void SpiderGot()
        {
            PopBubble();
            Spidered = true;
        }
    }
}
