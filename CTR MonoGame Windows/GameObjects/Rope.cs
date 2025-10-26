using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tao.Sdl;

namespace CTR_MonoGame
{
    class Rope : GameObject
    {
        // Candy Mass = 1
        // RopeSegment Mass = 0.02
        public const float SegmentLength = 30 * SingleLevel.SCALE;

        List<RopeSegment> segments;
        int cutAt;
        float cutTime;
        Candy attachedCandy;

        public Candy AttachedCandy
        {
            get { return attachedCandy; }
        }

        public bool HasBeenCut
        {
            get { return cutAt >= 0; }
        }

        int relaxed;
        bool rotatingCandy;
        float lastCandyAngle;

        public List<Vector2> DrawPts;

        SoundFX[] ropeBreakSounds;

        public Rope(Vector2 start, Vector2 end, Candy candy, float length)
        {
            DrawPts = new List<Vector2>();
            segments = new List<RopeSegment>();
            segments.Add(new RopeSegment(start));
            int count = (int)(length / SegmentLength) + 2;
            Vector2 offset = (end - start) / count;
            float len = 0;
            int c = 1;
            while (len < length)
            {
                float tL = Math.Min(SegmentLength, length - len);
                segments.Add(new RopeSegment(start + c * offset, tL));
                len += tL;
                c++;
            }
            for (int i = 1; i < count; i++)
            {
                //segments.Add(new RopeSegment(start + i * offset));
            }
            segments.Add(candy.Physics);
            position = start;
            this.attachedCandy = candy;
            cutAt = -1;
            ropeBreakSounds = new SoundFX[] {
                new SoundFX("Content/rope_bleak_1.ogg"),
                new SoundFX("Content/rope_bleak_2.ogg"),
                new SoundFX("Content/rope_bleak_3.ogg"),
                new SoundFX("Content/rope_bleak_4.ogg")
            };
        }

        public void Cut(int at)
        {
            if (cutAt < 0 && at >= 0 && at < segments.Count)
            {
                cutAt = at;
                for (int i = 0; i < segments.Count - 1; i++)
                {
                    segments[i].mass = 0.0001f;
                }
                cutTime = 2;
                ropeBreakSounds[relaxed].Play();
                attachedCandy = null;
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, GlobalState state)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (cutTime > 0)
            {
                cutTime -= elapsed;
            }

            // Gravity
            segments[0].position = position;
            for (int i = 1; i < segments.Count - 1; i++)
            {
                Vector2 a = state.Gravity * segments[i].mass * elapsed;
                Vector2 delt = segments[i].position - segments[i].lastPos + a * elapsed / segments[i].mass;
                segments[i].lastPos = segments[i].position;
                segments[i].position += delt;
            }

            // Relaxation
            for (int j = 0; j < 30; j++)
            {
                for (int i = 1; i < segments.Count; i++)
                {
                    if (i == cutAt)
                    {
                        continue;
                    }
                    Vector2 delta = segments[i - 1].position - segments[i].position;
                    if (delta == Vector2.Zero)
                    {
                        delta = Vector2.One;
                    }

                    float deltaLength = delta.Length();
                    float restLength = segments[i].length;

                    float diff = (deltaLength - restLength) / (Math.Max(1.0f, deltaLength) * (1.0f / segments[i].mass + 1.0f / segments[i - 1].mass));

                    if (i > 1)
                    {
                        Vector2 other = delta;
                        other *= diff / segments[i - 1].mass;
                        segments[i - 1].position -= other;
                    }

                    delta *= diff / segments[i].mass;
                    segments[i].position += delta;
                }
            }

            if (relaxed > 0)
            {
                if (rotatingCandy)
                {
                    float angle = GetAngle(segments[segments.Count - 1].position - segments[0].position);
                    if (attachedCandy != null)
                    {
                        attachedCandy.RopeRotate(angle - lastCandyAngle);
                    }
                    lastCandyAngle = angle;
                }
                else
                {
                    lastCandyAngle = GetAngle(segments[segments.Count - 1].position - segments[0].position);
                    rotatingCandy = true;
                }
            }
            else
            {
                rotatingCandy = false;
            }
        }

        private float GetAngle(Vector2 v)
        {
            return (float)Math.Atan2(v.Y, v.X);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            if (cutAt < 0)
            {
                DrawSection(segments, cameraPosition);
            }
            else if(cutAt < segments.Count)
            {
                DrawSection(segments.GetRange(0, cutAt), cameraPosition);
                DrawSection(segments.GetRange(cutAt, segments.Count - cutAt), cameraPosition);
            }
        }

        private void DrawSection(List<RopeSegment> section, Vector2 cameraPosition)
        {
            if (section.Count < 2)
                return;
            float alpha = 1;  // TODO: blink white on cut, fade after cut
            if (cutAt >= 0 && cutTime < 1.95f)
            {
                alpha = cutTime / 1.95f;
            }

            Vector4 c1 = new Vector4(95.0f / 200.0f, 61.0f / 200.0f, 37.0f / 200.0f, alpha);
            Vector4 d1 = new Vector4(95.0f / 500.0f, 61.0f / 500.0f, 37.0f / 500.0f, alpha);

            Vector4 c2 = new Vector4(152.0f / 225.0f, 99.0f / 225.0f, 62.0f / 225.0f, alpha);
            Vector4 d2 = new Vector4(152.0f / 500.0f, 99.0f / 500.0f, 62.0f / 500.0f, alpha);

            float stretchFactor = (segments[1].position - segments[0].position).Length();

            if (stretchFactor <= SegmentLength + 0.3)
            {
                relaxed = 0;
            }
            else if (stretchFactor <= SegmentLength + 1.0)
            {
                relaxed = 1;
            }
            else if (stretchFactor <= SegmentLength + 4.0)
            {
                relaxed = 2;
            }
            else
            {
                relaxed = 3;
            }

            if (stretchFactor > SegmentLength + 7.0)
            {
                float f = stretchFactor / SegmentLength * 2.0f;
                d1.X *= f;
                d2.X *= f;
            }

            bool useC1 = false;
            int numVertices = (section.Count - 1) * 3;
            float[] glVertices = new float[numVertices * 2];

            float step = 1.0f / numVertices;
            float a = 0.0f;
            int c = 0;
            int totalSegments = 0;

            Vector4 b1 = d1;
            Vector4 b2 = d2;

            float b1rf = (c1.X - d1.X) / (numVertices - 1);
            float b1gf = (c1.Y - d1.Y) / (numVertices - 1);
            float b1bf = (c1.Z - d1.Z) / (numVertices - 1);
            float b2rf = (c2.X - d2.X) / (numVertices - 1);
            float b2gf = (c2.Y - d2.Y) / (numVertices - 1);
            float b2bf = (c2.Z - d2.Z) / (numVertices - 1);

            Vector2? p1 = null;
            Vector2? p2 = null;

            DrawPts.Clear();

            while (true)
            {
                if (a > 0.99) a = 1.0f;
                //if (section.Count < 3) break;
                Vector2 p = calcPathBezier(section, a) - cameraPosition;
                glVertices[c++] = p.X;
                glVertices[c++] = p.Y;

                DrawPts.Add(p + cameraPosition);

                if (c >= 3 * 2 || a >= 1.0)
                {
                    Vector4 cc;

                    if (cutTime > 1.95f) // TODO: blink white on cut
                    {
                        cc = Color.White.ToVector4();
                    }
                    else if (useC1)
                    {
                        cc = b1;
                    }
                    else
                    {
                        cc = b2;
                    }

                    int count = c >> 1;
                    for (int i = 0; i < count - 1; i++)
                    {
            GLDrawer.DrawAttachedAntialiasedLine(new Vector2(glVertices[i * 2], glVertices[i * 2 + 1]),
                                   new Vector2(glVertices[i * 2 + 2], glVertices[i * 2 + 3]), 7f, new Color(cc), ref p1, ref p2);
                    }

                    glVertices[0] = glVertices[c - 2];
                    glVertices[1] = glVertices[c - 1];
                    c = 2;
                    useC1 = !useC1;
                    totalSegments++;

                    b1.X += b1rf * (count - 1);
                    b1.Y += b1gf * (count - 1);
                    b1.Z += b1bf * (count - 1);
                    b2.X += b2rf * (count - 1);
                    b2.Y += b2gf * (count - 1);
                    b2.Z += b2bf * (count - 1);
                }

                if (a >= 1.0)
                {
                    break;
                }

                a += step;
            }
        }

        private Vector2 calcPathBezier(List<RopeSegment> segments, float a)
        {
            List<RopeSegment> v = new List<RopeSegment>();
            int i;
            Vector2 res = Vector2.Zero;

            if (segments.Count > 2)
            {
                for (i = 0; i < segments.Count - 1; i++)
                {
                    v.Add(new RopeSegment(calc2PointBezier(segments[i], segments[i + 1], a)));
                }
                res = calcPathBezier(v, a);
            }
            else if (segments.Count == 2)
            {
                res = calc2PointBezier(segments[0], segments[1], a);
            }

            return res;
        }

        private Vector2 calc2PointBezier(RopeSegment a, RopeSegment b, float delta)
        {
            float d1;
            Vector2 res = new Vector2();
            d1 = 1.0f - delta;
            res.X = a.position.X * d1 + b.position.X * delta;
            res.Y = a.position.Y * d1 + b.position.Y * delta;

            return res;
        }

        internal void CutAtEnd()
        {
            Cut(segments.Count - 2);
            segments.RemoveAt(segments.Count - 1);
        }

        internal void ApplyCut(Cut cut)
        {
            if (cutAt >= 0)
            {
                return;
            }
            for (int i = 1; i < segments.Count; i++)
            {
                if (Util.LineInLine(cut.start, cut.end, segments[i-1].position, segments[i].position))
                {
                    Cut(i);
                }
            }
        }

        internal void SetAnchorPos(Vector2 position)
        {
            this.position = position;
        }

        internal void SetAttachedCandy(Candy c)
        {
            attachedCandy = c;
            segments.RemoveAt(segments.Count - 1);
            segments.Add(c.Physics);
        }

        internal float Length()
        {
            float rVal = 0;
            foreach (RopeSegment segment in segments)
            {
                rVal += segment.length;
            }
            return rVal;
        }

        internal void Extend(float a)
        {
            if (a > 0)
            {
                float toInsert = a;
                while (toInsert > 0)
                {
                    if (SegmentLength - segments[segments.Count - 2].length >= toInsert)
                    {
                        segments[segments.Count - 2].length += toInsert;
                        toInsert = 0;
                    }
                    else
                    {
                        toInsert -= SegmentLength - segments[segments.Count - 2].length;
                        segments[segments.Count - 2].length = SegmentLength;
                        segments.Insert(segments.Count - 1, new RopeSegment(segments[segments.Count - 2].position, toInsert));
                        toInsert = 0;
                    }
                }
            }
            else
            {
                float toRemove = -a;
                while (toRemove > 0 && segments.Count > 3)
                {
                    if (segments[segments.Count - 2].length > toRemove)
                    {
                        segments[segments.Count - 2].length -= toRemove;
                        toRemove = 0;
                    }
                    else
                    {
                        toRemove -= segments[segments.Count - 2].length;
                        segments.RemoveAt(segments.Count - 2);
                    }
                }
            }
        }
    }

    class RopeSegment
    {
        public Vector2 position, lastPos;
        public float length, mass;

        public RopeSegment(Vector2 position)
            : this(position, Rope.SegmentLength)
        {}

        public RopeSegment(Vector2 position, float length)
            :this(position, length, 0.02f)
        {}

        public RopeSegment(Vector2 position, float length, float mass)
        {
            this.position = position;
            lastPos = position;
            this.length = length;
            this.mass = mass;
        }

        public override string ToString()
        {
            return position.ToString();
        }
    }
}
