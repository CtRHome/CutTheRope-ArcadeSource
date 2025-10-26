using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Linq;

namespace CTR_MonoGame
{
    class Mover
    {
        public Vector2 Position
        {
            get;
            protected set;
        }

        public bool Circular
        {
            get;
            protected set;
        }

        public float Rotation
        {
            get;
            protected set;
        }

        public Vector2 Target
        {
            get
            {
                return path[targetPoint];
            }
        }

        public int Count
        {
            get { return path.Length; }
        }

        public Vector2 this[int i]
        {
            get { return path[i]; }
        }

        Vector2[] path;
        float[] moveSpeed;
        float rotateSpeed;
        int targetPoint;
        Vector2 offset;
        bool reverse;
        float overrun;

        private Mover(Vector2 position, string pathString, float moveSpeed, float rotation, float rotateSpeed)
        {
            if (pathString.StartsWith("R"))
            {
                Circular = true;
                bool clockwise = pathString.StartsWith("RC");
                float rad = int.Parse(pathString.Substring(2)) * SingleLevel.SCALE;
                int pointsCount = (int)rad / 2;
                path = new Vector2[pointsCount];
                float k_increment = (float)(2.0f * Math.PI / pointsCount);
                if (!clockwise) k_increment = -k_increment;
                float theta = 0.0f;

                for (int i = 0; i < pointsCount; ++i)
                {
                    Vector2 n = new Vector2((float)(rad * Math.Cos(theta)), (float)(rad * Math.Sin(theta)));
                    path[i] = position + n;
                    theta += k_increment;
                }
            }
            else
            {
                string[] pathSteps = pathString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                path = new Vector2[pathSteps.Length / 2 + 1];
                path[0] = position;
                for (int i = 0; i < path.Length - 1; i++)
                {
                    path[i + 1] = path[0] + new Vector2(float.Parse(pathSteps[2 * i]), float.Parse(pathSteps[2 * i + 1])) * SingleLevel.SCALE;
                }
            }
            Position = path[0];
            this.moveSpeed = new float[path.Length];
            for (int i = 0; i < this.moveSpeed.Length; i++)
            {
                this.moveSpeed[i] = moveSpeed;
            }
            this.Rotation = rotation;
            this.rotateSpeed = rotateSpeed;
            targetPoint = 1;
            CalculateOffset();
        }

        private void CalculateOffset()
        {
            offset = Vector2.Normalize(path[targetPoint] - Position) * moveSpeed[targetPoint];
        }

        internal void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
                Vector2 target = path[targetPoint];
                bool switchPoint = false;

                if (Position != target)
                {

                    float rdelta = elapsed;
                    if (overrun != 0)
                    {
                        rdelta += overrun;
                        overrun = 0;
                    }

                    Position += offset * rdelta;

                    // check if we passed the target
                    if (!SameSign(offset.X, target.X - Position.X) || !SameSign(offset.Y, target.Y - Position.Y))
                    {
                        overrun = (Position - target).Length();
                        float olen = offset.Length();
                        // overrun in seconds
                        overrun = overrun / olen;
                        Position = target;
                        switchPoint = true;
                    }
                }
                else
                {
                    switchPoint = true;
                }

                if (switchPoint)
                {
                    if (reverse)
                    {
                        targetPoint--;
                        if (targetPoint < 0)
                        {
                            targetPoint = path.Length - 1;
                        }
                    }
                    else
                    {
                        targetPoint++;
                        if (targetPoint >= path.Length)
                        {
                            targetPoint = 0;
                        }
                    }

                    CalculateOffset();

                }
            

            if (rotateSpeed != 0)
            {
                Rotation += rotateSpeed * elapsed;
            }

        }
        
        private bool SameSign(float p, float p2)
        {
            return Math.Sign(p) == Math.Sign(p2);
        }


        public static Mover Parse(XElement xml, Vector2 position)
        {
            if (xml.Attribute("path") != null)
            {
                string path = xml.Attribute("path").Value;
                float moveSpeed = Parse(xml, "moveSpeed");
                float rotateSpeed = Deg2Rad(ParseRaw(xml, "rotateSpeed"));
                float rotation = Deg2Rad(ParseRaw(xml, "angle"));
                return new Mover(position, path, moveSpeed, rotation, rotateSpeed);
            }
            return null;
        }

        public static Mover Build(Vector2 position, string pathString, float moveSpeed)
        {
            return new Mover(position, pathString, moveSpeed, 0, 0);
        }

        private static float Deg2Rad(float p)
        {
            return (float)(p * Math.PI / 180.0f);
        }

        private static float Parse(XElement node, string attributeName)
        {
            if (node.Attribute(attributeName) != null)
            {
                return int.Parse(node.Attribute(attributeName).Value) * SingleLevel.SCALE;
            }
            return 0;
        }

        private static float ParseRaw(XElement node, string attributeName)
        {
            if (node.Attribute(attributeName) != null)
            {
                return int.Parse(node.Attribute(attributeName).Value);
            }
            return 0;
        }

        internal void ModifyHatRotation()
        {
            Rotation += (float)Math.PI / 2f;
        }
    }
}
