using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class AnimatedSprite : Sprite
    {
        protected List<Rectangle> frames;
        protected List<Point> offsets;
        protected List<Animation> animations;
        public int currentAnimation;
        protected int currentFrame;
        double delayCounter;
        bool reverse;

        public AnimatedSprite(Texture2D image, string magicFrameBoundsString, string magicOffsetsString, Point fixedSize)
        {
            this.image = image;
            frames = new List<Rectangle>();
            string[] frameBoundsList = magicFrameBoundsString.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < frameBoundsList.Length / 4; i++)
            {
                int frameStart = i * 4;
                frames.Add(new Rectangle(int.Parse(frameBoundsList[frameStart]),
                    int.Parse(frameBoundsList[frameStart + 1]),
                    int.Parse(frameBoundsList[frameStart + 2]),
                    int.Parse(frameBoundsList[frameStart + 3])));
            }

            offsets = new List<Point>();
            string[] offsetList = magicOffsetsString.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < offsetList.Length / 2; i++)
            {
                int offsetStart = i * 2;
                offsets.Add(new Point(int.Parse(offsetList[offsetStart]), int.Parse(offsetList[offsetStart + 1])));
            }

            currentAnimation = -1;
            currentFrame = -1;

            this.fixedSize = fixedSize;
        }

        protected AnimatedSprite(Texture2D image, List<Rectangle> frames, List<Point> offsets, Point fixedSize)
        {
            this.image = image;
            this.frames = frames;
            this.offsets = offsets;
            currentAnimation = -1;
            currentFrame = -1;

            this.fixedSize = fixedSize;
        }

        protected void SetAnimation(int animation)
        {
            if (animation >= 0 && animation < animations.Count)
            {
                reverse = false;
                delayCounter = 0;
                currentAnimation = animation;
                currentFrame = animations[currentAnimation].startFrame;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (currentAnimation < 0 || animations == null || currentAnimation >= animations.Count)
            {
                return;
            }
            delayCounter += gameTime.ElapsedGameTime.TotalSeconds;
            bool tick = true;
            while (tick)
            {
                tick = false;
                if (!reverse && currentFrame < animations[currentAnimation].endFrame)
                {
                    if (delayCounter > animations[currentAnimation].frameDelay)
                    {
                        delayCounter -= animations[currentAnimation].frameDelay;
                        tick = true;
                        currentFrame++;
                    }
                }
                else if (reverse && currentFrame > animations[currentAnimation].startFrame)
                {
                    if (delayCounter > animations[currentAnimation].frameDelay)
                    {
                        delayCounter -= animations[currentAnimation].frameDelay;
                        tick = true;
                        currentFrame--;
                    }
                }
                else
                {
                    AnimationEnded(currentAnimation);
                    if (animations[currentAnimation].nextAnimation != null)
                    {
                        if (delayCounter > animations[currentAnimation].nextAnimationDelay)
                        {
                            delayCounter -= animations[currentAnimation].nextAnimationDelay;
                            tick = true;
                            currentAnimation = animations.IndexOf(animations[currentAnimation].nextAnimation);
                            currentFrame = animations[currentAnimation].startFrame;
                        }
                    }
                    else
                    {
                        //delayCounter -= animations[currentAnimation].frameDelay;
                        switch (animations[currentAnimation].loopType)
                        {
                            case Animation.LoopType.Vanish:
                                currentAnimation = -1;
                                currentFrame = -1;
                                break;
                            case Animation.LoopType.Repeat:
                                currentFrame = animations[currentAnimation].startFrame;
                                tick = true;
                                if (animations[currentAnimation].startFrame == animations[currentAnimation].endFrame)
                                {
                                    tick = false;
                                }
                                break;
                            case Animation.LoopType.PingPong:
                                reverse = !reverse;
                                //currentFrame += reverse ? -1 : 1;
                                //delayCounter -= animations[currentAnimation].frameDelay;
                                tick = true;
                                break;
                            case Animation.LoopType.Stop:
                            default:
                                break;
                        }
                    }
                }
            }
        }

        protected virtual void AnimationEnded(int animation)
        {}

        protected void AddAnimation(int index, Animation animation)
        {
            if (animations == null)
            {
                animations = new List<Animation>();
            }
            while (animations.Count < index + 1)
            {
                animations.Add(null);
            }
            animations[index] = animation;
        }

        protected void SetNextAnimation(int nextAnimation, int index, double delay)
        {
            if (animations.Count <= index || animations.Count <= nextAnimation)
            {
                return;
            }
            else
            {
                animations[index].nextAnimation = animations[nextAnimation];
                animations[index].nextAnimationDelay = delay;
            }
        }

        public override void Draw(SpriteBatch sb, Vector2 position, float rotation)
        {
            if (currentFrame < 0 || currentFrame > frames.Count)
            {
                return;
            }
            sb.Draw(image, position, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), 1, SpriteEffects.None, 1);
        }

        public override void DrawMiniMap(SpriteBatch sb, Vector2 miniPos, float rotation)
        {
            if (currentFrame < 0 || currentFrame > frames.Count)
            {
                return;
            }
            sb.Draw(image, miniPos, frames[currentFrame], Color.White, rotation, PtoV(fixedSize) / 2 - PtoV(offsets[currentFrame]), MINI_SCALE, SpriteEffects.None, 1);
        }
    }

    class Animation
    {
        public enum LoopType { Stop, Vanish, Repeat, PingPong };
        public Animation nextAnimation;
        public LoopType loopType;
        public double frameDelay, nextAnimationDelay;
        public int startFrame;
        public int endFrame;

        public Animation(double frameDelay, int startFrame, int endFrame, LoopType loopType)
        {
            this.frameDelay = frameDelay;
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            this.loopType = loopType;
        }
    }
}
