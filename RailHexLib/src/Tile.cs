
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
                [IdentityCell.leftSide] = Grounds.Ground.Grass,
                [IdentityCell.upLeftSide] = Grounds.Ground.Grass,
                [IdentityCell.upRightSide] = Grounds.Ground.Grass,
                [IdentityCell.rightSide] = Grounds.Ground.Grass,
                [IdentityCell.downRightSide] = Grounds.Ground.Grass,
                [IdentityCell.downLeftSide] = Grounds.Ground.Grass,
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
                [IdentityCell.leftSide] = Grounds.Ground.Water,
                [IdentityCell.upLeftSide] = Grounds.Ground.Water,
                [IdentityCell.upRightSide] = Grounds.Ground.Water,
                [IdentityCell.rightSide] = Grounds.Ground.Water,
                [IdentityCell.downRightSide] = Grounds.Ground.Water,
                [IdentityCell.downLeftSide] = Grounds.Ground.Water,
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
                [IdentityCell.leftSide] = Grounds.Ground.Road,
                [IdentityCell.upLeftSide] = Grounds.Ground.Road,
                [IdentityCell.upRightSide] = Grounds.Ground.Grass,
                [IdentityCell.rightSide] = Grounds.Ground.Grass,
                [IdentityCell.downRightSide] = Grounds.Ground.Grass,
                [IdentityCell.downLeftSide] = Grounds.Ground.Grass,
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
                [IdentityCell.leftSide] = Grounds.Ground.Road,
                [IdentityCell.upLeftSide] = Grounds.Ground.Grass,
                [IdentityCell.upRightSide] = Grounds.Ground.Road,
                [IdentityCell.rightSide] = Grounds.Ground.Grass,
                [IdentityCell.downRightSide] = Grounds.Ground.Grass,
                [IdentityCell.downLeftSide] = Grounds.Ground.Grass,
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
                [IdentityCell.leftSide] = Grounds.Ground.Road,
                [IdentityCell.upLeftSide] = Grounds.Ground.Grass,
                [IdentityCell.upRightSide] = Grounds.Ground.Grass,
                [IdentityCell.rightSide] = Grounds.Ground.Road,
                [IdentityCell.downRightSide] = Grounds.Ground.Grass,
                [IdentityCell.downLeftSide] = Grounds.Ground.Grass,
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
    //             [IdentityCell.rightSide] = Grounds.Ground.Grass,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Grass,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Grass,
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
    //             [IdentityCell.upRightSide] = Grounds.Ground.Grass,
    //             [IdentityCell.rightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Grass,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Grass
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
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Grass,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.rightSide] = Grounds.Ground.Grass,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Grass
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
    //             [IdentityCell.upLeftSide] = Grounds.Ground.Grass,
    //             [IdentityCell.upRightSide] = Grounds.Ground.Grass,
    //             [IdentityCell.rightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Grass,
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
    //             [IdentityCell.downRightSide] = Grounds.Ground.Grass,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Grass,
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
    //             [IdentityCell.rightSide] = Grounds.Ground.Grass,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Grass,
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
    //             [IdentityCell.upRightSide] = Grounds.Ground.Grass,
    //             [IdentityCell.rightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downRightSide] = Grounds.Ground.Road,
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Grass,
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
    //             [IdentityCell.downLeftSide] = Grounds.Ground.Grass,
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
}
