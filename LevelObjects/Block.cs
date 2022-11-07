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
    class Block : SpriteGameObject
    {
        public Point gridPosition { get; set; }
        Level level;
        public bool LockedIn { get; set; }
        public Block(Level level, Point gridPosition, int spriteIndex) : base("Sprites/blocks@7x1", .5f, spriteIndex)
        {
            this.gridPosition = gridPosition;
            this.level = level;
            LockedIn = false;
            ApplyCurrentPosition();
        }

        public void ApplyCurrentPosition()
        {
            //if (level.IsOnGrid(gridPosition))
            //    level.RemoveBlockFromGrid(gridPosition, this);

            level.AddBlockToGrid(gridPosition, this);
            LocalPosition = level.GetCellPosition(gridPosition.X, gridPosition.Y);
        }

       

        public bool CanMoveInDirection(Point direction)
        {
            if (!Active || !Visible)
                return false;
            
            Point nextPosition = gridPosition + direction;
            //Block nextBlock = level.GetBlock(nextPosition);

            if (nextPosition.X < 0 || nextPosition.X >= level.GridWidth ||
                nextPosition.Y < 0 || nextPosition.Y >= level.GridHeight)
                return false;



                 if (level.PositionHasBlock(nextPosition) && level.GetBlock(nextPosition).LockedIn)
                    return false;
            


            return true;
                
        }

        public void MoveInDirection(Point direction)
        {
            Point previousPosition = gridPosition;

            gridPosition += direction;
          

            level.RemoveBlockFromGrid(previousPosition);
            ApplyCurrentPosition();
        }

        public int GetDropDistance()
        {
            int distance = 0;
            Point dropDirection = new Point(0, 1);
            Point positionProbe = gridPosition;

            while (CanMoveInDirection(dropDirection))
            {
                distance++;
                gridPosition += dropDirection;
            }

            gridPosition = positionProbe;

            return distance;
        }
    }
}
