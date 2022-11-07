using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.LevelObjects
{
    internal class Level : GameObjectList
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

        public int GridWidth { get { return blockGrid.GetLength(0); } }
        public int GridHeight { get { return blockGrid.GetLength(1); } }
        
        Shape shape;

        bool isRunning = false;

        public int Score { get; set; }


        TextGameObject debugFont = new TextGameObject("Fonts/DebugFont", 1f, Color.White);
        public Level()
        {
            playingField.LocalPosition = new Vector2(50, 50);

            playingField.AddChild(playingFieldBackground);

            blockGrid = new Block[10, 20];

            AddChild(debugFont);

            AddChild(playingField);


            Reset();


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //debugFont.Text = $"Target Rows to Clear: {targetRowsToClear} \n Rows Cleared: {totalRowsCleared} \n Timer: {descensionTimer}";
            debugFont.Text = $"Score: {Score}";

            if (shape != null && shape.CurrentShapeState == Shape.ShapeState.ShapeDisabled)
            {
                RemoveShape();

                int rowsCleared = 0;
                int spacesBelow = 0;

                for (int y = GridHeight - 1; y >= 0; y--)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        DetectHorizontalMatch(0, y, ref rowsCleared);
                        
                        
                        if (rowsCleared > 0)
                        {
                            MoveDown(x, y, ref spacesBelow);
                        }
                    }
                }

                AddToScore(ref rowsCleared);

                totalRowsCleared += rowsCleared;


                if (totalRowsCleared >= targetRowsToClear)
                {
                    targetRowsToClear += 10;
                    level++;
                    descensionTimer -= .0425f;
                }

                if ((PositionHasBlock(new Point(5, 0)) ||
                    PositionHasBlock(new Point(6, 0))))
                {
                     //if (isRunning)
                     //   LoadShape();

                    isRunning = false;
                    
                } else
                {
                    LoadShape();
                }
                
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (!isRunning && inputHelper.KeyPressed(Keys.Enter))
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

        void DetectHorizontalMatch(int x, int y, ref int rowsCleared)
        {
 
            for (int i = 0; i < GridWidth; i++)
            {
                if (blockGrid[i, y] == null)
                    return;
            }


            for (int i = 0; i < GridWidth; i++)
                blockGrid[i, y] = null;
                    

            rowsCleared++;
        }

        void MoveDown(int x, int y, ref int spacesBelow)
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

        private void AddToScore(ref int rowsCleared)
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
            isRunning = true;
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
