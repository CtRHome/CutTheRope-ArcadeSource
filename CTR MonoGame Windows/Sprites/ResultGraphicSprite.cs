using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class ResultGraphicSprite : AnimatedSprite
    {
        public enum Image { Star1, Star2, Star3, Title, Line1, Data, DataValue, Final, ScoreValue, Button3, Button2, Button1, Stamp, Star, StarEmpty, Line, DarkArea }

        Image sign;
        float alpha;
        float scale;

        public ResultGraphicSprite(ContentManager content, Image sign)
            : base(content.Load<Texture2D>("menu_result_hd"), "1,1,7,6,1,9,6,6,1,17,6,6,1,25,6,7,10,1,8,8,1,34,6,8,1,44,6,8,10,11,8,8,10,21,8,8,1,54,6,6,10,31,8,6,1,62,7,6,1,70,6,6,20,1,182,175,20,178,168,162,204,1,503,9,204,12,714,302",
            "227,425,395,425,561,425,395,282,397,597,280,557,556,557,397,557,397,652,395,931,582,776,227,776,681,595,655,281,662,288,483,373,122,71", new Point(1503, 1202))
        {
            this.sign = sign;
            this.alpha = 1;
            this.scale = 1;
        }

        public void SetAlpha(float alpha)
        {
            this.alpha = alpha;
        }

        public void SetScale(float scale)
        {
            this.scale = scale;
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            currentFrame = (int)sign;
            sb.Draw(image, position, frames[currentFrame], new Color(Color.White, alpha), rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), scale, SpriteEffects.None, 1);
        }
    }
}
