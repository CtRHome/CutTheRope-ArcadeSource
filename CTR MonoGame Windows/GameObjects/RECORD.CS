using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class Record : GameObject
    {
        List<OnRecord> onDisc;
        List<Record> recordGroup;
        public bool Active
        {
            get;
            protected set;
        }
        float size;
        int dragging;
        bool twoHandles;
        Vector2 HandlePosition
        {
            get { return Util.RotateVector(Vector2.UnitY * size, rotation) + position; }
        }
        Vector2 Handle2Position
        {
            get { return Util.RotateVector(Vector2.UnitY * -size, rotation) + position; }
        }
        SoundFX zip, zah;
        int soundPlaying;

        public Record(ContentManager content, Vector2 position, float size, float handleAngle, bool oneHandle)
        {
            sprite = new RecordSprite(content, size, oneHandle);
            rotation = handleAngle;
            this.position = position;
            onDisc = new List<OnRecord>();
            recordGroup = new List<Record>();
            this.size = size;
            twoHandles = !oneHandle;
            zip = new SoundFX("scratch_out");
            zah = new SoundFX("scratch_in");
			dragging = -1;
        }

        public void Load(IEnumerable<OnRecord> objects)
        {
            onDisc.AddRange(objects);
        }

        public void SetRecordGroup(IEnumerable<Record> records)
        {
            recordGroup.AddRange(records);
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);

            int wasDragging = dragging;
            dragging = -1;

            for (int i = 0; i < Input.TOUCH_COUNT; i++)
            {
                if (state.Input.MouseJustClicked(i))
                {
                    if ((state.Input.TouchPos(i) + state.Camera.Position - HandlePosition).LengthSquared() < 20 * 20 * SingleLevel.SCALE * SingleLevel.SCALE)
                    {
                        dragging = i;
                        state.Input.ConsumeClick(i);
                        (sprite as RecordSprite).SetHighlight(1);
                    }
                    else if (twoHandles && (state.Input.TouchPos(i) + state.Camera.Position - Handle2Position).LengthSquared() < 20 * 20 * SingleLevel.SCALE * SingleLevel.SCALE)
                    {
                        dragging = i;
                        state.Input.ConsumeClick(i);
                        (sprite as RecordSprite).SetHighlight(2);
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

                    int soundToPlay = a > 0 ? 1 : 2;

                    if (Math.Abs(a) < 0.07f)
                    {
                        soundToPlay = 0;
                    }

                    if (soundToPlay != soundPlaying && soundToPlay > 0)
                    {
                        if (soundToPlay == 1)
                        {
                            zip.Play();
                        }
                        else
                        {
                            zah.Play();
                        }
                        soundPlaying = soundToPlay;
                    }

                    foreach (OnRecord or in onDisc)
                    {
                        if ((or.Position - Position).LengthSquared() < size * size)
                        {
                            or.RotatePosition(a, Position);
                        }
                    }
                }
            }


            if (dragging < 0)
            {
                (sprite as RecordSprite).SetHighlight(-1);
            }

        }

        public void Rotate(float angle)
        {
            rotation += angle;
            foreach (OnRecord or in onDisc)
            {
                if ((or.Position - Position).LengthSquared() < size * size)
                {
                    or.RotatePosition(angle, Position);
                }
            }

        }
    }
}
