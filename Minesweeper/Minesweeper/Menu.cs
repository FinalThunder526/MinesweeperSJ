using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    class Menu : Sprite
    {
        float scale;

        public Menu(Vector2 Position, string assetName)
            : base(Position, assetName)
        { }
    }
}
