using System;
using System.Diagnostics;
using System.Collections.Generic;

/**
 * Grids
 * 
 * ^ R Axis    \ Q Axis (Diagonal left-up to right-down)
 * |            v
 * ======================================================
 *  /R\
 * | Q |
 *  \S/
 * ======================================================
 *    /1\ /1\
 *   | - | 0 |
 *  /0\ /0\ /0\
 * | - | 0 | 1 |
 *  \ /-\ /-\ /
 *   | 0 | 1 |
 *    \ / \ /
 *    
 */

namespace RailHexLib
{
    // [System.Serializable]
    public class Cell : IEquatable<Cell>, IDistancable<Cell>
    {
        // [SerializeField]
        private int r, q;

        public float size;

        public int R => r;

        public int Q => q;

        public int S => -R - Q;

        public Cell(int r, int q, float size = 1.15f)
        {
            this.r = r;
            this.q = q;
            this.size = size;
        }

        public static Cell Rounded(float r, float q, float size = 1.15f)
        {
            var qn = Math.Round(q + .0001);
            var rn = Math.Round(r);
            var sn = Math.Round(-q - r);

            // Guarantee rule R+Q+S == 0
            var q_diff = Math.Abs(qn - q);
            var r_diff = Math.Abs(rn - r);
            var s_diff = Math.Abs(sn - q - r);

            // Q is biggest coord value - reset
            if (q_diff > r_diff)
            {
                qn = -rn - sn;

            }
            else
            {
                rn = -qn - sn;
            }

            return new Cell((int)Math.Ceiling(rn), (int)Math.Ceiling(qn), size);
        }

        public static Cell operator -(Cell l, Cell r)
        {
            Debug.Assert(l.size == r.size);
            return new Cell(l.R - r.R, l.Q - r.Q, r.size);
        }


        public override string ToString()
        {
            return $"(R:{R}, Q:{Q})";
        }
        public List<Cell> Neighbours()
        {
            return new List<Cell>
            {
                (this + IdentityCell.leftSide),
                (this + IdentityCell.upLeftSide),
                (this + IdentityCell.upRightSide),
                (this + IdentityCell.rightSide),
                (this + IdentityCell.downRightSide),
                (this + IdentityCell.downLeftSide)
            };
        }



        public override int GetHashCode() // required for use as a key in Dictionary
        {
            int hashCode = -630286571;
            hashCode = hashCode * -1521134295 + size.GetHashCode();
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            hashCode = hashCode * -1521134295 + Q.GetHashCode();
            return hashCode;
        }

        public bool Equals(Cell cell)
        {
            return cell != null &&
                   size == cell.size &&
                   R == cell.R &&
                   Q == cell.Q;
        }


        public int DistanceTo(Cell other)
        {
            return (Math.Abs(R - other.R) + Math.Abs(Q - other.Q) + Math.Abs(S - other.S)) / 2;
        }


        public static Cell operator +(Cell l, Cell r)
        {
            Debug.Assert(l.size == r.size);
            return new Cell(l.R + r.R, l.Q + r.Q, l.size);
        }


        public IdentityCell GetDirectionTo(Cell c)
        {
            Debug.Assert(DistanceTo(c) == 1, "Can get direction only for neighbor currently");
            var d = c - this;
            return new IdentityCell(d);
        }

        /// <summary/>
        /// <param name="targetCell">target cell</param>
        /// <returns>path from current cell to target, including current and target cells</returns>
        public List<Cell> PathTo(Cell targetCell)
        {
            var result = new List<Cell>();
            var distance = DistanceTo(targetCell);
            for (int i = 0; i <= distance; i++)
            {
                result.Add(CellLerp(targetCell, 1.0f / distance * i));
            }
            return result;
        }

        private float Lerp(int a, int b, float t)
        {
            return a + (b - a) * t;
        }
        public Cell CellLerp(Cell b, float t)
        {
            Debug.Assert(this.size == b.size);

            float r = Lerp(R, b.R, t);
            float q = Lerp(Q, b.Q, t);
            return Rounded(r, q);
        }


    }
    // new Dictionary<Cell, int>(new CellEqualityComparer());
    internal class CellEqualityComparer : IEqualityComparer<Cell>
    {
        public bool Equals(Cell x, Cell y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Cell obj)
        {
            return obj.GetHashCode();
        }
    }

    public class IdentityCell : Cell
    {
        public IdentityCell(int R, int Q, float size = 1.15f)
            : base(Math.Sign(R), Math.Sign(Q), size)
        {
        }
        public IdentityCell(Cell source) : this(source.R, source.Q, source.size)
        {
        }
        public IdentityCell Inverted()
        {
            int newR = 0;
            int newQ = 0;
            if (S == 0)
            {
                newR = Q;
                newQ = R;
            }
            else if (Q == 0)
            {
                newR = S;
                newQ = 0;
            }
            else
            {
                newR = 0;
                newQ = S;
            }
            return new IdentityCell(newR, newQ, size);
        }


        public override int GetHashCode()
        {
            int hashCode = -243245557;
            hashCode = hashCode * -1521134295 + size.GetHashCode();
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            hashCode = hashCode * -1521134295 + Q.GetHashCode();
            return hashCode;
        }

        public bool Equals(IdentityCell cell)
        {
            return cell != null
                && R == cell.R
                && Q == cell.Q;
        }

        public override bool Equals(object obj)
        {
            return obj is IdentityCell && this.Equals((IdentityCell)obj);
        }
        
        public static readonly IdentityCell leftSide = new IdentityCell(0, -1);
        public static readonly IdentityCell upLeftSide = new IdentityCell(-1, 0);
        public static readonly IdentityCell upRightSide = new IdentityCell(-1, 1);
        public static readonly IdentityCell rightSide = new IdentityCell(0, 1);
        public static readonly IdentityCell downRightSide = new IdentityCell(1, 0);
        public static readonly IdentityCell downLeftSide = new IdentityCell(1, -1);
        internal static IdentityCell self = new IdentityCell(0, 0);
    }
    internal class IdentityCellEqualityComparer : IEqualityComparer<IdentityCell>
    {
        public bool Equals(IdentityCell x, IdentityCell y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(IdentityCell obj)
        {
            return obj.GetHashCode();
        }
    }


}
