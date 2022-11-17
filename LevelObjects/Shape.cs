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
        float timer, startTimer;
        bool speedDown = false;
        float timeToMoveHorizontally, startTimeToMoveHorizontally, moveHorizontalTime, startMoveHorizontalTime;
      
        protected int blockIndex;
        bool isLeftKeyHeld = false;
        bool isRightKeyHeld = false;

        public enum ShapeState
        {
            ShapeActive, 
            ShapeInactive,
            ShapeDisabled
        }

        public ShapeState CurrentShapeState { get; set; } = ShapeState.ShapeActive;

        public Shape(Level level, Point gridPosition, int blockIndex, float timer)
        {
            this.level = level;
            this.gridPosition = gridPosition;
            this.blockIndex = blockIndex;
            this.timer = timer;
            startTimer = timer;

            timeToMoveHorizontally = .175f;
            startTimeToMoveHorizontally = timeToMoveHorizontally;

            moveHorizontalTime = .1f;
            startMoveHorizontalTime = moveHorizontalTime;

            InitializeBlocks();
            
        }



        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);


            if (inputHelper.KeyPressed(Keys.Left))
                HorizontalMovement(new Point(-1, 0));


            //A round-about way of triggering the rapid horizontal movement functionality
            //Still needs to be done this way because I can only use the gameTime variable in the update methodfwe
            if (inputHelper.KeyDown(Keys.Left))
                isLeftKeyHeld = true;
            else if (isLeftKeyHeld)
                isLeftKeyHeld = false;


            if (inputHelper.KeyPressed(Keys.Right))
                HorizontalMovement(new Point(1, 0));

            if (inputHelper.KeyDown(Keys.Right))
                isRightKeyHeld = true;
            else if (isRightKeyHeld)
                isRightKeyHeld = false;


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

            if (inputHelper.KeyPressed(Keys.Up))
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

            //Hard Drop
            if (inputHelper.KeyPressed(Keys.Space))
            {
                int[] dropDistances = new int[4];
                for (int x = 0; x < 4; x++)
                {
                    dropDistances[x] = Blocks[x].GetDropDistance();

                }

                Array.Sort(dropDistances);

                int minDistance = dropDistances[0];

                for (int x = 0; x < 4; x++)
                {
                    Blocks[x].MoveInDirection(new Point(0, minDistance));

                }
                timer = 0;
                level.Score += 2 * minDistance;

            }

        }
        


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            RapidHorizontalMovement(gameTime, isLeftKeyHeld, isRightKeyHeld, new Point(-1, 0));
            RapidHorizontalMovement(gameTime, isRightKeyHeld, isLeftKeyHeld, new Point(1, 0));

            if (!isRightKeyHeld && !isLeftKeyHeld)
                ResetRapidMovement();

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
                {
                    timer = startTimer / 10;
                    level.Score += 1;
                }
                    
            }
        }

        private void HorizontalMovement(Point direction)
        {
            //Make sure that each block is able to move in that direction before moving all of them.
            //If one of them can't move (because of sepcified conditions) then none of them will.

            for (int x = 0; x < 4; x++)
            {
                if (!Blocks[x].CanMoveInDirection(direction))
                    return;
            }

            //If all of the blocks are able to move in the desired direction, then move them there and register their position. 
            if (Blocks[0].gridPosition.X > Blocks[3].gridPosition.X)
            {
                if (direction.X < 0)
                {
                    for (int x = 3; x >= 0; x--)
                        Blocks[x].MoveInDirection(direction);
                }
                else
                {
                    for (int x = 0; x < 4; x++)
                        Blocks[x].MoveInDirection(new Point(1, 0));
                }

                gridPosition += direction;
            }
            else
            {
                if (blockIndex == 5 && rotationIndex == 2)
                {
                    int[] newOrder = { 0, 3, 1, 2 };
                    if (direction.X < 0)
                    {
                        for (int x = newOrder.Length - 1; x >= 0; x--)
                            Blocks[newOrder[x]].MoveInDirection(direction);
                    }
                    else
                    {
                        for (int x = 0; x < newOrder.Length; x++)
                            Blocks[newOrder[x]].MoveInDirection(direction);
                    }
                }
                else
                {
                    if (direction.X < 0)
                    {
                        for (int x = 0; x < 4; x++)
                            Blocks[x].MoveInDirection(direction);
                    }
                    else
                    {
                        for (int x = 3; x >= 0; x--)
                            Blocks[x].MoveInDirection(direction);
                    }

                }

                gridPosition += direction;
            }
        }


        private void RapidHorizontalMovement(GameTime gameTime, bool keyHeld, bool otherKeyHeld, Point direction)
        {
            if (keyHeld)
            {
                if (otherKeyHeld)
                {
                    ResetRapidMovement();
                    return;
                }

                timeToMoveHorizontally -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                keyHeld = true;

                if (timeToMoveHorizontally <= 0)
                {
                    if (moveHorizontalTime <= 0)
                    {
                        HorizontalMovement(direction);
                        moveHorizontalTime = startMoveHorizontalTime;
                    }
                    moveHorizontalTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        private void ResetRapidMovement()
        {
            timeToMoveHorizontally = startTimeToMoveHorizontally;
            moveHorizontalTime = startMoveHorizontalTime;
            isRightKeyHeld = false;
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

        public GameState Level { get { return (Level)ExtendedGame.GameStateManager.GetGameState(Game1.STATE_PLAYINGSCENE); } }

    }
}
