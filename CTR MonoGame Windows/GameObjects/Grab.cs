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
    class Grab : OnRecord
    {
        public Rope rope;
        float radius, radiusAlpha;
        SoundFX ropeGetSound, buzz;
        Spider spider;
        Vector2[] autoCircleVertices;
        float moveLength, moveOffset;
        bool moveVertical;
        int dragging;
        bool wheel;
        BeeSprite beeSprite;
        float beeAngle;

        Vector2 AdjustedPostion
        {
            get
            {
                if (moveLength < 0)
                {
                    return position;
                }
                else
                {
                    return position + moveOffset * (moveVertical ? Vector2.UnitY : Vector2.UnitX);
                }
            }
        }

        public Candy AttachedCandy
        {
            get
            {
                if (rope != null)
                {
                    return rope.AttachedCandy;
                }
                return null;
            }
        }

        public bool HasSpider
        {
            get { return spider != null; }
        }

        public bool SpiderGotCandy
        {
            get { return spider != null && spider.GotCandy; }
        }

        public Grab(ContentManager content, Mover m, Vector2 position, float moveLength, bool moveVertical, float moveOffset, float radius, float ropeLength, bool spider, bool wheel, Candy attachedCandy)
        {
            this.position = position;
            mover = m;
            // TODO: bees. bees? bees!
            ropeGetSound = new SoundFX("Content/rope_get.ogg");
            buzz = new SoundFX("buzz");
            if (mover != null)
            {
                beeSprite = new BeeSprite(content);
            }
            this.wheel = wheel;
            this.moveLength = moveLength;
            if (moveLength > 0)
            {
                sprite = new MovableGrabSprite(content, moveVertical, moveLength);
                this.moveVertical = moveVertical;
                this.moveOffset = moveOffset;
                this.position -= moveOffset * (moveVertical ? Vector2.UnitY : Vector2.UnitX);
                (sprite as MovableGrabSprite).SetGrabPosition(moveOffset);
            }
            else if (wheel)
            {
                sprite = new WheelGrabSprite(content);
            }
            else
            {
                if (radius <= 0)
                {
                    sprite = new GrabSprite(content);
                }
                else
                {
                    sprite = new AutoGrabSprite(content);
                }
            }
            if (radius <= 0)
            {
                rope = new Rope(AdjustedPostion, attachedCandy.Position, attachedCandy, ropeLength);
            }
            else
            {
                this.radius = radius;
                calcCircle();
                radiusAlpha = 1;
            }

            dragging = -1;

            if (spider)
            {
                this.spider = new Spider(content, position);
            }
			
			dragging = -1;
        }

        public override void RotatePosition(float angle, Vector2 center)
        {
            base.RotatePosition(angle, center);
            if (rope != null)
            {
                rope.SetAnchorPos(AdjustedPostion);
            }
            else
            {
                calcCircle();
            }
        }

        private void calcCircle()
        {
            int vertexCount = (int) Math.Max(16, radius / 2);

            if ((vertexCount % 2) != 0) vertexCount++;

            autoCircleVertices = new Vector2[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                autoCircleVertices[i] = AdjustedPostion + radius * new Vector2((float)Math.Cos(i * 2 * Math.PI / vertexCount), (float)Math.Sin(i * 2 * Math.PI / vertexCount));
            }
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);
            if (beeSprite != null)
            {
                GetBeeAngle((float)gameTime.ElapsedGameTime.TotalSeconds);
                beeSprite.Update(gameTime);
            }
            if (mover != null && rope == null)
            {
                calcCircle();
            }
            if (rope != null)
            {
                if (mover != null)
                {
                    rope.SetAnchorPos(AdjustedPostion);
                }
                rope.Update(gameTime, state);
                if (spider != null && !state.Camera.IgnoreTouches)
                {
                    if (!spider.Active)
                    {
                        spider.Activate(rope);
                    }
                    spider.Update(gameTime, state);
                }
                if (radiusAlpha > 0)
                {
                    radiusAlpha -= 1.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            else
            {
                AttachCandyIfClose(state.Candy);
                if (rope == null && state.Candy2 != null)
                {
                    AttachCandyIfClose(state.Candy2);
                }
            }
            if (wheel)
            {
                float len;
                if (rope != null)
                {
                    len = rope.Length() * 0.7f;
                }
                else
                {
                    len = 0f;
                }
                if (len == 0)
                {
                    (sprite as WheelGrabSprite).SetMiddleScale(0);
                }
                else
                {
                    (sprite as WheelGrabSprite).SetMiddleScale(Math.Max(0, Math.Min(1.2f, 1f - len / 700f)));
                }

                (sprite as WheelGrabSprite).SetHighlight(dragging >= 0);

                int wasDragging = dragging;
                dragging = -1;

                for (int i = Util.OnDevice ? 0 : -1; i < Input.TOUCH_COUNT; i++)
                {
                    if ((state.Input.TouchPos(i) + state.Camera.Position - AdjustedPostion).LengthSquared() < 30 * 30 * SingleLevel.SCALE * SingleLevel.SCALE)
                    {
                        if (state.Input.MouseJustClicked(i))
                        {
                            dragging = i;
                            state.Input.ConsumeClick(i);
                        }
                    }
                    if (wasDragging == i && state.Input.TouchDown(i))
                    {
                        dragging = i;
                    }
                    if (dragging == i && wasDragging == i)
                    {
                        Vector2 toCurrent = state.Input.TouchPos(i) + state.Camera.Position - Position;
                        Vector2 toLast = state.Input.LastTouchPos(i) + state.Camera.Position - Position;

                        float a = (float)(Math.Atan2(toCurrent.Y, toCurrent.X) - Math.Atan2(toLast.Y, toLast.X));
                        rotation += a;

                        a *= 180f / (float)Math.PI;

                        if (a != 0)
                        {
                            a = (a > 0) ? Math.Min(Math.Max(1.0f, a), 2.0f) : Math.Max(Math.Min(-1.0f, a), -2.0f);
                        }

                        a *= SingleLevel.SCALE;

                        if (rope != null)
                        {
                            if (a > 0)
                            {
                                if (len < 500.0 * SingleLevel.SCALE)
                                {
                                    rope.Extend(a);
                                }
                            }
                            else if (a != 0)
                            {
                                rope.Extend(a);
                            }
                        }
                    }
                }
            }
            if (moveLength > 0)
            {
                (sprite as MovableGrabSprite).SetGrabPosition(moveOffset);
                (sprite as MovableGrabSprite).SetHighlight(dragging >= 0);

                int wasDragging = dragging;
                dragging = -1;

                for (int i = Util.OnDevice ? 0 : -1; i < Input.TOUCH_COUNT; i++)
                {

                    if ((state.Input.TouchPos(i) + state.Camera.Position - AdjustedPostion).LengthSquared() < 30 * 30 * SingleLevel.SCALE * SingleLevel.SCALE)
                    {
                        if (state.Input.MouseJustClicked(i))
                        {
                            dragging = i;
                            state.Input.ConsumeClick(i);
                        }
                    }
                    if (wasDragging == i && state.Input.TouchDown(i))
                    {
                        dragging = i;
                    }
                    if (dragging == i && wasDragging == i)
                    {
                        if (moveVertical)
                        {
                            moveOffset += state.Input.TouchPos(i).Y - state.Input.LastTouchPos(i).Y;
                        }
                        else
                        {
                            moveOffset += state.Input.TouchPos(i).X - state.Input.LastTouchPos(i).X;
                        }
                        moveOffset = Math.Max(0, Math.Min(moveLength, moveOffset));
                        if (rope == null)
                        {
                            calcCircle();
                        }
                        else
                        {
                            rope.SetAnchorPos(AdjustedPostion);
                        }
                    }
                }
            }
        }

        private void AttachCandyIfClose(Candy c)
        {
            if (!c.Spidered && (c.Position - AdjustedPostion).LengthSquared() <= (radius + 15 * SingleLevel.SCALE) * (radius + 15 * SingleLevel.SCALE))
            {
                rope = new Rope(AdjustedPostion, c.Position, c, radius);
                ropeGetSound.Play();
                if (mover != null)
                {
                    buzz.Play();
                }
            }
        }

        public override void UpdateBounds()
        {
            if (rope != null)
            {
                rope.SetAnchorPos(AdjustedPostion);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            sprite.Draw(sb, position - cameraPosition, rotation);
            sb.End();
            
            // Begin spritebatch with scaling for GLDrawer calls
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            
            if (radiusAlpha > 0)
            {
                Color c = new Color(0.2f, 0.5f, 0.9f, radiusAlpha);
                for (int i = 0; i < autoCircleVertices.Length; i += 2)
                {
                    GLDrawer.DrawAntialiasedLine(autoCircleVertices[i] - cameraPosition, autoCircleVertices[i + 1] - cameraPosition, 3, c);
                }
            }
            if (rope != null)
            {
                rope.Draw(sb, cameraPosition);
            }
            sb.End();
            
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            (sprite as GrabSprite).DrawTop(sb, position - cameraPosition, rotation);
            if (spider != null)
            {
                spider.Draw(sb, cameraPosition);
            }
            if (mover != null)
            {
                beeSprite.Draw(sb, position - cameraPosition, beeAngle);
            }
        }

        private void GetBeeAngle(float elapsed)
        {
            Vector2 v = mover.Target - mover.Position;

            float a = 0;
            if (Math.Abs(v.X) > 15 * SingleLevel.SCALE)
            {
                const float MAX_ANGLE = 0.17453292f;
                a = (v.X > 0) ? MAX_ANGLE : -MAX_ANGLE;
            }
            beeAngle += 1.047197551f * elapsed * Math.Sign(a - beeAngle);
        }

        public override void DrawMiniMap(SpriteBatch sb, int levelY)
        {
            base.DrawMiniMap(sb, levelY);
            sb.End();
            if (rope != null && rope.AttachedCandy != null)
            {
                Vector2 pos1 = GetMiniPos(position, levelY);
                Vector2 pos2 = GetMiniPos(rope.AttachedCandy.Position, levelY);
                GLDrawer.DrawAntialiasedLine(pos1, pos2, 2, new Color(new Vector4(95.0f / 200.0f, 61.0f / 200.0f, 37.0f / 200.0f, 1f)));
            }
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            //(sprite as GrabSprite).DrawTop(sb, position - cameraPosition, rotation);
            if (spider != null)
            {
                spider.DrawMiniMap(sb, levelY);
            }
        }

        internal void ReleaseAtEnd()
        {
            if (rope != null)
            {
                rope.CutAtEnd();
            }
        }

        internal void ApplyCut(Cut cut)
        {
            if (rope != null && dragging < 0)
            {
                rope.ApplyCut(cut);
            }
        }

        internal void GiveSpiderCandy(Candy c, GlobalState s)
        {
            if (spider != null)
            {
                spider.GiveCandy(c, s);
            }
        }

        internal void SetAttachedCandy(Candy candy)
        {
            if (rope != null && !candy.Spidered)
            {
                rope.SetAttachedCandy(candy);
            }
        }
    }
}
