using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CTR_MonoGame
{
    class ExitAuditButton : Button
    {
        public enum ButtonType { Exit, Audit, Operator }

        public ExitAuditButton(ContentManager content, Vector2 position, ButtonType type)
            : base(position, type == ButtonType.Exit ? 256 : 462, 94)
        {
            sprite = new ExitAuditButtonSprite(content, type);
        }
    }
}
