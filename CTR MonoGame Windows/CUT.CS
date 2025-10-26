using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class Cut
    {
        public Vector2 start, end;
        public float width;
        public float lifetime;

        public Cut(Vector2 start, Vector2 end)
        {
            lifetime = 0.1f;
            width = 5;
            this.start = start;
            this.end = end;
        }
    }
}
