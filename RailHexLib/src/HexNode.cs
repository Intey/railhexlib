// using RailHexLib.Grounds;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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

        public HexNode TopLeft
        {
            get => left;
            set
            {
                left = value;
                value.right = this;
            }
        }
        public HexNode Top
        {
            get => upLeft;
            set
            {
                upLeft = value;
                value.downRight = this;
            }
        }
        public HexNode TopRight
        {
            get => upRight;
            set
            {
                upRight = value;
                value.downLeft = this;
            }
        }
        public HexNode BottomRight
        {
            get => right;
            set
            {
                right = value;
                value.left = this;
            }
        }
        public HexNode Bottom
        {
            get => downRight;
            set
            {
                downRight = value;
                value.upLeft = this;
            }
        }
        public HexNode BottomLeft
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
                    BottomLeft = newNode;
                }
                else if (direction.Equals(IdentityCell.bottomSide))
                {
                    Bottom = newNode;
                }
                else if (direction.Equals(IdentityCell.bottomRightSide))
                {
                    BottomRight = newNode;
                }
                else if (direction.Equals(IdentityCell.topLeftSide))
                {
                    TopLeft = newNode;
                }
                else if (direction.Equals(IdentityCell.topSide))
                {
                    Top = newNode;
                }
                else if (direction.Equals(IdentityCell.topRightSide))
                {
                    TopRight = newNode;
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
            var result = new List<Cell>();
            var frontier = new PriorityQueue<HexNode, int>();
            frontier.Enqueue(this, 1);
            var previousCellFor = new Dictionary<Cell, Cell>();
            previousCellFor[this.Cell] = null;
            var costSoFar = new Dictionary<HexNode, int>();
            costSoFar[this] = 0;

            while(frontier.Count != 0) {
                var current = frontier.Dequeue();
                Debug.WriteLine($"Procell frontier: {current}");
                if (current.Cell == targetCell){
                    break;
                }
                foreach (var neighbor in current.Neighbors()) {
                    var newCost = costSoFar[current] + 1; // TODO: add moveCost to cells(Tiles?)

                    Debug.WriteLine($"Check frontier neighbor: {neighbor}");
                    if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) {
                        costSoFar[neighbor] = newCost;
                        var priority = newCost + neighbor.Cell.DistanceTo(targetCell);
                        frontier.Enqueue(neighbor, priority);
                        previousCellFor[neighbor.Cell] = current.Cell;
                    }
                }
            }
            if(!previousCellFor.ContainsKey(targetCell)) return result;
            
            var curr = previousCellFor[targetCell];
            while(curr != null) {
                result.Add(curr);
                curr = previousCellFor[curr];
            }
            result.Reverse();
            result.Add(targetCell);
            return result;
        }
        private List<HexNode> Neighbors() 
        {

            var result =  new List<HexNode>();
            if (TopLeft != null) result.Add(TopLeft);
            if (Top != null) result.Add(Top);
            if (TopRight != null) result.Add(TopRight);
            
            if (BottomRight != null) result.Add(BottomRight);
            if (Bottom != null) result.Add(Bottom);
            if (BottomLeft != null) result.Add(BottomLeft);
            return result;
        }

        private HexNode findCell(Cell node, IdentityCell fromSide, HashSet<HexNode> visited)
        {
            if (visited.Contains(this)) return null;

            if (Cell.Equals(node)) { return this; }

            visited.Add(this);

            HexNode found;
            if (TopLeft != null && (fromSide == null || !fromSide.Equals(IdentityCell.topLeftSide)))
            {
                found = TopLeft.findCell(node, IdentityCell.bottomRightSide, visited);
                if (found != null) return found;
            }
            if (Top != null && (fromSide == null || !fromSide.Equals(IdentityCell.topSide)))
            {
                found = Top.findCell(node, IdentityCell.bottomSide, visited);
                if (found != null) return found;
            }
            if (TopRight != null && (fromSide == null || !fromSide.Equals(IdentityCell.topRightSide)))
            {
                found = TopRight.findCell(node, IdentityCell.bottomLeftSide, visited);
                if (found != null) return found;
            }
            if (BottomRight != null && (fromSide == null || !fromSide.Equals(IdentityCell.bottomRightSide)))
            {
                found = BottomRight.findCell(node, IdentityCell.topLeftSide, visited);
                if (found != null) return found;
            }
            if (Bottom != null && (fromSide == null || !fromSide.Equals(IdentityCell.bottomSide)))
            {
                found = Bottom.findCell(node, IdentityCell.topSide, visited);
                if (found != null) return found;
            }
            if (BottomLeft != null && (fromSide == null || !fromSide.Equals(IdentityCell.bottomLeftSide)))
            {
                found = BottomLeft.findCell(node, IdentityCell.topRightSide, visited);
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