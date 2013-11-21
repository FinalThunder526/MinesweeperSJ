using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    class Cell : Sprite
    {
        public enum State { Closed, Flagged, Question, Opened, FalseFlag };
        public State CellState;
        
        public int AdjacentMines, Dimension;
        public bool isMined;

        public Cell(int D, int Height, Vector2 Pos, bool isThisMined)
        {
            isMined = isThisMined;
            this.height = height;
            this.Dimension = D;
            CellState = State.Closed;
            this.Position = Pos;
            StateChanged();
        }

        // Only the Location is changed by external forces.
        /// <summary>
        /// Converting index to Position.
        /// </summary>
        /// <param name="Index">Index of a cell.</param>
        /// <returns>The corresponding Position.</returns>
        public Vector2 PositionFromIndex(int Index)
        {
            if (Index < Dimension * Dimension || Index >= 0)
            {
                int x = ((Index) % Dimension) * height;
                int y = ((Index) / Dimension) * height;
                return new Vector2(x, y);
            }
            else
            {
                return new Vector2(-1, -1);
            }
        }

        // Remember to load content again after this.
        /// <summary>
        /// Updates the cell's asset based on its original CellState.
        /// </summary>
        public void StateChanged()
        {
            if (CellState == State.Opened)
            {
                if (!isMined)
                    this.AssetName = "Cells\\" + Convert.ToString(AdjacentMines);
                else
                    this.AssetName = "Cells\\Mine";
            }
            else
            {
                this.AssetName = "Cells\\" + Convert.ToString(CellState);
            }
        }

        public void Draw(SpriteBatch theSpriteBatch, int CellHeight)
        {
            theSpriteBatch.Draw(Texture, Position,
                 new Rectangle(0, 0, Texture.Width, Texture.Height), Color.White,
                 0.0f, Vector2.Zero, (float)((float)(CellHeight)/40f), SpriteEffects.None, 0);
        }
    }
}
