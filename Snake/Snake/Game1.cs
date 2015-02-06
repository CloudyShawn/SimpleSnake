/**************************************************************
 * Project Name:        Snake
 * Creater:             CloudyShawn
 * Last Modified Date:  06/02/2015
 * Description:         Basic original snake game, currently no
 *                  menus are implemented but game is fully 
 *                  functional. Map sizes and block sizes are
 *                  editable as well. Score working.
 *************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Snake
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont TimesRoman;

        const int FRAME = 50;
        int frameCounter;

        KeyboardState curKeyboardState;

        enum GameState
        {
            GAME,
            LOAD,
            MENU,
            HIGHSCORE
        }

        GameState gameState;

        enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
        }

        int[][] map;
        int blockSize;
        Texture2D whiteBlock;

        int snakeSize;
        Direction curDir;
        Direction oldDir;
        Vector2 snakePos;

        Random random;
        bool isNoFood;
        Vector2 foodPos;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            gameState = GameState.LOAD;
            isNoFood = true;
            random = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            whiteBlock = Content.Load<Texture2D>("white block");
            TimesRoman = Content.Load<SpriteFont>("TimesNewRoman");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            whiteBlock.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            switch (gameState)
            {
                case GameState.GAME:
                    UpdateGame(gameTime);
                    break;
                case GameState.HIGHSCORE:
                    UpdateHighScore();
                    break;
                case GameState.LOAD:
                    NewGame(60, 40, 10);
                    break;
                case GameState.MENU:
                    UpdateMenu();
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateMenu()
        {
            
        }

        private void NewGame(int width, int height, int size)
        {
            snakeSize = 1;

            map = new int[width][];
            for (int i = 0; i < width; i++)
            {
                map[i] = new int[height];
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[i][j] = 0;
                }
            }

            snakePos = new Vector2(width / 2, height / 2);
            map[(int)snakePos.X][(int)snakePos.Y] = snakeSize;

            curDir = Direction.UP;
            isNoFood = true;
            blockSize = size;

            graphics.PreferredBackBufferWidth = width * blockSize;
            graphics.PreferredBackBufferHeight = height * blockSize;
            graphics.ApplyChanges();

            gameState = GameState.GAME;
        }

        private void UpdateGame(GameTime gameTime)
        {
            curKeyboardState = Keyboard.GetState();
            if (curKeyboardState.IsKeyDown(Keys.Left) && oldDir != Direction.RIGHT)
            {
                curDir = Direction.LEFT;
            }
            else if (curKeyboardState.IsKeyDown(Keys.Right) && oldDir != Direction.LEFT)
            {
                curDir = Direction.RIGHT;
            }
            else if (curKeyboardState.IsKeyDown(Keys.Up) && oldDir != Direction.DOWN)
            {
                curDir = Direction.UP;
            }
            else if (curKeyboardState.IsKeyDown(Keys.Down) && oldDir != Direction.UP)
            {
                curDir = Direction.DOWN;
            }

            while (isNoFood)
            {
                foodPos = new Vector2((int)random.Next(0, map.Length), (int)random.Next(0, map[0].Length));

                if (map[(int)foodPos.X][(int)foodPos.Y] == 0)
                {
                    map[(int)foodPos.X][(int)foodPos.Y] = -1;
                    isNoFood = false;
                }
            }

            frameCounter += gameTime.ElapsedGameTime.Milliseconds;

            if (frameCounter >= FRAME)
            {
                switch (curDir)
                {
                    case Direction.DOWN:
                        if (snakePos.Y == map[0].Length - 1)
                        {
                            snakePos.Y = 0;
                        }
                        else
                        {
                            snakePos.Y += 1;
                        }
                        break;
                    case Direction.LEFT:
                        if (snakePos.X == 0)
                        {
                            snakePos.X = map.Length - 1;
                        }
                        else
                        {
                            snakePos.X -= 1;
                        }
                        break;
                    case Direction.RIGHT:
                        if (snakePos.X == map.Length - 1)
                        {
                            snakePos.X = 0;
                        }
                        else
                        {
                            snakePos.X += 1;
                        }
                        break;
                    case Direction.UP:
                        if (snakePos.Y == 0)
                        {
                            snakePos.Y = map[0].Length - 1;
                        }
                        else
                        {
                            snakePos.Y -= 1;
                        }
                        break;
                }

                if (map[(int)snakePos.X][(int)snakePos.Y] == -1)
                {
                    snakeSize++;
                    isNoFood = true;
                }
                else if (map[(int)snakePos.X][(int)snakePos.Y] > 0)
                {
                    gameState = GameState.LOAD;
                }
                else 
                {
                    UpdateMap();
                }
                map[(int)snakePos.X][(int)snakePos.Y] = snakeSize;

                frameCounter -= FRAME;

                oldDir = curDir;
            }
        }

        private void UpdateHighScore()
        {

        }

        private void UpdateMap()
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i][j] > 0)
                    {
                        map[i][j]--;
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.GAME:
                    DrawGame(spriteBatch);
                    break;
                case GameState.HIGHSCORE:
                    DrawHighScore();
                    break;
                case GameState.LOAD:
                    DrawMenu();
                    break;
                case GameState.MENU:
                    DrawMenu();
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawMenu()
        {

        }

        private void DrawGame(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i][j] != 0)
                    {
                        spriteBatch.Draw(whiteBlock, new Vector2(i * blockSize, j * blockSize), new Rectangle(0, 0, blockSize, blockSize), Color.White);
                    }
                }
            }

            spriteBatch.DrawString(TimesRoman, "Score: " + snakeSize.ToString(), new Vector2(10, 10), Color.Black);
        }

        private void DrawHighScore()
        {

        }
    }
}
