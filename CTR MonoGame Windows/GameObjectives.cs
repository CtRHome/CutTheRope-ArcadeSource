using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CTR_MonoGame
{
    class GameObjectives : ITransitionable
    {
        float initialDelay;

        Texture2D getBonus;
        Input input;

        public bool Done
        {
            get;
            protected set;
        }

        public GameObjectives(ContentManager content, Input input)
        {
            Done = false;

            initialDelay = 0;
            this.input = input;

            getBonus = content.Load<Texture2D>("getBonus");
        }

        public void UpdateTransition(GameTime gameTime)
        {
            Update(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            initialDelay += delta;

            if (input.MouseJustClicked() || input.KeyJustPressed(Keys.Space) || initialDelay > 4)
            {
                Done = true;
            }
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Matrix.CreateScale(0.5f));

            spriteBatch.Draw(getBonus, Vector2.Zero, Color.White);

            spriteBatch.End();
        }
    }
}
