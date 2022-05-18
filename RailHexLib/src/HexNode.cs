using RailHexLib.Grounds;
using System;
using System.Collections.Generic;

namespace RailHexLib
{
    public class HexNode
    {
        public Cell Cell;
        // links
        private HexNode left;
        private HexNode upLeft;
        private HexNode upRight;
        private HexNode right;
        private HexNode downRight;
        private HexNode downLeft;

        public HexNode(Cell c)
        {
            Cell = c;
        }

        public HexNode Left
        {
            get => left;
            set
            {
                left = value;
                value.right = this;
            }
        }
        public HexNode UpLeft
        {
            get => upLeft;
            set
            {
                upLeft = value;
                value.downRight = this;
            }
        }
        public HexNode UpRight
        {
            get => upRight;
            set
            {
                upRight = value;
                value.downLeft = this;
            }
        }
        /// <summary>
        /// Adds a newNode to self if possible. 
        /// Changes the newNode related side link to the parent that adopts it.
        /// </summary>
        /// <param name="newNode">The node to add</param>
        /// <returns>parent node that adopt newNode. If not adopted - return null</returns>
        /// <exception cref="NotImplementedException"></exception>
        public HexNode Add(HexNode newNode)
        {
            //check direction, and distance. 
            // if distance == 1, than place by direction
            // else - move in given direction
            // return parent node in which we place newRoadCell
            throw new NotImplementedException();
        }

        public HexNode FindCell(Cell node)
        {
            throw new NotImplementedException();
        }


        public HexNode Right
        {
            get => right;
            set
            {
                right = value;
                value.left = this;
            }
        }
        public HexNode DownRight
        {
            get => downRight;
            set
            {
                downRight = value;
                value.upLeft = this;
            }
        }
        public HexNode DownLeft
        {
            get => downLeft;
            set
            {
                downLeft = value;
                value.upRight = this;
            }
        }

        internal List<Cell> PathTo(Cell joineryCell)
        {
            throw new NotImplementedException();
        }
    }
}
