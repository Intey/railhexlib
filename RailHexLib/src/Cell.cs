﻿using System;
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
    public class Cell : IEquatable<Cell>
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

        public static Cell Rounded(float r, float q)
        {
            var qn = Math.Round(q);
            var rn = Math.Round(r);
            var sn = Math.Round(-q - r);

            var q_diff = Math.Abs(qn - q);
            var r_diff = Math.Abs(rn - r);
            var s_diff = Math.Abs(sn - q - r);


            if (q_diff > r_diff && q_diff > s_diff)
            {
                qn = -rn - sn;

            }
            else if (r_diff > s_diff)
            {
                rn = -qn - sn;

            }
            return new Cell((int)Math.Ceiling(rn), (int)Math.Ceiling(qn));
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
                new Cell(r, q - 1, size),
                new Cell(r - 1, q, size),
                new Cell(r - 1, q + 1, size),
                new Cell(r, q + 1, size),
                new Cell(r + 1, q, size),
                new Cell(r + 1, q - 1, size),
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

        public static Cell operator +(Cell l, Cell r)
        {
            Debug.Assert(l.size == r.size);
            return new Cell(l.R + r.R, l.Q + r.Q, l.size);
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
        public static readonly IdentityCell upLeftSide = new IdentityCell(1, -1);
        public static readonly IdentityCell upRightSide = new IdentityCell(1, 0);
        public static readonly IdentityCell rightSide = new IdentityCell(0, 1);
        public static readonly IdentityCell downSide = new IdentityCell(-1, 1);
        public static readonly IdentityCell downRightSide = new IdentityCell(-1, 1);
        public static readonly IdentityCell downLeftSide = new IdentityCell(-1, 0);
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
