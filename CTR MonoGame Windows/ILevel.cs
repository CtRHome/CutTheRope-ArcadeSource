namespace CTR_MonoGame
{
    interface ILevel : ITransitionable
    {
        void DrawMiniMap(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch);
        bool GameOver { get; }
        int StarsGot { get; }
        void Update(Microsoft.Xna.Framework.GameTime gameTime);
        bool Victory { get; }
        float[] Times { get; }
        ILevel Reset();
    }
}
