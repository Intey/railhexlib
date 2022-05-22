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

        public HexNode FindCell(Cell node)
        {
            return findCell(node, null);
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

        private HexNode findCell(Cell node, IdentityCell fromSide) {
            if (Cell.Equals(node)) { return this; }
            else {
                HexNode found = null;
                if ( Left != null && (fromSide == null || !fromSide.Equals(IdentityCell.leftSide))) {
                    found = Left.findCell(node, IdentityCell.rightSide);
                    if (found != null) return found;
                }
                if (UpLeft != null && (fromSide == null || !fromSide.Equals(IdentityCell.upLeftSide))) {
                    found = UpLeft.findCell(node, IdentityCell.downRightSide);
                    if (found != null) return found;
                }
                if (UpRight != null && (fromSide == null || !fromSide.Equals(IdentityCell.upRightSide))) {
                    found = UpRight.findCell(node, IdentityCell.downLeftSide);
                    if (found != null) return found;
                }
                if (Right != null && (fromSide == null || !fromSide.Equals(IdentityCell.rightSide))) {
                    found = Right.findCell(node, IdentityCell.leftSide);
                    if (found != null) return found;
                }
                if (DownRight != null && (fromSide == null || !fromSide.Equals(IdentityCell.downRightSide))) {
                    found = DownRight.findCell(node, IdentityCell.upLeftSide);
                    if (found != null) return found;
                }
                if (DownLeft != null && (fromSide == null || !fromSide.Equals(IdentityCell.downLeftSide))) {
                    found = DownLeft.findCell(node, IdentityCell.upRightSide);
                    if (found != null) return found;
                }
                return null;
            }
        }
    }
}
