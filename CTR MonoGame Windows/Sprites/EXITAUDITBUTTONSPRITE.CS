using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CTR_MonoGame
{
    class ExitAuditButtonSprite : ButtonSprite
    {

        public ExitAuditButtonSprite(ContentManager content, ExitAuditButton.ButtonType type)
            : base(content.Load<Texture2D>(TextureName(type)),
            new Rectangle[] { new Rectangle(0, 0, type == ExitAuditButton.ButtonType.Exit ? 256 : 462, 94) },
            new Point[] { Point.Zero }, new Point(type == ExitAuditButton.ButtonType.Exit ? 256 : 462, 94), 0, 0)
        {
        }

        static string TextureName(ExitAuditButton.ButtonType type)
        {
            switch (type)
            {
                case ExitAuditButton.ButtonType.Exit:
                    return "exitButton";
                case ExitAuditButton.ButtonType.Audit:
                    return "auditButton";
                case ExitAuditButton.ButtonType.Operator:
                default:
                    return "operatorButton";
            }
        }
    }
}
