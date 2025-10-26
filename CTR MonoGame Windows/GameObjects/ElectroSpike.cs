using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class ElectroSpike : Spike
    {
        bool powered;
        float timer, onTime, offTime;
        SoundFX zap;

        public ElectroSpike(ContentManager content, Mover m, Vector2 position, float rotation, float initialDelay, float onTime, float offTime)
            :base(content, m, position, rotation)
        {
            sprite = new ElectricSpikeSprite(content);
            UpdateBounds();
            this.onTime = onTime;
            this.offTime = offTime;
            timer = initialDelay;
            zap = new SoundFX("electric");
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer < 0)
            {
                powered = !powered;
                (sprite as ElectricSpikeSprite).SetPower(powered);
                if (powered)
                {
                    zap.PlayFor((int)(onTime * 1000f));
                }
                timer += powered ? onTime : offTime;
            }
        }

        public override bool IntersectsCandy(Vector2 candyPos)
        {
            return powered && base.IntersectsCandy(candyPos);
        }

        public override void UpdateBounds()
        {
            float pWidth = (sprite as ElectricSpikeSprite).Width / 2f;

            t1.X = position.X - pWidth;
            t2.X = position.X + pWidth;
            t1.Y = t2.Y = position.Y - 10.0f * SingleLevel.SCALE / 2.0f;

            b1.X = t1.X;
            b2.X = t2.X;
            b1.Y = b2.Y = position.Y + 10.0f * SingleLevel.SCALE / 2.0f;


            t1 = Util.RotateVector(t1, rotation, position);
            t2 = Util.RotateVector(t2, rotation, position);
            b1 = Util.RotateVector(b1, rotation, position);
            b2 = Util.RotateVector(b2, rotation, position);
        }
    }
}
