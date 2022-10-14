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

        public int GridWidth { get { return blockGrid.GetLength(0); } }
        public int GridHeight { get { return blockGrid.GetLength(1); } }
        int blockCount = 0;
        
        Shape shape;
        List<Block> blockCollection = new List<Block>();

        int score = 0;

        TextGameObject debugFont = new TextGameObject("Fonts/DebugFont", 1f, Color.White);
        public Level()
        {
            playingField.LocalPosition = new Vector2(170, 80);

            playingField.AddChild(playingFieldBackground);

            blockGrid = new Block[10, 20];

            AddChild(debugFont);

            AddChild(playingField);

            LoadShape();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            
            debugFont.Text = score.ToString();

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
                            MoveDown(x, y, ref spacesBelow);
                    }
                        
                }

                score += rowsCleared;

                LoadShape();
                
            }



        }

        void LoadShape()
        {

            if (!PositionHasBlock(new Point(5, 0)))
            {
                shape = new Shape(this, new Point(5, 0), ExtendedGame.Random.Next(7));
                playingField.AddChild(shape);
            } else
                ExtendedGame.BackgroundColor = Color.White;

            
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

            //if (blockGrid[x, y].CanMoveInDirection(new Point(0, rowsCleared)) &&
            //    blockGrid[x, y] is Block)
            //{
            //    blockGrid[x, y].MoveInDirection(new Point(0, rowsCleared));
            //}


            int horizontalSpacesClear = 0;
            if (x == 0 && y < GridHeight - 1)
            {
                
                for (int i = 0; i < 10; i++)
                {
                    
                    if (blockGrid[i, y + spacesBelow + 1] == null)
                        horizontalSpacesClear++;

                    if (horizontalSpacesClear == 10)
                    {
                        if ((y + spacesBelow + 1) < 19)
                            i = 0;

                        horizontalSpacesClear = 0;
                        spacesBelow++;
                    }
                    
                   
                    
                }
            }

            if (spacesBelow > 0)
            {
                if (blockGrid[x, y] is Block)
                    blockGrid[x, y].MoveInDirection(new Point(0, spacesBelow));



            }

            

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
