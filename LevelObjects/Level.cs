using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.LevelObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.LevelObjects
{
    internal class Level : GameState
    {
        SpriteGameObject playingFieldBackground = new SpriteGameObject("Sprites/playingField", .25f);
        GameObjectList playingField = new GameObjectList();

        Block[,] blockGrid { get; set; }

        const int TileWidth = 32;
        const int TileHeight = 32;

        int totalRowsCleared;
        int targetRowsToClear;
        int level;
        float descensionTimer;
        int rowsCleared = 0;
        int spacesBelow = 0;
        float startRowMatchDetectionTimer, rowMatchDetectionTimer = .5f;

        public int GridWidth { get { return blockGrid.GetLength(0); } }
        public int GridHeight { get { return blockGrid.GetLength(1); } }
        
        Shape shape;

        TextGameObject scoreFont = new TextGameObject("Fonts/Score", 1f, Color.White, TextGameObject.Alignment.Center);
        TextGameObject levelFont = new TextGameObject("Fonts/Score", 1f, Color.White, TextGameObject.Alignment.Center);
        TextGameObject linesFont = new TextGameObject("Fonts/Score", 1f, Color.White, TextGameObject.Alignment.Center);

        public Process CurrentProcess { get; set; }
        
        public enum Process
        {
            Running,
            Stopped,
            Paused,
        }

        public int Score { get; set; }


        TextGameObject debugFont = new TextGameObject("Fonts/DebugFont", 1f, Color.White);
        public Level()
        {
            playingField.LocalPosition = new Vector2(50, 50);

            playingField.AddChild(playingFieldBackground);

            blockGrid = new Block[10, 20];

            startRowMatchDetectionTimer = rowMatchDetectionTimer;

            gameObjects.AddChild(debugFont);

            gameObjects.AddChild(playingField);

            Reset();

            gameObjects.AddChild(scoreFont);
            scoreFont.LocalPosition = new Vector2(500, 100);

            gameObjects.AddChild(levelFont);
            levelFont.LocalPosition = new Vector2(500, 300);

            gameObjects.AddChild(linesFont);
            linesFont.LocalPosition = new Vector2(500, 500);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            debugFont.Text = $"Level: {level}\ntarget rows: {targetRowsToClear}\ntimer: {descensionTimer}";
            if (shape != null && shape.CurrentShapeState == Shape.ShapeState.ShapeDisabled)
            {
                RemoveShape();

                for (int y = GridHeight - 1; y >= 0; y--)
                    for (int x = 0; x < GridWidth; x++)
                        DetectHorizontalMatch(x, y);


                if (CurrentProcess != Process.Paused && rowsCleared == 0)
                {
                    if (PositionHasBlock(new Point(5, 0)) ||
                         PositionHasBlock(new Point(6, 0)))
                    {
                        //if (isRunning)
                        //   LoadShape();

                        CurrentProcess = Process.Stopped;

                    }
                    else
                    {
                        LoadShape();
                    }
                }
            }

            if (rowsCleared > 0)
            {
                rowMatchDetectionTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (rowMatchDetectionTimer <= 0)
                {
                    for (int y = GridHeight - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < GridWidth; x++)
                        {
                            if (blockGrid[x, y] != null && blockGrid[x, y].Color != Color.White)
                                blockGrid[x, y] = null;
                        }
                    }

                    for (int y = GridHeight - 1; y >= 0; y--)
                        for (int x = 0; x < GridWidth; x++)
                            MoveDown(x, y);

                      
                    totalRowsCleared += rowsCleared;

                    AddToScore();

                    if (totalRowsCleared >= targetRowsToClear)
                    {
                        targetRowsToClear += 10;
                        level++;
                        descensionTimer -= .0425f;
                    }

                    spacesBelow = 0;
                    rowsCleared = 0;
                    rowMatchDetectionTimer = startRowMatchDetectionTimer;
                    LoadShape();
                }

            } 

            scoreFont.Text = $"Score: \n   {Score}";
            levelFont.Text = $"Level: \n   {level}";
            linesFont.Text = $"Lines: \n   {totalRowsCleared}";
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (CurrentProcess != Process.Running && inputHelper.KeyPressed(Keys.Enter))
            {
                Reset();
            }
        }

        void LoadShape()
        {
            shape = new Shape(this, new Point(5, 0), ExtendedGame.Random.Next(7), descensionTimer);
            playingField.AddChild(shape);
        }

        void RemoveShape()
        {
            playingField.RemoveChild(shape);
            shape = null;

        }

        void DetectHorizontalMatch(int x, int y)
        {
 
            for (int i = 0; i < GridWidth; i++)
            {
                if (blockGrid[i, y] == null)
                    return;
            }

            for (int i = 0; i < GridWidth; i++)
            {
                blockGrid[i, y].Color = Color.LightGray;

            }

            if (x == 0)
                rowsCleared++;
        }

        void MoveDown(int x, int y)
        {

            int horizontalSpacesClear = 0;
            if (x == 0 && y < GridHeight - 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (blockGrid[i, y + spacesBelow + 1] == null)
                        horizontalSpacesClear++;

                    if (horizontalSpacesClear == GridWidth)
                    {
                        if ((y + spacesBelow + 1) < 19)
                            i = 0;

                        horizontalSpacesClear = 0;
                        spacesBelow++;
                    }
                }
            }

            
            if (spacesBelow > 0 && blockGrid[x, y] != null)
                    blockGrid[x, y].MoveInDirection(new Point(0, spacesBelow));



        }

        private void AddToScore()
        {
            int lineScore = 0;
            switch (rowsCleared)
            {
                case 1:
                    lineScore = 40;
                    break;
                case 2:
                    lineScore = 100;
                    break;
                case 3:
                    lineScore = 300;
                    break;
                case 4:
                    lineScore = 1200;
                    break;
            }

            int levelScore = lineScore * level;

            Score += levelScore;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (blockGrid[x, y] is Block)
                        blockGrid[x, y].Draw(gameTime, spriteBatch);
                }
            }
        }

        public override void Reset()
        {
            base.Reset();

            totalRowsCleared = 0;
            targetRowsToClear = 10;
            descensionTimer = 1;
            level = 1;
            CurrentProcess = Process.Running;
            Score = 0;

            for (int y = 0; y < GridHeight; y++)
                for (int x = 0; x < GridWidth; x++)
                    blockGrid[x, y] = null;

            LoadShape();
        }

        public Vector2 GetCellPosition(int x, int y)
        {
            return new Vector2(x * TileWidth, y * TileHeight);
        }

        public void AddBlockToGrid(Point gridPosition, Block block)
        {
            blockGrid[gridPosition.X, gridPosition.Y] = block;
        }

        public void RemoveBlockFromGrid(Point gridPosition)
        {

            blockGrid[gridPosition.X, gridPosition.Y] = null;
        }

        public bool PositionOnGrid(Point gridPosition)
        {
            return gridPosition.X >= 0 && gridPosition.X <= GridWidth 
                && gridPosition.Y >= 0 && gridPosition.Y <= GridHeight;
        }

        public bool PositionHasBlock(Point gridPosition)
        {
            return blockGrid[gridPosition.X, gridPosition.Y] is Block;
        }

        public bool IsOnGridEdge(Point gridPosition)
        {
            return gridPosition.X == 0 || gridPosition.X == GridWidth - 1 ||
                gridPosition.Y == 0 || gridPosition.Y == GridHeight - 1;
        }

        public Block GetBlock(Point direction)
        {
            if (!PositionOnGrid(direction))
                return null;

            return blockGrid[direction.X, direction.Y];
        }
    }
}
