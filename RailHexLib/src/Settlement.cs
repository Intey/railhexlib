using System;
using System.Collections.Generic;
using RailHexLib.Grounds;

namespace RailHexLib
{
    public class Settlement : Structure
    {

        public Settlement(Cell center,
                          string name = "GIVEMEADEFAULTNAME",
                          List<Dictionary<Resource, int>> needs = null,
                          Dictionary<Resource, int> resources = null
        ) 
        : base(center, name, needs, resources)
        {
            sides = new Dictionary<IdentityCell, Tile>(new IdentityCellEqualityComparer());
            foreach (var (side, _) in center.Neighbours())
            {
                sides[side] = new GrassTile();
            }
            incomeRoadCell = IdentityCell.topLeftSide;

            sides[incomeRoadCell] = new ROAD_180Tile();

            if (needs == null) 
            {
                
            }

        }

        public override void Rotate60Clock()
        {
            base.Rotate60Clock();

            if (incomeRoadCell.Equals(IdentityCell.topLeftSide))
            {
                incomeRoadCell = IdentityCell.topSide;
            }
            else if (incomeRoadCell.Equals(IdentityCell.topSide))
            {
                incomeRoadCell = IdentityCell.topRightSide;
            }
            else if (incomeRoadCell.Equals(IdentityCell.topRightSide))
            {
                incomeRoadCell = IdentityCell.bottomRightSide;
            }
            else if (incomeRoadCell.Equals(IdentityCell.bottomRightSide))
            {
                incomeRoadCell = IdentityCell.bottomSide;
            }
            else if (incomeRoadCell.Equals(IdentityCell.bottomSide))
            {
                incomeRoadCell = IdentityCell.bottomLeftSide;
            }
            else
            {
                incomeRoadCell = IdentityCell.topLeftSide;
            }
            foreach (var side in sides)
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
            foreach (var side in sides)
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
