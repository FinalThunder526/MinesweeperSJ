using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Minesweeper
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MouseState MState;
        SpriteFont Font, Font2;
        Grid MyGrid;
        Menu MyMenu;

        enum GameState { Menu, Game, PreEndGame, EndGame };
        GameState MyGameState = GameState.Menu;

        Stopwatch Watch, SecondWatch;
        
        int CellOver, minesGuessedCorrectly = 0, totalFlags = 0;
        int userDimension = 10;
        bool start = true;

        int wins, losses;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // First, display a selection of mine dimensions.
            //InitializeMenu();
            InitGraphicsMode(600, 600);

            // dimension, # of mines
            MyGrid = new Grid(10, 7, 40, "Grid");
            MyMenu = new Menu(new Vector2(0, 0), "Menu2");
            Watch = new Stopwatch();
            SecondWatch = new Stopwatch();

            //Watch.Start();

            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            MyGrid.LoadContent(this.Content);
            MyMenu.LoadContent(this.Content);
            Font = Content.Load<SpriteFont>("Font");
            Font2 = Content.Load<SpriteFont>("Font2");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        bool haslost = false;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if ((!UpdateMouse(MState) || haslost) && (MyGameState == GameState.Game || MyGameState == GameState.PreEndGame))
                Lose();
            else if (minesGuessedCorrectly >= MyGrid.totalMines && (MyGameState == GameState.Game || MyGameState == GameState.PreEndGame))
                Win();
            
   
            UpdateMouseState();
            if(MyGrid.isOver(new Vector2(MState.X, MState.Y)))
                CellOver = IndexFromMouse(MState);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Occurs when the player has lost.
        /// </summary>
        private void Lose()
        {
            OpenAll("minesOnly");
            haslost = true;
            
            MyGameState = GameState.PreEndGame;
            Watch.Stop(); 
            if (!SecondWatch.IsRunning)
                SecondWatch.Start();
            if (SecondWatch.ElapsedMilliseconds > 3000)
            {
                losses++;
                MyMenu.changeAsset("lose", Content);
                MyGameState = GameState.EndGame;
            }
        }

        /// <summary>
        /// Checks if the player has won.
        /// </summary>
        private void Win()
        {
            OpenAll("all");
            MyGameState = GameState.PreEndGame;
            Watch.Stop();
            if (!SecondWatch.IsRunning)
                SecondWatch.Start();
            if (SecondWatch.ElapsedMilliseconds > 2000)
            {
                wins++;
                MyMenu.changeAsset("win", Content);
                InitGraphicsMode(400, 400);
                MyGameState = GameState.EndGame;
            }
        }


        /// <summary>
        /// End-game sequence
        /// </summary>
        /// <param name="p">The style of what to open up. minesOnly or all</param>
        private void OpenAll(string p)
        {
            for (int i = 0; i < MyGrid.Cells.Length; i++ )
            {
                Cell c = MyGrid.Cells[i];
                if (p.Equals("all"))
                {
                    c.CellState = Cell.State.Opened;
                }
                else if (p.Equals("minesOnly"))
                {
                    c.CellState = (c.isMined) ? Cell.State.Opened : (c.CellState == Cell.State.Flagged) ? Cell.State.FalseFlag : c.CellState;
                }
                ChangeState(i);
            }
        }

        
        /// <summary>
        /// All mouse actions, including detonating.
        /// </summary>
        /// <param name="OldState">Old state of the mouse.</param>
        /// <returns>Whether the player has lost or not.</returns>
        private bool UpdateMouse(MouseState OldState)
        {
            MouseState NewState = Mouse.GetState();
            Vector2 MousePos = new Vector2(NewState.X, NewState.Y);
            float x = MousePos.X, y = MousePos.Y;
            // Initial menu, with the arrows and the level #.
            if (MyGameState == GameState.Menu)
            {
                if (NewState.LeftButton == ButtonState.Pressed && OldState.LeftButton == ButtonState.Released)
                {
                    #region Level Changing
                    // Upper button
                    if (x > 190 && x < 300)
                    {
                        if (y > f1x1(x) && y < 335)
                            userDimension += (userDimension < 25) ? 1 : 0;

                    }
                    else if (x >= 300 && x < 415)
                    {
                        if (y > f1x2(x) && y < 335)
                            userDimension += (userDimension < 25) ? 1 : 0;
                    }

                    // Lower button
                    if (x > 190 && x < 300)
                    {
                        if (y < f2x1(x) && y > 460)
                            userDimension += (userDimension > 7) ? -1 : 0;

                    }
                    else if (x >= 300 && x < 415)
                    {
                        if (y < f2x2(x) && y > 460)
                            userDimension += (userDimension > 7) ? -1 : 0;
                    }
                    #endregion

                    if (x >= 460 && x <= 575 && y >= 350 && y <= 430)
                    {
                        SetGrid(userDimension);
                    }
                }
            }
            // Game, checking cell-clicks.
            else if (MyGameState == GameState.Game)
            {
                if (NewState.LeftButton == ButtonState.Pressed && OldState.LeftButton == ButtonState.Released)
                {
                    if (MyGrid.Cells[CellOver].isMined && MyGrid.Cells[CellOver].CellState != Cell.State.Flagged)
                        return false;
                    if (MyGrid.Cells[CellOver].CellState == Cell.State.Closed && CellOver < MyGrid.Cells.Length)
                    {
                        if (start)
                        {
                            start = false;
                            Watch.Start();
                        }
                        Open();
                    }
                    ChangeState(CellOver);
                }
                else if (NewState.RightButton == ButtonState.Pressed && OldState.RightButton == ButtonState.Released)
                {
                    if (MyGrid.Cells[CellOver].CellState == Cell.State.Closed)
                        Flag();
                    else if (MyGrid.Cells[CellOver].CellState == Cell.State.Flagged)
                        unFlag();
                    else if (MyGrid.Cells[CellOver].CellState == Cell.State.Question)
                        MyGrid.Cells[CellOver].CellState = Cell.State.Closed;
                    ChangeState(CellOver);
                }
            }
            else if (MyGameState == GameState.EndGame)
            {
                if (NewState.LeftButton == ButtonState.Pressed && OldState.LeftButton == ButtonState.Released)
                {
                    if ((x >= 80 && x <= 320) && (y >= 238 && y <= 328))
                    {
                        Reset();
                    }
                }
            }
            return true;
        }

        // Resets the game to the beginning.
        private void Reset()
        {
            MyGameState = GameState.Menu;
            Watch.Reset(); SecondWatch.Reset();
            haslost = false;
            InitGraphicsMode(600, 600);
            MyMenu.changeAsset("Menu2", Content);
        }

        #region Equations for the Buttons
        float f1x1(float x)
        { return (-19f / 22f) * x + 499.091f; }
        float f1x2(float x)
        { return (19f / 23f) * x - 19.091f; }
        float f2x1(float x)
        { return (19f / 22f) * x + 295.901f; }
        float f2x2(float x)
        { return (-19f/23f) * x + 802.826f; }
        #endregion

        /// <summary>
        /// Initializes the grid. Includes re-construction of grid, InitializeCells() and LoadContent().
        /// </summary>
        /// <param name="dim">The dimension of the grid.</param>
        private void SetGrid(int dim)
        {
            MyGrid = new Grid(dim, getMinesFromDimension(dim), 40, "Grid");
            MyGrid.InitializeCells("mined");
            MyGrid.LoadContent(Content);
            InitGraphicsMode(MyGrid.height, MyGrid.height + MyGrid.CellHeight);
            MyGameState = GameState.Game;
        }

        /// <summary>
        /// Given a dimension, calculates the number of mines the minefield should have.
        /// </summary>
        /// <param name="dim">The dimension of the grid.</param>
        /// <returns>The number of mines of the grid.</returns>
        private int getMinesFromDimension(int dim)
        {
            int x = (dim > 12) ? (int)(dim*dim/(5)) : (int)(dim*dim/5.5);
            return x;
        }

        /// <summary>
        /// Opens the cell that the mouse is currently over.
        /// </summary>
        private void Open()
        {
            int currentCell = CellOver;
            if (!MyGrid.Cells[CellOver].isMined)
            {
                // The main cell-opening logic.
                List<int> OpenedCells = MyGrid.Play(currentCell, Content);
                for (int i = 0; i < OpenedCells.Count; i++)
                {
                    ChangeState(OpenedCells[i]);
                }
            }
            else
            {
                MyGrid.Cells[CellOver].CellState = Cell.State.Opened;
                ChangeState(CellOver);
            }
        }

        #region Flagging
        /// <summary>
        /// Flags the current cell. Increases the tally of correct mine guesses by one, if it is a correct guess.
        /// </summary>
        private void Flag()
        {
            int currentCell = CellOver;
            MyGrid.Cells[currentCell].CellState = Cell.State.Flagged;
            if(MyGrid.Cells[currentCell].isMined)
            {
                minesGuessedCorrectly++;
            }
            totalFlags++;
        }
        /// <summary>
        /// De-flags the current cell. Reverse of Flag().
        /// </summary>
        private void unFlag()
        {
            int currentCell = CellOver;
            MyGrid.Cells[CellOver].CellState = Cell.State.Question;
            if (MyGrid.Cells[currentCell].isMined)
            {
                minesGuessedCorrectly--;
            }
            totalFlags--;
        }
        #endregion

        /// <summary>
        /// What happens when a cell is clicked.
        /// </summary>
        /// <param name="CellOverNow">The index of the cell that was clicked.</param>
        private void ChangeState(int CellOverNow)
        {
            MyGrid.Cells[CellOverNow].StateChanged();
            MyGrid.Cells[CellOverNow].LoadContent(this.Content);
        }
        
        /// <summary>
        /// Converts mouse position to the cell it corresponds to.
        /// </summary>
        /// <param name="MS">Current mouse state.</param>
        /// <returns>The int of the cell.</returns>
        public int IndexFromMouse(MouseState MS)
        {
            int MyX, MyY;
            MyX = 20 * ((int)(MS.X / 20));
            MyY = 20 * ((int)(MS.Y / 20));
            return MyGrid.IndexFromPosition(new Vector2(MyX, MyY));
        }
        
        private void UpdateMouseState()
        {
            MState = Mouse.GetState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            if (MyGameState == GameState.Game || MyGameState == GameState.PreEndGame)
            {
                MyGrid.Draw(spriteBatch);
                DrawGameText();
            }
            else if (MyGameState == GameState.Menu)
            {
                MyMenu.Draw(spriteBatch);
                spriteBatch.DrawString(Font2, "" + userDimension, new Vector2(263, 355), Color.White);
            }
            else if (MyGameState == GameState.EndGame)
            {
                InitGraphicsMode(400, 400);
                MyMenu.Draw(spriteBatch);
                hud = "Time: " + Watch.ElapsedMilliseconds / 100;
                spriteBatch.DrawString(Font, hud, new Vector2(5, 370), Color.White);
                hud2 = "Wins: " + wins + "    Losses: " + losses;
                spriteBatch.DrawString(Font, hud2, new Vector2(220, 370), Color.White);
                
            }

            string temp = "" + Mouse.GetState();
            //spriteBatch.DrawString(Font, temp, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        string debug = "";
        string hud, hud2;
            
        public void DrawGameText()
        {
            MState = Mouse.GetState();
            debug = "";
            debug += "Mouse: " + new Vector2(MState.X, MState.Y);
            debug += "\nCell Over: " + CellOver;
            debug += "\nAdjacents: " + ListToString(MyGrid.getValidAdjacentLocations(CellOver));
            debug += (CellOver < MyGrid.Cells.Length) ? "\nAsset: " + MyGrid.Cells[CellOver].AssetName : "";
            debug += "\nStarted?: " + start;
            //spriteBatch.DrawString(Font, debug, new Vector2(0, 40), Color.White);

            hud = "";
            hud += "Time: " + Watch.ElapsedMilliseconds / 100;
            hud += "      Mines Flagged: " + totalFlags + "/" + MyGrid.totalMines;
            spriteBatch.DrawString(Font, hud, new Vector2(5, MyGrid.CellHeight * MyGrid.dimension), Color.White);
        }
        
        private void InitGraphicsMode(int iWidth, int iHeight)
        {
            graphics.PreferredBackBufferHeight = iHeight;
            graphics.PreferredBackBufferWidth = iWidth;
            graphics.ApplyChanges();
        }

        private string ListToString(List<int> l)
        {
            string str = "";
            foreach (int i in l)
            {
                str += i + " ";
            }
            return str;
        }
    }
}
