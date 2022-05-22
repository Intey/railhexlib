// using RailHexLib.Grounds;
using System;
using System.Collections.Generic;

namespace RailHexLib
{
    public class HexNode
    {
        public Cell Cell;
        // links
        private HexNode left = null;
        private HexNode upLeft = null;
        private HexNode upRight = null;
        private HexNode right = null;
        private HexNode downRight = null;
        private HexNode downLeft = null;

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

        /// <summary>
        /// Adds a newNode to self if possible. 
        /// Changes the newNode related side link to the parent that adopts it.
        /// </summary>
        /// <param name="newNode">The node to add</param>
        /// <returns>parent node that adopt newNode. If not adopted - return null</returns>
        public HexNode Add(HexNode newNode)
        {

            if (Cell.DistanceTo(newNode.Cell) == 1)
            {
                var direction = Cell.GetDirectionTo(newNode.Cell);
                if (direction.Equals(IdentityCell.downLeftSide))
                {
                    DownLeft = newNode;
                }
                else if (direction.Equals(IdentityCell.downRightSide))
                {
                    DownRight = newNode;
                }
                else if (direction.Equals(IdentityCell.rightSide))
                {
                    Right = newNode;
                }
                else if (direction.Equals(IdentityCell.leftSide))
                {
                    Left = newNode;
                }
                else if (direction.Equals(IdentityCell.upLeftSide))
                {
                    UpLeft = newNode;
                }
                else if (direction.Equals(IdentityCell.upRightSide))
                {
                    UpRight = newNode;
                }
                return this;
            }
            else
            {
                throw new NotImplementedException("Can't add HexNode to the child node");
            }
        }

        public HexNode FindCell(Cell node, int searchDepth=-1)
        {
            return findCell(node, null, searchDepth);
        }

        internal List<Cell> PathTo(Cell joineryCell)
        {
            var result = new List<Cell>
            {
                Cell
            };

            if (Cell.Equals(joineryCell))
            {
                return result;
            }

            var distance = Cell.DistanceTo(joineryCell);
            var nextCell = Cell.CellLerp(joineryCell, 1.0f / distance);

            var node = FindCell(nextCell, 1); // should check only children
            if (node == null) throw new NotFiniteNumberException();

            result.AddRange(node.PathTo(joineryCell));
            return result;
        }

        private HexNode findCell(Cell node, IdentityCell fromSide, int searchDepth =-1)
        {
            if (Cell.Equals(node)) { return this; }
            if (searchDepth == 0) return null;

            HexNode found;
            if (Left != null && (fromSide == null || !fromSide.Equals(IdentityCell.leftSide)))
            {
                found = Left.findCell(node, IdentityCell.rightSide, --searchDepth);
                if (found != null) return found;
            }
            if (UpLeft != null && (fromSide == null || !fromSide.Equals(IdentityCell.upLeftSide)))
            {
                found = UpLeft.findCell(node, IdentityCell.downRightSide, --searchDepth);
                if (found != null) return found;
            }
            if (UpRight != null && (fromSide == null || !fromSide.Equals(IdentityCell.upRightSide)))
            {
                found = UpRight.findCell(node, IdentityCell.downLeftSide, --searchDepth);
                if (found != null) return found;
            }
            if (Right != null && (fromSide == null || !fromSide.Equals(IdentityCell.rightSide)))
            {
                found = Right.findCell(node, IdentityCell.leftSide, --searchDepth);
                if (found != null) return found;
            }
            if (DownRight != null && (fromSide == null || !fromSide.Equals(IdentityCell.downRightSide)))
            {
                found = DownRight.findCell(node, IdentityCell.upLeftSide, --searchDepth);
                if (found != null) return found;
            }
            if (DownLeft != null && (fromSide == null || !fromSide.Equals(IdentityCell.downLeftSide)))
            {
                found = DownLeft.findCell(node, IdentityCell.upRightSide, --searchDepth);
                if (found != null) return found;
            }
            return null;

        }
    }
}
