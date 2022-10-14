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
    internal class Shape : GameObject
    {
        public Block[] Blocks { get; set; } = new Block[4];
        Point gridPosition;
        Level level;
        protected Point[,,] blockPlacement;
        int rotationIndex = 1;
        float timer = 1f, startTimer;
        bool speedDown = false;
        protected int blockIndex;
        public enum ShapeState
        {
            ShapeActive, 
            ShapeInactive,
            ShapeDisabled
        }

        public ShapeState CurrentShapeState { get; set; } = ShapeState.ShapeActive;

        public Shape(Level level, Point gridPosition, int blockIndex)
        {
            this.level = level;
            this.gridPosition = gridPosition;
            this.blockIndex = blockIndex;
            startTimer = timer;

            InitializeBlocks();
            
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);


            if (inputHelper.KeyPressed(Keys.Left))
            {
                

                //Make sure that each block is able to move in that direction before moving all of them.
                //If one of them can't move (because of sepcified conditions) then none of them will. 

                
                for (int x = 0; x < 4; x++)
                {
                    if (!Blocks[x].CanMoveInDirection(new Point(-1, 0)))
                        return;
                }
                
                //If all of the blocks are able to move in the desired direction, then move them there and register their position. 
                if (Blocks[0].gridPosition.X > Blocks[3].gridPosition.X)
                {
                    for (int x = 3; x >= 0; x--)
                    {
                        Blocks[x].MoveInDirection(new Point(-1, 0));
                    }
                    gridPosition += new Point(-1, 0);
                } else
                {
                    if (blockIndex == 5 && rotationIndex == 2)
                    {
                        int[] newOrder = { 0, 3, 1, 2 };
                        for (int x = newOrder.Length - 1; x >= 0; x--)
                        {
                            Blocks[newOrder[x]].MoveInDirection(new Point(-1, 0));
                        }
                    } else
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            Blocks[x].MoveInDirection(new Point(-1, 0));
                        }
                    }

                    gridPosition += new Point(-1, 0);
                }

            }

            if (inputHelper.KeyPressed(Keys.Right))
            {
                //Make sure that each block is able to move in that direction before moving all of them.
                //If one of them can't move (because of sepcified conditions) then none of them will. 
                for (int x = 0; x < 4; x++)
                {
                    if (!Blocks[x].CanMoveInDirection(new Point(1, 0)))
                        return;
                }

                //If all of the blocks are azble to move in the desired direction, then move them there and register their position. 
                if (Blocks[0].gridPosition.X > Blocks[3].gridPosition.X)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        Blocks[x].MoveInDirection(new Point(1, 0));
                    }
                    gridPosition += new Point(1, 0);
                } else
                {
                    if (blockIndex == 5 && rotationIndex == 2)
                    {
                        int[] newOrder = { 0, 3, 1, 2 };
                        for (int x = 0; x < newOrder.Length; x++)
                        {
                            Blocks[newOrder[x]].MoveInDirection(new Point(1, 0));
                        }
                    } else
                    {
                        for (int x = 3; x >= 0; x--)
                        {
                            Blocks[x].MoveInDirection(new Point(1, 0));
                        }
                    }

                    gridPosition += new Point(1, 0);
                }

            }

            if (inputHelper.KeyDown(Keys.Down))
            {
                if (!speedDown)
                    timer = 0;

                speedDown = true;
                
            }   
            else
                speedDown = false;

            //if (CurrentShapeState == ShapeState.ShapeDisabled)
            //    return;

            if (inputHelper.KeyPressed(Keys.Space))
            {

                //Calculates the difference in position between the position you intend the block to move to...
                //and the position the block is already in, when trying to rotate.
                Point difference = Point.Zero;

                void Difference(int num)
                {
                    difference = (gridPosition + blockPlacement[blockIndex, rotationIndex, num]) - (Blocks[num].gridPosition);
                }


                //Make sure that each block is able to move in that direction before moving all of them.
                //If one of them can't move (because of sepcified conditions) then none of them will. 
                for (int x = 0; x < 4; x++)
                {
                    Difference(x);
                    if (!Blocks[x].CanMoveInDirection(difference))
                        return; //returns method to prevent the blocks from moving (which is done in the for-loop below)
                }

                //If all of the blocks are able to move in the desired direction, then move them there and register their position. 
                if (blockIndex < 6)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Difference(i);
                        Blocks[i].MoveInDirection(difference);
                    }
                }
                else
                {
                    for (int i = 3; i >= 0; i--)
                    {
                        Difference(i);
                        Blocks[i].MoveInDirection(difference);
                    }
                }


                //Cycle through the rotation index which sets the next position of block placements
                if (rotationIndex < 3)
                    rotationIndex++;
                else
                    rotationIndex = 0;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            
           
            if (CurrentShapeState != ShapeState.ShapeDisabled)
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;


            for (int x = 0; x < 4; x++)
            {

                if (!Blocks[x].CanMoveInDirection(new Point(0, 1)))
                {
                    CurrentShapeState = ShapeState.ShapeInactive;
                    break;  //break the loop because if one block can't move down then the whole shape can'
                } else
                {
                    CurrentShapeState = ShapeState.ShapeActive;
                }
            }

            if (timer <= 0)
            {
                if (CurrentShapeState == ShapeState.ShapeInactive)
                {
                    for (int x = 0; x < 4; x++)
                        Blocks[x].LockedIn = true;

                    CurrentShapeState = ShapeState.ShapeDisabled;

                    return;
                    //Return so it doesn't needlessly execute the rest of the code block
                    //and the reference to this object can be deleted from the level class
                    //so garbage collection can get rid of it
                }


                if (Blocks[0].gridPosition.Y > Blocks[3].gridPosition.Y)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Blocks[i].MoveInDirection(new Point(0, 1));
                    }
                    gridPosition += new Point(0, 1);
                } else
                {
                    if (blockIndex == 5 && rotationIndex == 3)
                    {
                        int[] newOrder = { 0, 3, 1, 2 };
                        for (int x = 0; x < newOrder.Length; x++)
                        {
                            Blocks[newOrder[x]].MoveInDirection(new Point(0, 1));
                        }
                    } else
                    {
                        for (int i = 3; i >= 0; i--)
                        {
                            Blocks[i].MoveInDirection(new Point(0, 1));
                        }
                    }

                    gridPosition += new Point(0, 1);
                }

                if (!speedDown)
                    timer = startTimer;
                else
                    timer = startTimer / 10;
            }
        }


        protected void InitializeBlocks()
        {
            BlockPlacements();

            Blocks = new Block[4] { new Block(level, gridPosition + blockPlacement[blockIndex, 0, 0], blockIndex),
                                   new Block(level, gridPosition + blockPlacement[blockIndex, 0, 1], blockIndex),
                                   new Block(level, gridPosition + blockPlacement[blockIndex, 0, 2], blockIndex),
                                   new Block(level, gridPosition + blockPlacement[blockIndex, 0, 3], blockIndex) };

            foreach (Block block in Blocks)
                block.Parent = this;
        }

        void BlockPlacements()
        {
            //The relative positions of the tetromino when it rotates
            blockPlacement = new Point[,,]
            {

                {
                    { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(1, 1) },
                    { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(-1, 1) },
                    { new Point(1, 0), new Point(0, 0), new Point(-1, 0), new Point(-1, -1)},
                    { new Point(0, 1), new Point(0, 0), new Point(0, -1), new Point(1, -1)}
                },
                {

                    { new Point(1, 0), new Point(0, 0), new Point(-1, 0), new Point(-1, 1)},
                    { new Point(0, 1), new Point(0, 0), new Point(0, -1), new Point(-1, -1)},
                    { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(1, -1) },
                    { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(1, 1) }
                },

                {
                    { new Point(-1, 0), new Point(0, 0), new Point(-1, 1), new Point(0, 1) },
                    { new Point(-1, 0), new Point(0, 0), new Point(-1, 1), new Point(0, 1) },
                    { new Point(-1, 0), new Point(0, 0), new Point(-1, 1), new Point(0, 1) },
                    { new Point(-1, 0), new Point(0, 0), new Point(-1, 1), new Point(0, 1) }
                },

                {
                    { new Point(-1, 0), new Point(0, 0), new Point(0, 1), new Point(1, 1) },
                    { new Point(0, -1), new Point(0, 0), new Point(-1, 0), new Point(-1, 1) },
                    { new Point(1, 0), new Point(0, 0), new Point(0, -1), new Point(-1, -1) },
                    { new Point(0, 1), new Point(0, 0), new Point(1, 0), new Point(1, -1) }
                },
                {
                    { new Point(1, 0), new Point(0, 0), new Point(-1, 0), new Point(-2, 0) },
                    { new Point(0, 2), new Point(0, 1), new Point(0, 0), new Point(0, -1) },
                    { new Point(-2, 1), new Point(-1, 1), new Point(0, 1), new Point(1, 1) },
                    { new Point(-1, -1), new Point(-1, 0), new Point(-1, 1), new Point(-1, 2) }
                },
                {
                    { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(0, 1) },
                    { new Point(0, -1), new Point(0, 0), new Point(-1, 0), new Point(0, 1)},
                    { new Point(1, 0), new Point(0, 0), new Point(0, -1), new Point(-1, 0)},
                    { new Point(0, 1), new Point(0, 0), new Point(1, 0), new Point(0, -1) },
                },
                {
                    { new Point(1, 0), new Point(0, 0), new Point(0, 1), new Point(-1, 1) },
                    { new Point(0, 1), new Point(0, 0), new Point(-1, 0), new Point(-1, -1) },
                    { new Point(-1, 0), new Point(0, 0), new Point(0, -1), new Point(1, -1) },
                    { new Point(0, -1), new Point(0, 0), new Point(1, 0), new Point(1, 1) }
                },
            };
        }

    }
}
