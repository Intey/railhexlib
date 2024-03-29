
using System;
using System.Collections.Generic;
using System.Linq;
namespace RailHexLib
{
    public abstract class Tile : Interfaces.IRotatable<Grounds.Ground>
    {
        internal Grounds.Ground GetSideBiome(IdentityCell side)
        {
            return sides[side];
        }

        internal bool HasBiome(Grounds.Ground expected) => sides.ContainsValue(expected);

        public abstract string tileName();
        public virtual void Tick(int ticks) { }
    }
    public class GrassTile : Tile
    {
        public override string tileName() => "Grass";
        public override string ToString()
        {
            return "Tile:Grass";
        }
        public GrassTile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.topLeftSide] = Grounds.Ground.Grass,
                [IdentityCell.topSide] = Grounds.Ground.Grass,
                [IdentityCell.topRightSide] = Grounds.Ground.Grass,
                [IdentityCell.bottomRightSide] = Grounds.Ground.Grass,
                [IdentityCell.bottomSide] = Grounds.Ground.Grass,
                [IdentityCell.bottomLeftSide] = Grounds.Ground.Grass,
            };
        }
    }

    public class WaterTile : Tile
    {
        public override string tileName() => "Water";

        public WaterTile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.topLeftSide] = Grounds.Ground.Water,
                [IdentityCell.topSide] = Grounds.Ground.Water,
                [IdentityCell.topRightSide] = Grounds.Ground.Water,
                [IdentityCell.bottomRightSide] = Grounds.Ground.Water,
                [IdentityCell.bottomSide] = Grounds.Ground.Water,
                [IdentityCell.bottomLeftSide] = Grounds.Ground.Water,
            };
        }
        public override string ToString()
        {
            return "Tile:Water";
        }
    }

    public class ROAD_60Tile : Tile
    {
        public override string tileName() => "ROAD_60";

        public ROAD_60Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.topLeftSide] = Grounds.Ground.Road,
                [IdentityCell.topSide] = Grounds.Ground.Road,
                [IdentityCell.topRightSide] = Grounds.Ground.Ground,
                [IdentityCell.bottomRightSide] = Grounds.Ground.Ground,
                [IdentityCell.bottomSide] = Grounds.Ground.Ground,
                [IdentityCell.bottomLeftSide] = Grounds.Ground.Ground,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(60)";
        }
    }

    public class ROAD_120Tile : Tile
    {
        public override string tileName() => "ROAD_120";

        public ROAD_120Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.topLeftSide] = Grounds.Ground.Road,
                [IdentityCell.topSide] = Grounds.Ground.Ground,
                [IdentityCell.topRightSide] = Grounds.Ground.Road,
                [IdentityCell.bottomRightSide] = Grounds.Ground.Ground,
                [IdentityCell.bottomSide] = Grounds.Ground.Ground,
                [IdentityCell.bottomLeftSide] = Grounds.Ground.Ground,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(120)";
        }
    }

    public class ROAD_180Tile : Tile
    {
        public override string tileName() => "ROAD_180";

        public ROAD_180Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.topLeftSide] = Grounds.Ground.Road,
                [IdentityCell.topSide] = Grounds.Ground.Ground,
                [IdentityCell.topRightSide] = Grounds.Ground.Ground,
                [IdentityCell.bottomRightSide] = Grounds.Ground.Road,
                [IdentityCell.bottomSide] = Grounds.Ground.Ground,
                [IdentityCell.bottomLeftSide] = Grounds.Ground.Ground,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(180)";
        }
    }

    // public class ROAD_3T_60_120Tile : Tile
    // {
    //     public override string tileName() => "ROAD_3T_60_120";

    //     public ROAD_3T_60_120Tile()
    //     {
    //         sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
    //         {
    //             [IdentityCell.leftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.rightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Ground,
    //         };
    //     }
    //     public override string ToString()
    //     {
    //         return "Tile:Road(60,120)";
    //     }
    // }

    // public class ROAD_3T_60_180Tile : Tile
    // {
    //     public override string tileName() => "ROAD_3T_60_180";

    //     public ROAD_3T_60_180Tile()
    //     {
    //         sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
    //         {
    //             [IdentityCell.leftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.rightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Ground
    //         };
    //     }
    //     public override string ToString()
    //     {
    //         return "Tile:Road(60,180)";
    //     }
    // }

    // public class ROAD_3T_120_240Tile : Tile
    // {
    //     public override string tileName() => "ROAD_3T_120_240";

    //     public ROAD_3T_120_240Tile()
    //     {
    //         sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
    //         {
    //             [IdentityCell.leftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Ground,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.rightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Ground
    //         };
    //     }
    //     public override string ToString()
    //     {
    //         return "Tile:Road(120,240)";
    //     }
    // }

    // public class ROAD_3T_180_300Tile : Tile
    // {
    //     public override string tileName() => "ROAD_3T_180_300";

    //     public ROAD_3T_180_300Tile()
    //     {
    //         sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
    //         {
    //             [IdentityCell.leftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Ground,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.rightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Road,
    //         };
    //     }
    //     public override string ToString()
    //     {
    //         return "Tile:Road(180,300)";
    //     }
    // }

    // public class ROAD_4T_60_120_180Tile : Tile
    // {
    //     public override string tileName() => "ROAD_4T_60_120_180";

    //     public ROAD_4T_60_120_180Tile()
    //     {
    //         sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
    //         {
    //             [IdentityCell.leftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.rightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Ground,
    //         };
    //     }
    //     public override string ToString()
    //     {
    //         return "Tile:Road(60,120,180)";
    //     }
    // }

    // public class ROAD_4T_60_120_240Tile : Tile
    // {
    //     public override string tileName() => "ROAD_4T_60_120_240";

    //     public ROAD_4T_60_120_240Tile()
    //     {
    //         sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
    //         {
    //             [IdentityCell.leftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.rightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Ground,
    //         };
    //     }
    //     public override string ToString()
    //     {
    //         return "Tile:Road(60,120,240)";
    //     }
    // }

    // public class ROAD_4T_60_180_240Tile : Tile
    // {
    //     public override string tileName() => "ROAD_4T_60_180_240";

    //     public ROAD_4T_60_180_240Tile()
    //     {
    //         sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
    //         {
    //             [IdentityCell.leftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Ground,
    //             [IdentityCell.rightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Ground,
    //         };
    //     }
    //     public override string ToString()
    //     {
    //         return "Tile:Road(60,180,240)";
    //     }
    // }

    // public class ROAD_5T_60_120_180_240Tile : Tile
    // {
    //     public override string tileName() => "ROAD_5T_60_120_180_240";

    //     public ROAD_5T_60_120_180_240Tile()
    //     {
    //         sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
    //         {
    //             [IdentityCell.leftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.rightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Ground,
    //         };
    //     }
    //     public override string ToString()
    //     {
    //         return "Tile:Road(60,120,180,240)";
    //     }
    // }

    // public class ROAD_6TTile : Tile
    // {
    //     public override string tileName() => "ROAD_6T";
    //     public ROAD_6TTile()
    //     {

    //         sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
    //         {
    //             [IdentityCell.leftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Road,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.rightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Road,
    //         };
    //     }
    //     public override string ToString()
    //     {
    //         return "Tile:Road(All)";
    //     }
    // }

    public class ForestTile : Tile
    {
        public override string tileName() => "Forest";
        public ForestTile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.topLeftSide] = Grounds.Ground.Forest,
                [IdentityCell.topSide] = Grounds.Ground.Forest,
                [IdentityCell.topRightSide] = Grounds.Ground.Forest,
                [IdentityCell.bottomRightSide] = Grounds.Ground.Forest,
                [IdentityCell.bottomSide] = Grounds.Ground.Forest,
                [IdentityCell.bottomLeftSide] = Grounds.Ground.Forest,
            };
        }
        public override string ToString() => tileName();
    }
}
