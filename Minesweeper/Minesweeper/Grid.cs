using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    class Grid : Sprite
    {
        public int dimension;
        public int totalMines;
        public Cell[] Cells;
        public int CellHeight;

        public Grid(int d, int m, int CellHeight, string Asset)
        {
            this.dimension = d;
            this.totalMines = m;
            this.AssetName = Asset;
            this.CellHeight = CellHeight;
            this.height = CellHeight * dimension;

            Cells = new Cell[dimension * dimension];
            InitializeCells("mined");

        }

        /// <summary>
        /// Initializes all the mines in the various cells.
        /// </summary>
        /// <param name="type">These types: mined, special, and noMines.</param>
        public void InitializeCells(string type)
        {
            if(type.Equals("noMines"))
            {
                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new Cell(dimension, CellHeight, PositionFromIndex(i), false);
                    Cells[i].StateChanged();
                }
            }
            else if (type.Equals("mined"))
            {
                Random gen = new Random();
                // Creates an empty field.
                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new Cell(dimension, CellHeight, PositionFromIndex(i), false);
                }
                // Sets totalMines random mines in the field.
                for (int i = 0; i < totalMines; i++)
                {
                    bool done = false;
                    int n;
                    while(!done)
                    {
                        n = gen.Next(Cells.Length);
                        if (!Cells[n].isMined)
                        {
                            Cells[n].isMined = true;
                            done = true;
                        }
                    }
                }
                
            }
            else if (type.Equals("special"))
            {
                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new Cell(dimension, CellHeight, PositionFromIndex(i), false);
                    Cells[i].StateChanged();
                }
                Cells[1] = new Cell(dimension, CellHeight, PositionFromIndex(0), true);
                Cells[dimension] = new Cell(dimension, CellHeight, PositionFromIndex(dimension), true);
                Cells[1 + dimension] = new Cell(dimension, CellHeight, PositionFromIndex(1 + dimension), true);
                
            }
            SetMineCount();
        }

        /// <summary>
        /// Sets the number of adjacent mines for all the cells in the grid.
        /// </summary>
        private void SetMineCount()
        {
            List<int> AdjacentMines;
            //int i = 0;
            for (int i = 0; i < Cells.Length; i++)
            {
                if (!Cells[i].isMined)
                {
                    AdjacentMines = getValidAdjacentLocations(i);
                    for (int j = 0; j < AdjacentMines.Count; j++)
                    {
                        if (Cells[AdjacentMines[j]].isMined)
                        {
                            Cells[i].AdjacentMines++;
                            //Cells[i].StateChanged();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Slightly complex method: gets all the adjacent indices for a given index in the grid.
        /// </summary>
        /// <param name="index">An index in the grid.</param>
        /// <returns>A list of all the adjacent indices.</returns>
        public List<int> getValidAdjacentLocations(int index)
        {
            List<int> AdjacentMines = new List<int>();
            // top edge
            if (index < dimension-1 && index > 0)
            {
                AdjacentMines.Add(index - 1);
                AdjacentMines.Add(index + 1);
                AdjacentMines.Add(index + dimension);
                AdjacentMines.Add(index + dimension + 1);
                AdjacentMines.Add(index + dimension - 1);
            }
            // TL corner
            else if (index == 0)
            {
                AdjacentMines.Add(1);
                AdjacentMines.Add(dimension);
                AdjacentMines.Add(dimension + 1);
            }
            // TR
            else if (index == dimension - 1)
            {
                AdjacentMines.Add(dimension - 2);
                AdjacentMines.Add((dimension * 2) - 1);
                AdjacentMines.Add((dimension * 2) - 2);
            }
            // BL
            else if (index == dimension * (dimension - 1))
            {
                int x = dimension * (dimension - 1);
                AdjacentMines.Add(x + 1);
                AdjacentMines.Add(x - dimension);
                AdjacentMines.Add(x - dimension + 1);
            }
            // BR
            else if (index == dimension * dimension - 1)
            {
                int x = dimension * dimension - 1;
                AdjacentMines.Add(x - 1);
                AdjacentMines.Add(x - dimension);
                AdjacentMines.Add(x - dimension - 1);
            }
            // bottom edge
            else if (index < dimension * dimension - 1 && index > dimension * (dimension - 1))
            {
                AdjacentMines.Add(index - 1);
                AdjacentMines.Add(index + 1);
                AdjacentMines.Add(index - dimension);
                AdjacentMines.Add(index - dimension + 1);
                AdjacentMines.Add(index - dimension - 1);
            }
            // left edge
            else if (index % dimension == 0)
            {
                AdjacentMines.Add(index - dimension);
                AdjacentMines.Add(index + dimension);
                AdjacentMines.Add(index + 1);
                AdjacentMines.Add(index - dimension + 1);
                AdjacentMines.Add(index + dimension + 1);
            }
            // right edge
            else if (index % dimension == dimension - 1)
            {
                AdjacentMines.Add(index - dimension);
                AdjacentMines.Add(index + dimension);
                AdjacentMines.Add(index - 1);
                AdjacentMines.Add(index - dimension - 1);
                AdjacentMines.Add(index + dimension - 1);
            }
            // anywhere else
            else
            {
                AdjacentMines.Add(index + 1);
                AdjacentMines.Add(index - 1);
                AdjacentMines.Add(index + dimension);
                AdjacentMines.Add(index - dimension);
                AdjacentMines.Add(index + dimension + 1);
                AdjacentMines.Add(index + dimension - 1);
                AdjacentMines.Add(index - dimension + 1);
                AdjacentMines.Add(index - dimension - 1);
            }
           
            return AdjacentMines;
        }

        /// <summary>
        /// The method sequence that is run when a cell is opened.
        /// It's recursive! =D
        /// </summary>
        /// <param name="index">The index of the cell to be opened.</param>
        /// <returns>All the opened cells.</returns>
        public List<int> Play(int index, ContentManager Content)
        {
            // The list of all opened cells.
            List<int> OpenedCells = new List<int>();
            // All this occurs only if the selected cell is not mined.
            if(!Cells[index].isMined)
            {
                // Oh, also only if the selected cell is closed. =P
                if (Cells[index].CellState == Cell.State.Closed)
                {
                    // Opens current cell.
                    Cells[index].CellState = Cell.State.Opened;
                    // If n > 0, open only yourself. Do not open any other cells at all.
                    if (Cells[index].AdjacentMines > 0)
                    {
                        OpenedCells.Add(index);
                        Cells[index].StateChanged();
                        Cells[index].LoadContent(Content);
                    }
                    // If n == 0 (n is never < 0), then...
                    else
                    {
                        // Get all adjacent cells.
                        List<int> AdjacentCells = getValidAdjacentLocations(index);
                        for (int i = 0; i < AdjacentCells.Count; i++)
                        {
                            // For each of the adjacent cells, open it.
                            List<int> OpenedCellsCurrent = Play(AdjacentCells[i], Content);
                            // Add the opened cells to the list of opened cells.
                            foreach (int j in OpenedCellsCurrent)
                            {
                                OpenedCells.Add(j);
                                Cells[index].StateChanged();
                                Cells[index].LoadContent(Content);
                            }
                        }
                    }
                }
            }
            return OpenedCells;
        }
        
        /// <summary>
        /// Converting index into Position.
        /// </summary>
        /// <param name="Index">The index of the cell.</param>
        /// <returns>The position of the given cell.</returns>
        public Vector2 PositionFromIndex(int Index)
        {
            if (Index < dimension * dimension || Index >= 0)
            {
                int x = ((Index) % dimension) * CellHeight;
                int y = ((Index) / dimension) * CellHeight;
                return new Vector2(x, y);
            }
            else
            {
                return new Vector2(-1, -1);
            }
        }
        /// <summary>
        /// Converting Position into index.
        /// </summary>
        /// <param name="MyPosition">Position of the cell.</param>
        /// <returns>The index of the given cell.</returns>
        public int IndexFromPosition(Vector2 MyPosition)
        {
            MyPosition.X = (int)(MyPosition.X / CellHeight);
            MyPosition.Y = (int)(MyPosition.Y / CellHeight);
            return (int)(MyPosition.X + dimension * MyPosition.Y);

        }
        
        public void LoadContent(ContentManager theContentManager)
        {
            Texture = theContentManager.Load<Texture2D>(AssetName);
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].LoadContent(theContentManager);
            }
        }
        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(Texture, Position,
                 new Rectangle(0, 0, Texture.Width, Texture.Height), Color.White,
                 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].Draw(theSpriteBatch, CellHeight);
            }

        }

        
    }
}
