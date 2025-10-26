using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CTR_MonoGame
{
    class GiantNumberTextSprite : Sprite
    {
        protected float scale;

        protected Texture2D[] digits;

        public GiantNumberTextSprite(ContentManager content)
            : base(content.Load<Texture2D>("TallyScreen/0"), new Rectangle(0,0,190,360), Point.Zero, new Point(190, 360))
        {
            digits = new Texture2D[10];
            for (int i = 0; i < digits.Length; i++)
            {
                digits[i] = content.Load<Texture2D>("TallyScreen/" + i);
            }
            scale = 1;
        }

        private string[] GetLines(string s)
        {
            return s.Split('\n');
        }

        private float GetMaxLength(string s)
        {
            return (from line in GetLines(s) select GetLength(line)).Max();
        }

        private float GetLength(string s)
        {
            float rVal = 0;
            for (int i = 0; i < s.Length; i++)
            {
                rVal += GetWidth(s[i]);
            }
            return rVal;
        }

        public float GetWidth(char c)
        {
            return digits[int.Parse(c.ToString())].Width * scale * 0.8f;
        }
		
		public void SetScale(float newScale)
		{
			scale = newScale;
		}

        public void Draw(SpriteBatch sb, string s, Vector2 pos)
        {
            Draw(sb, s, pos, TextSprite.Alignment.Left);
        }

        public void Draw(SpriteBatch sb, string s, Vector2 pos, TextSprite.Alignment a)
        {
            Draw(sb, s, pos, a, Color.White);
        }

        public void Draw(SpriteBatch sb, string s, Vector2 pos, TextSprite.Alignment a, Color color)
        {
            Draw(sb, s, pos, a, color, 0);
        }

        public void Draw(SpriteBatch sb, string s, Vector2 pos, TextSprite.Alignment a, Color color, float rotation)
        {
            Vector2 position = pos;
            foreach (string line in GetLines(s))
            {
                position.X = pos.X;
                float length = GetLength(line);
                switch (a)
                {
                    default:
                    case TextSprite.Alignment.Left:
                        break;
                    case TextSprite.Alignment.Center:
                        position.X -= length / 2;
                        break;
                    case TextSprite.Alignment.Right:
                        position.X -= length;
                        break;
                }
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] != ' ')
                    {
                        sb.Draw(digits[int.Parse(line[i].ToString())], Util.RotateVector(position, rotation, pos), null, color, rotation, Vector2.Zero, scale, SpriteEffects.None, 1);
                    }
                    position.X += GetWidth(line[i]);
                }
                position.Y += digits[0].Height * scale;
            }
        }
    }
}
