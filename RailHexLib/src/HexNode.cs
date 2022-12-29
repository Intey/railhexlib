// using RailHexLib.Grounds;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using PostSharp.Patterns.Diagnostics;

namespace RailHexLib
{
    /// <summary>
    /// Used for the road making.
    /// Previos algorithm use approximation to detect the direction in which 
    /// it's go. But this didn't works with the loops: road starts on the 
    /// left and go to the right. 
    /// Working approach - go over all cell and find a way in the PathTo.
    /// 
    /// </summary>
    public class HexNode : IEnumerable<HexNode>
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
                if (direction.Equals(IdentityCell.bottomLeftSide))
                {
                    DownLeft = newNode;
                }
                else if (direction.Equals(IdentityCell.bottomSide))
                {
                    DownRight = newNode;
                }
                else if (direction.Equals(IdentityCell.bottomRightSide))
                {
                    Right = newNode;
                }
                else if (direction.Equals(IdentityCell.topLeftSide))
                {
                    Left = newNode;
                }
                else if (direction.Equals(IdentityCell.topSide))
                {
                    UpLeft = newNode;
                }
                else if (direction.Equals(IdentityCell.topRightSide))
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

        public HexNode FindCell(Cell node, int searchDepth = -1)
        {
            HashSet<HexNode> visited = new HashSet<HexNode>();
            return findCell(node, null, visited);
        }

        public List<Cell> PathTo(Cell targetCell)
        {
            Debug.WriteLine($"Path from {Cell} to {targetCell}");
            var result = new List<Cell>
            {
                Cell
            };

            if (Cell.Equals(targetCell))
            {
                return result;
            }
             
            var distance = Cell.DistanceTo(targetCell);
            var nextCell = Cell.CellLerp(targetCell, 1.0f / distance);
            Debug.WriteLine($"PathTo: distance {distance}, next: {nextCell}");
            var node = FindCell(nextCell, 1); // should check only children
            Debug.WriteLine($"find cell returns {node}");
            if (node == null) return null;

            var nextNodes = node.PathTo(targetCell);
            Debug.WriteLine($"next node returns {node}");
            if (nextNodes == null) return null;
            result.AddRange(nextNodes);
            return result;
        }

        private HexNode findCell(Cell node, IdentityCell fromSide, HashSet<HexNode> visited)
        {
            if (visited.Contains(this)) return null;

            if (Cell.Equals(node)) { return this; }

            visited.Add(this);

            HexNode found;
            if (Left != null && (fromSide == null || !fromSide.Equals(IdentityCell.topLeftSide)))
            {
                found = Left.findCell(node, IdentityCell.bottomRightSide, visited);
                if (found != null) return found;
            }
            if (UpLeft != null && (fromSide == null || !fromSide.Equals(IdentityCell.topSide)))
            {
                found = UpLeft.findCell(node, IdentityCell.bottomSide, visited);
                if (found != null) return found;
            }
            if (UpRight != null && (fromSide == null || !fromSide.Equals(IdentityCell.topRightSide)))
            {
                found = UpRight.findCell(node, IdentityCell.bottomLeftSide, visited);
                if (found != null) return found;
            }
            if (Right != null && (fromSide == null || !fromSide.Equals(IdentityCell.bottomRightSide)))
            {
                found = Right.findCell(node, IdentityCell.topLeftSide, visited);
                if (found != null) return found;
            }
            if (DownRight != null && (fromSide == null || !fromSide.Equals(IdentityCell.bottomSide)))
            {
                found = DownRight.findCell(node, IdentityCell.topSide, visited);
                if (found != null) return found;
            }
            if (DownLeft != null && (fromSide == null || !fromSide.Equals(IdentityCell.bottomLeftSide)))
            {
                found = DownLeft.findCell(node, IdentityCell.topRightSide, visited);
                if (found != null) return found;
            }
            return null;

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        internal HexNode GetSide(IdentityCell side)
        {
            if (side == IdentityCell.topLeftSide) return left;
            if (side == IdentityCell.topSide) return upLeft;
            if (side == IdentityCell.topRightSide) return upRight;
            if (side == IdentityCell.bottomRightSide) return right;
            if (side == IdentityCell.bottomSide) return downRight;
            if (side == IdentityCell.bottomLeftSide) return downLeft;
            Debug.Assert(false, "impossible side");
            return null;
        }

        public IEnumerator<HexNode> GetEnumerator()
        {
            return new HexNodeEnumerator(this);
        }

        public override string ToString()
        {
            return $"<{Cell.ToString()}>";
        }
    }

    class HexNodeEnumerator : IEnumerator<HexNode>
    {
        HexNode StartPoint;
        IdentityCell previous = IdentityCell.self;
        HashSet<HexNode> visited = new HashSet<HexNode>();
        Stack<HexNode> current = new Stack<HexNode>();
        public HexNodeEnumerator(HexNode startPoint)
        {
            StartPoint = startPoint;
            current = new Stack<HexNode>();
        }
        public HexNode Current => current.Peek();

        object IEnumerator.Current => current.Peek();

        public bool MoveNext()
        {
            const bool HAS_NEXT = true;

            // move to first
            if (current.Count == 0)
            {
                current.Push(StartPoint);
                visited.Add(StartPoint);
                return HAS_NEXT;
            }

            // if current node doesn't have any children, go to previous
            while (current.Count > 0) { 
                var moved = MoveToSide(IdentityCell.topLeftSide)
                || MoveToSide(IdentityCell.topSide)
                || MoveToSide(IdentityCell.topRightSide)
                || MoveToSide(IdentityCell.bottomRightSide)
                || MoveToSide(IdentityCell.bottomSide)
                || MoveToSide(IdentityCell.bottomLeftSide);
                if (moved) { return HAS_NEXT; }

                // if we can't move to a current node side, check previos until possible
                current.Pop();
            }

            return !HAS_NEXT; // TODO: pop & continue move

        }
        bool isSideDone(IdentityCell side)
        {
            return Current.GetSide(side) == null || visited.Contains(Current.GetSide(side));
        }

        private bool MoveToSide(IdentityCell side)
        {
            var sideNode = Current.GetSide(side);
            if (!isSideDone(side))
            {
                visited.Add(sideNode);
                current.Push(sideNode);
                return true;
            }
            return false;
        }

        public void Reset()
        {
            current = null;
            visited.Clear();
        }

        public void Dispose()
        {
            Reset();
        }
    }
}