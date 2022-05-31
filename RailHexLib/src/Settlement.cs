using System;
using System.Collections.Generic;

namespace RailHexLib
{
    public class Settlement : Structure
    {

        public Settlement(Cell center, string name="GIVEMEADEFAULTNAME") : base(center, name)
        {
            sides = new Dictionary<IdentityCell, Tile>(new IdentityCellEqualityComparer());
            foreach (var neighbor in center.Neighbours())
            {
                sides[new IdentityCell(neighbor - center)] = new GrassTile();
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
            foreach(var side in sides)
            {
                side.Value.Rotate60Clock();
            }
        }

        public override string TileName()
        {
            return "Settlement";
        }

        public override Cell GetEnterCell()
        {
            return center + incomeRoadCell;
        }
        private IdentityCell incomeRoadCell;

        public override Dictionary<Cell, Tile> GetHexes()
        {
            var result = base.GetHexes();
            foreach(var side in sides)
            {
                result[center + side.Key] = side.Value;
            }
            return result;
        }
        public override string ToString()
        {
            return $"{name} Settlement";
        }
    }

}
