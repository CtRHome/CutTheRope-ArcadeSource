using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class CharacterSprite : AnimatedSprite, ICharacterAnimation
    {
        public enum Animations
        {
            CHAR_ANIMATION_IDLE, CHAR_ANIMATION_IDLE2, CHAR_ANIMATION_IDLE3, CHAR_ANIMATION_IDLE4, CHAR_ANIMATION_EXCITED, CHAR_ANIMATION_PUZZLED,
    CHAR_ANIMATION_FAIL, CHAR_ANIMATION_WIN, CHAR_ANIMATION_MOUTH_OPEN, CHAR_ANIMATION_MOUTH_CLOSE, CHAR_ANIMATION_CHEW,
    CHAR_ANIMATION_GREETING }

        #region Animation Frame Constants
        const int IMG_CHAR_ANIMATIONS_fail_start = 0;
        const int IMG_CHAR_ANIMATIONS_fail_end = 12;
        const int IMG_CHAR_ANIMATIONS_mouth_open_start = 13;
        const int IMG_CHAR_ANIMATIONS_mouth_open_end = 21;
        const int IMG_CHAR_ANIMATIONS_mouth_close_start = 22;
        const int IMG_CHAR_ANIMATIONS_mouth_close_end = 25;
        const int IMG_CHAR_ANIMATIONS_chew_start = 26;
        const int IMG_CHAR_ANIMATIONS_chew_end = 34;
        const int IMG_CHAR_ANIMATIONS_blink_start = 35;
        const int IMG_CHAR_ANIMATIONS_blink_end = 36;
        const int IMG_CHAR_ANIMATIONS_excited_start = 37;
        const int IMG_CHAR_ANIMATIONS_excited_end = 56;
        const int IMG_CHAR_ANIMATIONS_puzzled_start = 57;
        const int IMG_CHAR_ANIMATIONS_puzzled_end = 83;
        const int IMG_CHAR_ANIMATIONS_greeting_start = 84;
        const int IMG_CHAR_ANIMATIONS_greeting_end = 112;

        const int IMG_CHAR_ANIMATIONS_IDLE_idle_start = 0;
        const int IMG_CHAR_ANIMATIONS_IDLE_idle_end = 18;
        const int IMG_CHAR_ANIMATIONS_IDLE_idle2_start = 19;
        const int IMG_CHAR_ANIMATIONS_IDLE_idle2_end = 43;
        const int IMG_CHAR_ANIMATIONS_IDLE_idle3_start = 44;
        const int IMG_CHAR_ANIMATIONS_IDLE_idle3_end = 59;
        #endregion

        Texture2D idleTexture;
        List<Rectangle> idleFrames;
        List<Point> idleOffsets;
        int idleCounter;

        public float Scale = 1;

        public CharacterSprite(ContentManager content)
            : base(content.Load<Texture2D>("char_animations_hd"), "1,1,179,158,1,161,170,162,1,325,164,169,1,496,159,173,1,671,156,175,1,848,157,177,1,1027,159,170,1,1199,162,163,1,1364,165,159,1,1525,168,161,173,161,171,163,173,326,173,166,348,326,174,166,1,1688,165,182,1,1872,164,172,173,494,163,170,173,666,163,168,173,836,163,164,346,161,163,162,511,161,163,162,676,161,163,162,841,161,163,162,173,1002,163,167,173,1171,163,174,182,1,164,157,348,1,179,138,1006,161,201,159,1209,161,182,162,173,1347,165,166,173,1515,163,172,173,1689,163,179,348,494,163,179,513,494,165,178,680,494,185,172,524,326,196,166,529,1,124,70,529,73,114,73,655,1,171,158,173,1870,168,169,348,675,159,188,348,865,150,199,348,1066,146,198,348,1266,150,196,348,1464,156,189,348,1655,161,186,348,1843,161,178,513,674,161,180,513,856,161,182,513,1040,161,184,513,1226,161,186,513,1414,161,186,513,1602,161,186,513,1790,157,189,676,674,154,192,832,674,156,186,867,494,163,175,1032,494,167,169,1393,161,182,163,722,326,172,165,1201,494,158,170,1361,494,153,172,1516,494,154,176,990,674,156,179,1672,494,160,172,1834,494,164,170,1577,161,167,163,896,326,170,165,1148,674,173,167,1323,674,174,170,1499,674,174,171,1675,674,174,171,1851,674,174,171,832,862,174,171,832,1035,174,171,832,1208,174,171,832,1381,174,171,832,1554,174,171,832,1727,174,171,1008,862,171,174,1008,1038,166,179,1008,1219,163,181,1008,1402,165,179,1181,862,170,172,1353,862,171,168,1008,1583,160,180,1008,1765,158,187,1176,1038,157,190,1335,1038,160,190,1176,1230,167,192,1497,1038,170,182,1526,862,175,167,828,1,172,157,1002,1,171,145,1175,1,178,144,1355,1,178,146,1535,1,174,145,1711,1,171,145,1746,161,178,143,1068,326,178,145,1248,326,174,145,1424,326,171,145,1597,326,178,143,1777,326,178,145,832,1900,174,145,1703,862,171,145,1669,1038,174,142,1845,1038,172,141,1876,862,157,165,676,868,152,191,1176,1424,157,188,1345,1230,170,163,1176,1614,164,157,1176,1773,163,165,",
            "169,191,173,187,176,180,179,176,180,174,179,172,179,179,177,186,175,190,174,188,172,186,171,183,171,183,174,167,175,177,175,179,175,181,175,185,175,187,175,187,175,187,175,187,175,181,175,175,175,192,168,211,154,190,163,187,174,183,176,177,176,170,176,170,174,170,162,177,156,183,192,212,199,212,172,191,174,180,179,161,183,150,185,151,183,153,179,160,176,163,176,171,176,169,176,167,176,165,176,163,176,163,176,163,178,160,180,157,179,163,176,174,173,180,166,186,171,184,179,179,182,177,181,173,180,170,178,176,175,179,175,185,173,183,171,181,171,178,171,177,171,177,171,177,171,177,171,177,171,177,171,177,171,177,171,177,172,174,175,169,176,167,175,170,173,177,172,181,178,168,179,161,179,158,178,158,175,156,173,165,169,180,171,191,171,203,166,204,166,202,168,203,171,203,166,205,166,203,168,203,171,203,166,205,166,203,168,203,171,203,169,206,171,207,182,182,184,156,181,159,172,186,175,191,176,184",
            new Point(514, 514))
        {
            idleTexture = content.Load<Texture2D>("char_animations_idle_hd");
            LoadIdleFrames("1,1,163,167,166,1,163,166,331,1,163,167,496,1,163,167,661,1,163,166,1,170,163,170,1,342,163,172,1,516,163,173,1,691,163,177,1,870,163,179,1,1051,163,180,166,870,163,179,1,1233,163,181,1,1416,163,181,1,1599,163,181,331,870,163,178,166,691,163,176,166,516,163,173,166,170,163,170,826,1,176,163,331,170,170,166,503,170,166,170,166,342,169,172,331,691,173,176,166,1051,178,180,506,691,181,174,337,342,176,171,671,170,157,166,830,170,157,167,515,342,157,170,331,516,175,173,689,691,181,174,496,870,176,174,674,870,157,174,833,870,157,174,1,1782,157,174,346,1051,175,174,523,1051,181,174,706,1051,180,176,166,1233,177,179,166,1414,173,181,166,1597,174,174,674,342,169,166,845,342,176,163,508,516,166,154,676,516,164,157,842,516,164,166,166,1773,164,173,345,1233,165,175,512,1233,165,174,679,1233,165,172,846,1233,165,165,345,1410,166,155,345,1567,163,158,345,1727,163,166,513,1410,165,171,513,1583,166,173,681,1583,168,174,851,1583,168,174,680,1410,166,166",
                "176,182,176,182,176,182,176,182,176,182,176,179,176,177,176,175,176,172,176,170,176,169,176,169,176,168,176,168,176,168,176,171,176,173,176,176,176,179,169,186,173,183,175,179,170,177,167,173,164,169,162,175,167,178,186,183,186,182,186,179,168,176,162,175,167,175,186,175,186,175,186,175,168,175,162,175,162,173,164,170,166,168,167,175,174,183,169,186,173,195,175,192,175,182,176,175,176,173,176,174,176,177,175,184,173,194,176,191,176,183,174,178,173,176,171,175,171,175,173,183");

            AddAnimation(Animations.CHAR_ANIMATION_GREETING, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_greeting_start, IMG_CHAR_ANIMATIONS_greeting_end);
            AddAnimation(Animations.CHAR_ANIMATION_IDLE, 0.05, Animation.LoopType.Repeat, IMG_CHAR_ANIMATIONS_IDLE_idle_start, IMG_CHAR_ANIMATIONS_IDLE_idle_end);
            AddAnimation(Animations.CHAR_ANIMATION_IDLE2, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_IDLE_idle2_start, IMG_CHAR_ANIMATIONS_IDLE_idle2_end);
            AddAnimation(Animations.CHAR_ANIMATION_IDLE3, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_IDLE_idle3_start, IMG_CHAR_ANIMATIONS_IDLE_idle3_end);
            AddAnimation(Animations.CHAR_ANIMATION_IDLE4, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_IDLE_idle3_start, IMG_CHAR_ANIMATIONS_IDLE_idle3_end);
            AddAnimation(Animations.CHAR_ANIMATION_EXCITED, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_excited_start, IMG_CHAR_ANIMATIONS_excited_end);
            AddAnimation(Animations.CHAR_ANIMATION_PUZZLED, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_puzzled_start, IMG_CHAR_ANIMATIONS_puzzled_end);
            AddAnimation(Animations.CHAR_ANIMATION_FAIL, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_fail_start, IMG_CHAR_ANIMATIONS_fail_end);
            AddAnimation(Animations.CHAR_ANIMATION_WIN, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_mouth_close_start, IMG_CHAR_ANIMATIONS_mouth_close_end);
            AddAnimation(Animations.CHAR_ANIMATION_MOUTH_OPEN, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_mouth_open_start, IMG_CHAR_ANIMATIONS_mouth_open_end);
            AddAnimation(Animations.CHAR_ANIMATION_MOUTH_CLOSE, 0.05, Animation.LoopType.Stop, IMG_CHAR_ANIMATIONS_mouth_close_start, IMG_CHAR_ANIMATIONS_mouth_close_end);
            AddAnimation(Animations.CHAR_ANIMATION_CHEW, 0.05, Animation.LoopType.Repeat, IMG_CHAR_ANIMATIONS_chew_start, IMG_CHAR_ANIMATIONS_chew_end);

            SetNextAnimation((int)Animations.CHAR_ANIMATION_CHEW, (int)Animations.CHAR_ANIMATION_WIN, 0.05);
            SetNextAnimation((int)Animations.CHAR_ANIMATION_PUZZLED, (int)Animations.CHAR_ANIMATION_MOUTH_CLOSE, 0.05);
            SetNextAnimation((int)Animations.CHAR_ANIMATION_IDLE, (int)Animations.CHAR_ANIMATION_GREETING, 0.05);
            SetNextAnimation((int)Animations.CHAR_ANIMATION_IDLE, (int)Animations.CHAR_ANIMATION_IDLE2, 0.05);
            SetNextAnimation((int)Animations.CHAR_ANIMATION_IDLE4, (int)Animations.CHAR_ANIMATION_IDLE3, 0.05);
            SetNextAnimation((int)Animations.CHAR_ANIMATION_IDLE, (int)Animations.CHAR_ANIMATION_IDLE4, 0.05);
            SetNextAnimation((int)Animations.CHAR_ANIMATION_IDLE, (int)Animations.CHAR_ANIMATION_EXCITED, 0.05);
            SetNextAnimation((int)Animations.CHAR_ANIMATION_IDLE, (int)Animations.CHAR_ANIMATION_PUZZLED, 0.05);

            idleCounter = Util.R.Next(5, 20);
        }

        private void LoadIdleFrames(string magicFrameBoundsString, string magicOffsetsString)
        {
            idleFrames = new List<Rectangle>();
            string[] frameBoundsList = magicFrameBoundsString.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < frameBoundsList.Length / 4; i++)
            {
                int frameStart = i * 4;
                idleFrames.Add(new Rectangle(int.Parse(frameBoundsList[frameStart]),
                    int.Parse(frameBoundsList[frameStart + 1]),
                    int.Parse(frameBoundsList[frameStart + 2]),
                    int.Parse(frameBoundsList[frameStart + 3])));
            }

            idleOffsets = new List<Point>();
            string[] offsetList = magicOffsetsString.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < offsetList.Length / 2; i++)
            {
                int offsetStart = i * 2;
                idleOffsets.Add(new Point(int.Parse(offsetList[offsetStart]), int.Parse(offsetList[offsetStart + 1])));
            }
        }

        void AddAnimation(Animations animation, double delay, Animation.LoopType loopType, int start, int end)
        {
            base.AddAnimation((int)animation, new Animation(delay, start, end, loopType));
        }

        protected override void AnimationEnded(int animation)
        {
            if (animation == (int)Animations.CHAR_ANIMATION_IDLE)
            {
                idleCounter--;
                if (idleCounter == 0)
                {
                    idleCounter = Util.R.Next(2, 5);
                    SetAnimation(Util.R.Next(2) == 0 ? Animations.CHAR_ANIMATION_IDLE2 : Animations.CHAR_ANIMATION_IDLE3);
                }
            }
        }

        public override void Draw(SpriteBatch sb, Microsoft.Xna.Framework.Vector2 position, float rotation)
        {
            if (currentFrame < 0 || currentFrame > frames.Count)
            {
                return;
            }
            if (currentAnimation == (int)Animations.CHAR_ANIMATION_IDLE ||
                currentAnimation == (int)Animations.CHAR_ANIMATION_IDLE2 ||
                currentAnimation == (int)Animations.CHAR_ANIMATION_IDLE3 ||
                currentAnimation == (int)Animations.CHAR_ANIMATION_IDLE4)
            {
                sb.Draw(idleTexture, position, idleFrames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(idleOffsets[currentFrame]), Scale, SpriteEffects.None, 1);
            }
            else
            {
                sb.Draw(image, position, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), Scale, SpriteEffects.None, 1);
            }
        }

        public override void DrawMiniMap(SpriteBatch sb, Vector2 miniPos, float rotation)
        {
            if (currentAnimation == (int)Animations.CHAR_ANIMATION_IDLE ||
                currentAnimation == (int)Animations.CHAR_ANIMATION_IDLE2 ||
                currentAnimation == (int)Animations.CHAR_ANIMATION_IDLE3 ||
                currentAnimation == (int)Animations.CHAR_ANIMATION_IDLE4)
            {
                sb.Draw(idleTexture, miniPos, idleFrames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(idleOffsets[currentFrame]), MINI_SCALE, SpriteEffects.None, 1);
            }
            else
            {
                base.DrawMiniMap(sb, miniPos, rotation);
            }
        }

        private Vector2 GetIdleDrawPos(Vector2 position)
        {
            return new Vector2(position.X + idleOffsets[currentFrame].X - fixedSize.X / 2, position.Y + idleOffsets[currentFrame].Y - fixedSize.Y / 2);
        }

        public void SetAnimation(Animations animation)
        {
            base.SetAnimation((int)animation);
        }
    }
}
