using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CTR_MonoGame
{
    interface ITransitionable
    {
        void Draw(GameTime gameTime, SpriteBatch sb);

        void UpdateTransition(GameTime gameTime);
    }

    class TransitionManager
    {
        public bool Done
        {
            get;
            protected set;
        }

        public bool Phase2
        {
            get
            {
                return currentTransition.Phase2;
            }
        }


        Transition currentTransition;
        ConfettiTransition confetti;
        ShadeTransition shade;
        BonusTransition bonus;

        ITransitionable from, to;

        public TransitionManager(ContentManager content)
        {
            confetti = new ConfettiTransition(content);
            shade = new ShadeTransition(content);
            bonus = new BonusTransition(content);
        }

        public void Transition(ITransitionable from, ITransitionable to)
        {
            currentTransition = shade;
            currentTransition.Reset();
            Done = false;
            this.from = from;
            this.to = to;
        }

        public void BonusTransition(ITransitionable from, ITransitionable to)
        {
            currentTransition = bonus;
            currentTransition.Reset();
            Done = false;
            this.from = from;
            this.to = to;
        }

        public void Update(GameTime gameTime)
        {
            currentTransition.Update(gameTime);
            if (currentTransition.Phase2)
            {
                to.UpdateTransition(gameTime);
            }
            else
            {
                from.UpdateTransition(gameTime);
            }
            if (currentTransition.Done)
            {
                Done = true;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            if (currentTransition.Phase2)
            {
                to.Draw(gameTime, sb);
            }
            else
            {
                from.Draw(gameTime, sb);
            }
            currentTransition.Draw(gameTime, sb);
        }
    }

    abstract class Transition
    {
        public abstract bool Phase2 { get; }
        public abstract bool Done { get; }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch sb);

        public abstract void Reset();
    }

    class ShadeTransition : Transition
    {
        protected Texture2D shade;

        protected float time;

        protected const float TRANSITION_TIME = 0.75f;

        public override bool Done
        {
            get { return time > TRANSITION_TIME; }
        }

        public override bool Phase2
        {
            get { return time > TRANSITION_TIME / 2f; }
        }

        public ShadeTransition(ContentManager content)
        {
            shade = content.Load<Texture2D>("menu_drawings_bigpage_markers_hd");
        }

        public override void Reset()
        {
            time = 0;
        }

        public override void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            sb.Draw(shade, new Rectangle(0, 0, 1080, 1920), new Color(Color.White, 1f - Math.Abs(time - TRANSITION_TIME / 2f) * 2f / TRANSITION_TIME));
            sb.End();
        }
    }

    class ConfettiTransition : ShadeTransition
    {
        List<ConfettiSprite> confetti;
        List<Vector2> positions;
        List<float> speeds;

        public ConfettiTransition(ContentManager content)
            : base(content)
        {
            confetti = new List<ConfettiSprite>();
            positions = new List<Vector2>();
            for (int i = 0; i < 1080; i += 5)
            {
                confetti.Add(new ConfettiSprite(content));
                positions.Add(new Vector2(i, 10));
            }
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            speeds = new List<float>();
            Random R = new Random();
            for (int i = 0; i < positions.Count; i++)
            {
                positions[i] = new Vector2(5 * i, 10);
                speeds.Add((float)R.NextDouble() * 2000f / TRANSITION_TIME);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (ConfettiSprite item in confetti)
            {
                item.Update(gameTime);
                item.Alpha = Math.Min(2f - time, 1);
            }
            for (int i = 0; i < positions.Count; i++)
            {
                Vector2 v = positions[i];
                v.Y += speeds[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;
                positions[i] = v;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            sb.Draw(shade, new Rectangle(0, 0, 1080, 1920), new Color(Color.White, 1f - Math.Abs(time - TRANSITION_TIME / 2f) * 2f / TRANSITION_TIME));
            for (int i = 0; i < confetti.Count; i++)
            {
                confetti[i].Draw(sb, positions[i], 0);
            }
            sb.End();
        }
    }

    class BonusTransition : Transition
    {
        float age;
        BoxCoverSprite cover;
        LoadScreenSprite tapeL, tapeR, knife;
        Texture2D text, sparkle, sparkle2, fallSparkle, banner, shadow;

        public override bool Phase2
        {
            get { return true; }
        }

        public override bool Done
        {
            get { return age > 8.5f; }
        }

        public BonusTransition(ContentManager content)
        {
            cover = new BoxCoverSprite(content);
            tapeL = new LoadScreenSprite(content, LoadScreenSprite.Sign.TapeLeft);
            tapeR = new LoadScreenSprite(content, LoadScreenSprite.Sign.TapeRight);
            knife = new LoadScreenSprite(content, LoadScreenSprite.Sign.Knife);
            text = content.Load<Texture2D>("BonusCover/BONUS ROUND TEXT");
            sparkle = content.Load<Texture2D>("BonusCover/SWIRLING SPARKLES");
            sparkle2 = content.Load<Texture2D>("BonusCover/SWIRLING SPARKLES_1");
            fallSparkle = content.Load<Texture2D>("BonusCover/FALLING SPARKLES");
            banner = content.Load<Texture2D>("BonusCover/GET READY BANNER");
            shadow = content.Load<Texture2D>("BonusCover/GET READY BANNER SHADOW");
        }

        public override void Update(GameTime gameTime)
        {
            age += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));
            float sinT = (float)Math.Sin(gameTime.TotalRealTime.TotalSeconds);
            if (age < 2)
            {
                cover.DrawBox(sb, new Rectangle(0, 0, 540, 1920), false);
                cover.DrawBox(sb, new Rectangle(540, 0, 540, 1920), true);
                tapeL.Draw(sb, new Vector2(540, 960), 0);
                tapeR.Draw(sb, new Vector2(540, 960), 0);

                sb.Draw(shadow, new Vector2(540, 1400), null, Color.White, 0, Center(shadow), 1, SpriteEffects.None, 1);
                sb.Draw(banner, new Vector2(540, 1400), null, Color.White, 0, Center(shadow), 1, SpriteEffects.None, 1);

                sb.Draw(sparkle, new Vector2(540, 700), null, Color.White, (float)gameTime.TotalRealTime.TotalSeconds, Center(sparkle), 1, SpriteEffects.None, 1);
                sb.Draw(sparkle2, new Vector2(540, 700), null, Color.White, (float)gameTime.TotalRealTime.TotalSeconds * 2, Center(sparkle2), 1, SpriteEffects.None, 1);
                sb.Draw(fallSparkle, new Vector2(540, 960 + 1920 * SawTooth(gameTime.TotalRealTime.TotalSeconds)), null, Color.White, 0, Center(fallSparkle), 1, SpriteEffects.None, 1);
                sb.Draw(fallSparkle, new Vector2(540, -960 + 1920 * SawTooth(gameTime.TotalRealTime.TotalSeconds)), null, Color.White, 0, Center(fallSparkle), 1, SpriteEffects.None, 1);
                sb.Draw(text, new Vector2(540, 700), null, Color.White, 0, Center(text), 1 + (sinT * sinT / 15f), SpriteEffects.None, 1);
            }
            else if (age < 3)
            {
                float t = age - 2f;
                float t2 = t;
                float t3 = t;
                cover.DrawBox(sb, new Rectangle(0, 0, 540, 1920), false);
                cover.DrawBox(sb, new Rectangle(540, 0, 540, 1920), true);
                tapeL.Draw(sb, new Vector2(540, 960), 0);
                tapeR.Draw(sb, new Vector2(540, 960), 0);

                sb.Draw(shadow, new Vector2(540, 1400), null, Color.White, 0, Center(shadow), 1, SpriteEffects.None, 1);
                sb.Draw(banner, new Vector2(540, 1400), null, Color.White, 0, Center(shadow), 1, SpriteEffects.None, 1);

                sb.Draw(sparkle, new Vector2(540, 700 - 700 * t2), null, Color.White, (float)gameTime.TotalRealTime.TotalSeconds, Center(sparkle), 1, SpriteEffects.None, 1);
                sb.Draw(sparkle2, new Vector2(540, 700 - 700 * t3), null, Color.White, (float)gameTime.TotalRealTime.TotalSeconds * 2, Center(sparkle2), 1, SpriteEffects.None, 1);
                sb.Draw(fallSparkle, new Vector2(540, 960 + 1920 * SawTooth(gameTime.TotalRealTime.TotalSeconds)), null, Color.White, 0, Center(fallSparkle), 1, SpriteEffects.None, 1);
                sb.Draw(fallSparkle, new Vector2(540, -960 + 1920 * SawTooth(gameTime.TotalRealTime.TotalSeconds)), null, Color.White, 0, Center(fallSparkle), 1, SpriteEffects.None, 1);
                sb.Draw(text, new Vector2(540, 700 - 700 * t), null, Color.White, 0, Center(text), 1 + (sinT * sinT / 15f), SpriteEffects.None, 1);
            }
            else if (age < 4)
            {
                float t = age - 2f;
                float t2 = t;
                float t3 = t;

                float t4 = age - 3f;
                float it4 = 1 - t4;

                cover.DrawBox(sb, new Rectangle(0, 0, 540, 1920), false);
                cover.DrawBox(sb, new Rectangle(540, 0, 540, 1920), true);
                tapeL.Draw(sb, new Vector2(540, 960), 0);
                tapeR.Draw(sb, new Vector2(540, 960), 0);

                sb.Draw(shadow, new Vector2(540 + 1000 * t4, 1400), null, Color.White, t4, Center(shadow), 1, SpriteEffects.None, 1);
                sb.Draw(banner, new Vector2(540 + 1000 * t4, 1400 - 400 * t4), null, Color.White, t4, Center(shadow), 1 + t4 / 4f, SpriteEffects.None, 1);

                sb.Draw(sparkle, new Vector2(540, 700 - 700 * t2), null, Color.White, (float)gameTime.TotalRealTime.TotalSeconds, Center(sparkle), 1, SpriteEffects.None, 1);
                sb.Draw(sparkle2, new Vector2(540, 700 - 700 * t3), null, Color.White, (float)gameTime.TotalRealTime.TotalSeconds * 2, Center(sparkle2), 1, SpriteEffects.None, 1);
                sb.Draw(fallSparkle, new Vector2(540, 960 + 1920 * SawTooth(gameTime.TotalRealTime.TotalSeconds)), null, new Color(Color.White, it4), 0, Center(fallSparkle), 1, SpriteEffects.None, 1);
                sb.Draw(fallSparkle, new Vector2(540, -960 + 1920 * SawTooth(gameTime.TotalRealTime.TotalSeconds)), null, new Color(Color.White, it4), 0, Center(fallSparkle), 1, SpriteEffects.None, 1);
                sb.Draw(text, new Vector2(540, 700 - 700 * t), null, Color.White, 0, Center(text), 1 + (sinT * sinT / 15f), SpriteEffects.None, 1);
            }
            else if (age < 5)
            {
                float t4 = age - 3f;
                float it4 = 1 - t4;

                float t5 = (age - 4f) / 2f;

                cover.DrawBox(sb, new Rectangle(0, 0, 540, 1920), false);
                cover.DrawBox(sb, new Rectangle(540, 0, 540, 1920), true);
                tapeL.Draw(sb, new Vector2(540, 960), 0);
                tapeR.Draw(sb, new Vector2(540, 960), 0);

                sb.Draw(shadow, new Vector2(540 + 1000 * t4, 1400), null, Color.White, t4, Center(shadow), 1, SpriteEffects.None, 1);
                sb.Draw(banner, new Vector2(540 + 1000 * t4, 1400 - 400 * t4), null, Color.White, t4, Center(shadow), 1 + t4 / 4f, SpriteEffects.None, 1);

                knife.Draw(sb, new Vector2(540, -960 + 2880 * t5), 0);
            }
            else if (age < 6)
            {
                float t5 = (age - 4f) / 2f;

                cover.DrawBox(sb, new Rectangle(0, 0, 540, 1920), false);
                cover.DrawBox(sb, new Rectangle(540, 0, 540, 1920), true);
                tapeL.Draw(sb, new Vector2(540, 960), 0);
                tapeR.Draw(sb, new Vector2(540, 960), 0);

                knife.Draw(sb, new Vector2(540, -960 + 2880 * t5), 0);
            }
            else if (age < 8)
            {
                float t6 = (age - 6f) / 2f;
                float it6 = 1f - t6;

                cover.DrawBox(sb, new Rectangle(0, 0, (int)(540 * it6), 1920), false);
                cover.DrawBox(sb, new Rectangle(540 + (int)(540 * t6), 0, (int)(540 * it6), 1920), true);

                cover.DrawEdge(sb, new Rectangle((int)(540 * it6), 0, (int)(63 * t6), 1920), false);
                cover.DrawEdge(sb, new Rectangle(1080 - (int)(540 * it6) - (int)(63 * t6), 0, (int)(63 * t6), 1920), false);

                tapeL.Draw(sb, new Vector2(540 * it6, 960), 0);
                tapeR.Draw(sb, new Vector2(540 + 540 * t6, 960), 0);
            }
            else
            {
                float t6 = (age - 6f) / 2f;
                float it6 = 1f - t6;

                cover.DrawEdge(sb, new Rectangle((int)(540 * it6), 0, (int)(63 * t6), 1920), false);
                cover.DrawEdge(sb, new Rectangle(1080 - (int)(540 * it6) - (int)(63 * t6), 0, (int)(63 * t6), 1920), false);
            }
            sb.End();
        }

        public override void Reset()
        {
            age = 0;
        }

        private float SawTooth(double time)
        {
            return (float)(time - Math.Floor(time));
        }

        private Vector2 Center(Texture2D tex)
        {
            return new Vector2(tex.Width, tex.Height) / 2f;
        }
    }
}
