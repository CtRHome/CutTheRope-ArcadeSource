using Microsoft.Xna.Framework;
using System;

namespace CTR_MonoGame
{
    static class Util
    {
        public const bool DebugDraw = false;

        public static readonly bool OnDevice = Environment.OSVersion.Platform == PlatformID.Unix;

        public static Random R = new Random();

        public static bool LineInLine(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            Vector2 DP, QA, QB;
            float d, la, lb;

            DP.X = p3.X - p1.X + p4.X - p2.X;
            DP.Y = p3.Y - p1.Y + p4.Y - p2.Y;
            QA.X = p2.X - p1.X;
            QA.Y = p2.Y - p1.Y;
            QB.X = p4.X - p3.X;
            QB.Y = p4.Y - p3.Y;

            d = QA.Y * QB.X - QB.Y * QA.X;
            la = QB.X * DP.Y - QB.Y * DP.X;
            lb = QA.X * DP.Y - QA.Y * DP.X;

            return (Math.Abs(la) <= Math.Abs(d) && Math.Abs(lb) <= Math.Abs(d));
        }

        public static bool LineInRect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            Vector2 pa = new Vector2(p3.X, p4.Y);
            Vector2 pb = new Vector2(p4.X, p3.Y);

            return LineInLine(p1, p2, p3, pa) ||
                LineInLine(p1, p2, p3, pb) ||
                LineInLine(p1, p2, p4, pa) ||
                LineInLine(p1, p2, p4, pb);
        }

        internal static Vector2 RotateVector(Vector2 v, float angle, Vector2 center)
        {
            Vector2 res = v;
            res.X -= center.X;
            res.Y -= center.Y;

            res = RotateVector(res, angle);

            res.X += center.X;
            res.Y += center.Y;

            return res;

        }

        public static float IntToFrac(int val)
        {
            if (val > 0)
            {
                return val;
            }
            else if (val > -7)
            {
                return 1f / (float)(2 - val);
            }
            else
            {
                return 0;
            }
        }

        internal static Vector2 RotateVector(Vector2 v, float angle)
        {
            Vector2 res;
            float cosA = (float)Math.Cos(angle);
            float sinA = (float)Math.Sin(angle);

            res.X = v.X * cosA - v.Y * sinA;
            res.Y = v.X * sinA + v.Y * cosA;
            return res;
        }

        internal static bool LineInCircle(Vector2 p1, Vector2 p2, Vector2 center, float r)
        {
            Vector2 v = center - p1;
            float pLength = Vector2.Dot(v, Vector2.Normalize(p2 - p1));

            Vector2 closestPt;
            if (pLength < 0)
            {
                closestPt = p1;
            }
            else if (pLength * pLength > (p2 - p1).LengthSquared())
            {
                closestPt = p2;
            }
            else
            {
                closestPt = pLength * Vector2.Normalize(p2 - p1) + p1;
            }

            return (closestPt - center).LengthSquared() <= r * r;
        }

        internal static float RandomAngle()
        {
            return (float)(R.NextDouble() * 2 * Math.PI);
        }

        public static Vector2 RandomOnUnitCircle()
        {
            float theta = RandomAngle();
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
        }

        public static Vector2 RandomInsideUnitCircle()
        {
            return RandomOnUnitCircle() * (float)R.NextDouble();
        }

        public static int SelectRandom(params int[] possibilities)
        {
            return possibilities[R.Next(possibilities.Length)];
        }
    }
}
