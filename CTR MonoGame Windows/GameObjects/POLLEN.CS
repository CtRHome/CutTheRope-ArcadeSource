using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class Pollen : GameObject
    {
        List<PData> grains;

        public Pollen(ContentManager content)
        {
            mover = null;
            this.position = Vector2.Zero;
            this.rotation = 0;
            sprite = new PollenSprite(content);
            grains = new List<PData>();
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (PData grain in grains)
            {
                if (grain.fadingOut)
                {
                    grain.Alpha -= elapsed;
                    if (grain.Alpha < 0.3f)
                    {
                        grain.fadingOut = false;
                    }
                }
                else
                {
                    grain.Alpha += elapsed;
                    if (grain.Alpha > 0.3f)
                    {
                        grain.fadingOut = true;
                    }
                }

                if (grain.shrinking)
                {
                    grain.Scale -= elapsed;
                    if (grain.Scale < grain.minScale)
                    {
                        grain.shrinking = false;
                    }
                }
                else
                {
                    grain.Scale += elapsed;
                    if (grain.Scale > grain.maxScale)
                    {
                        grain.shrinking = true;
                    }
                }
            }
        }

        public void AddPollen(Mover m)
        {
            for (int i = 0; i < m.Count - 1; i++)
            {
                if (!m.Circular || i % 2 == 0)
                {
                    AddPollen(m[i], m[i + 1]);
                }
            }

            if (m.Count > 2)
            {
                AddPollen(m[0], m[m.Count - 1]);
            }

        }

        private void AddPollen(Vector2 from, Vector2 to)
        {
            Vector2 v = to - from;
            float MIN_DIST = 20 * SingleLevel.SCALE;
            int count = (int)( v.Length() / MIN_DIST);
            v.Normalize();
            for (int i = 0; i <= count; i++)
            {
                Vector2 vn = from + v * i * MIN_DIST;
                vn += Util.RandomInsideUnitCircle() * 4;
                AddPollen(vn);
            }
        }

        private void AddPollen(Vector2 vn)
        {
            PData newGrain = new PData();
            newGrain.Location = vn;
            newGrain.Alpha = 0.3f + (float)Util.R.NextDouble() * 0.7f;

            float[] sizes = new float[] { 0.3f, 0.3f, 0.5f, 0.5f, 0.6f };
            float scale = sizes[Util.R.Next(sizes.Length)] * (1 + (float)Util.R.NextDouble() / 10);
            newGrain.minScale = scale;
            newGrain.maxScale = 1;

            newGrain.Scale = scale + (float)Util.R.NextDouble() * (1 - scale);
            newGrain.shrinking = true;

            grains.Add(newGrain);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            for (int i = 0; i < grains.Count; i++)
            {
                (sprite as PollenSprite).Alpha = grains[i].Alpha;
                (sprite as PollenSprite).Scale = grains[i].Scale;
                sprite.Draw(sb, grains[i].Location - cameraPosition, 0);
            }
        }

        class PData
        {
            public Vector2 Location;
            public float Alpha, Scale, minScale, maxScale;
            public bool fadingOut, shrinking;
        }
    }
}

