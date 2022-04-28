using System;
using System.Collections.Generic;

namespace RailHexLib
{
    public class Settlement : Structure
    {

        public Settlement(Cell center) : base(center)
        {
            sides = new Dictionary<IdentityCell, Tile>(new IdentityCellEqualityComparer());
            foreach (var neighbor in center.Neighbours())
            {
                sides[new IdentityCell(neighbor -center)] = new GrassTile();
            }
            incomeRoadCell = IdentityCell.leftSide;

            sides[incomeRoadCell] = new ROAD_180Tile();
        }

        public override void Rotate60Clock()
        {
            base.Rotate60Clock();
            if (incomeRoadCell.Equals(IdentityCell.leftSide))
            {
                incomeRoadCell = IdentityCell.upLeftSide;
            }
            else if (incomeRoadCell.Equals(IdentityCell.upLeftSide))
            {
                incomeRoadCell = IdentityCell.upRightSide;
            }
            else if (incomeRoadCell.Equals(IdentityCell.upRightSide))
            {
                incomeRoadCell = IdentityCell.rightSide;
            }
            else if (incomeRoadCell.Equals(IdentityCell.rightSide))
            {
                incomeRoadCell = IdentityCell.downRightSide;
            }
            else if (incomeRoadCell.Equals(IdentityCell.downRightSide))
            {
                incomeRoadCell = IdentityCell.downLeftSide;
            }
            else
            {
                incomeRoadCell = IdentityCell.leftSide;
            }

        }

        public override string TileName()
        {
            return "Settlement";
        }

        public override Cell GetIcomeRoadCell()
        {
            return center + incomeRoadCell;
        }
        private IdentityCell incomeRoadCell;

    }

}
