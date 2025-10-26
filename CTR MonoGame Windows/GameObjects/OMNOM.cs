using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Tao.Sdl;
using Microsoft.Xna.Framework.Graphics;


namespace CTR_MonoGame
{
    class OMNOM : GameObject
    {
        public enum SpecialCharacter { None, Caesar, Painter, Pharaoh, Pirate, Viking, Prehistoric }

        Box box;
        float mouthCloseTimer;
        bool mouthOpen;
        bool gotCandy;
        SoundFX mouthOpenSound, mouthCloseSound, chewSound, sadSound;
        TextSprite valueFont;
        Texture2D ribbonSeat;

        public int Value
        {
            get;
            set;
        }

        public OMNOM(ContentManager content, Box box, Vector2 position)
            :this(content, box, position, 0)
        {}

        public OMNOM(ContentManager content, Box box, Vector2 position, int value)
            : this(content, box, position, value, SpecialCharacter.None)
        { }
           
        public OMNOM(ContentManager content, Box box, Vector2 position, int value, SpecialCharacter type)
        {
            this.Value = value;
            switch (type)
            {
                default:
                case SpecialCharacter.None:
                    sprite = new CharacterSprite(content);
                    break;
                case SpecialCharacter.Caesar:
                case SpecialCharacter.Painter:
                case SpecialCharacter.Pharaoh:
                case SpecialCharacter.Pirate:
                case SpecialCharacter.Viking:
                    sprite = new SpecialCharacterSprite(content, type);
                    break;
                case SpecialCharacter.Prehistoric:
                    break;
            }
            this.box = box;
            this.position = position;
            (sprite as ICharacterAnimation).SetAnimation(CharacterSprite.Animations.CHAR_ANIMATION_GREETING);
            mouthOpenSound = new SoundFX("Content/monster_open.ogg");
            mouthCloseSound = new SoundFX("Content/monster_close.ogg");
            chewSound = new SoundFX("Content/monster_chewing.ogg");
            sadSound = new SoundFX("Content/monster_sad.ogg");
            valueFont = new TextSprite(content, true, 0.7f);
            ribbonSeat = content.Load<Texture2D>("ribbonSeat");
        }

        public void SetType(ContentManager content, SpecialCharacter type)
        {
            switch (type)
            {
                default:
                case SpecialCharacter.None:
                    sprite = new CharacterSprite(content);
                    break;
                case SpecialCharacter.Caesar:
                case SpecialCharacter.Painter:
                case SpecialCharacter.Pharaoh:
                case SpecialCharacter.Pirate:
                case SpecialCharacter.Viking:
                case SpecialCharacter.Prehistoric:
                    sprite = new SpecialCharacterSprite(content, type);
                    break;
            }
            (sprite as ICharacterAnimation).SetAnimation(CharacterSprite.Animations.CHAR_ANIMATION_GREETING);
        }

        public override void Update(GameTime gameTime, GlobalState state)
        {
            base.Update(gameTime, state);

            if (!state.Candy.Half && !state.Candy.Spidered && !gotCandy)
            {
                if (!mouthOpen)
                {
                    if ((state.Candy.Position - position).LengthSquared() < 200 * 200)
                    {
                        mouthOpen = true;
                        mouthCloseTimer = 1;
                        (sprite as ICharacterAnimation).SetAnimation(CharacterSprite.Animations.CHAR_ANIMATION_MOUTH_OPEN);
                        mouthOpenSound.Play();
                    }
                }
                else
                {
                    if (mouthCloseTimer > 0)
                    {
                        mouthCloseTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (mouthCloseTimer <= 0)
                        {
                            if ((state.Candy.Position - position).LengthSquared() > 200 * 200)
                            {
                                mouthOpen = false;
                                (sprite as ICharacterAnimation).SetAnimation(CharacterSprite.Animations.CHAR_ANIMATION_MOUTH_CLOSE);
                                mouthCloseSound.Play();
                            }
                            else
                            {
                                mouthCloseTimer = 1;
                            }
                        }
                    }
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Vector2 cameraPosition)
        {
            if (Value > 0)
            {
                sb.Draw(ribbonSeat, position - cameraPosition + Vector2.UnitY * 40, null, Color.White, rotation, new Vector2(ribbonSeat.Width, ribbonSeat.Height) / 2f, 1, SpriteEffects.None, 1);
            }
            else
            {
                box.Platform.Draw(sb, position - cameraPosition, 0);
            }
            base.Draw(sb, cameraPosition);
            if (Value > 0)
            {
                if (!FCOptions.FixedTickets && FCOptions.UseTickets)
                {
                    valueFont.SetScale(0.6f);
                    valueFont.Draw(sb, FCOptions.TicketName.ToString() + "s", position - cameraPosition + Vector2.UnitY * 75, TextSprite.Alignment.Center);
                    valueFont.SetScale(1.2f);
                }
                valueFont.Draw(sb, Value.ToString(), position - cameraPosition + Vector2.UnitY * 100, TextSprite.Alignment.Center);
            }
        }

        public override void DrawMiniMap(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, int levelY)
        {
            box.Platform.DrawMiniMap(sb, GetMiniPos(position, levelY), 0);
            base.DrawMiniMap(sb, levelY);
        }

        internal void Excite()
        {
            if ((sprite as AnimatedSprite).currentAnimation == (int)CharacterSprite.Animations.CHAR_ANIMATION_IDLE)
            {
                (sprite as ICharacterAnimation).SetAnimation(CharacterSprite.Animations.CHAR_ANIMATION_EXCITED);
            }
        }

        internal void CandyGot()
        {
            gotCandy = true;
            (sprite as ICharacterAnimation).SetAnimation(CharacterSprite.Animations.CHAR_ANIMATION_WIN);
            chewSound.Play();
        }

        internal void Lose()
        {
            (sprite as ICharacterAnimation).SetAnimation(CharacterSprite.Animations.CHAR_ANIMATION_FAIL);
            sadSound.Play();
        }

        internal void SetBox(Box box)
        {
            this.box = box;
        }
    }
}
