using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class OnRecord : GameObject
    {
        public virtual void RotatePosition(float angle, Vector2 center)
        {
            position = Util.RotateVector(position, angle, center);
            base.UpdateBounds();
        }
    }
}
