
using System;
using System.Collections.Generic;
namespace RailHexLib
{
    public abstract class Tile : Interfaces.IRotatable<Grounds.Ground>
    {
        internal Grounds.Ground GetSideBiome(IdentityCell side)
        {
            return sides[side];
        }

        public abstract string tileName();
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
                [IdentityCell.leftSide] = Grounds.Grass.instance,
                [IdentityCell.upLeftSide] = Grounds.Grass.instance,
                [IdentityCell.upRightSide] = Grounds.Grass.instance,
                [IdentityCell.rightSide] = Grounds.Grass.instance,
                [IdentityCell.downRightSide] = Grounds.Grass.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance,
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
                [IdentityCell.leftSide] = Grounds.Water.instance,
                [IdentityCell.upLeftSide] = Grounds.Water.instance,
                [IdentityCell.upRightSide] = Grounds.Water.instance,
                [IdentityCell.rightSide] = Grounds.Water.instance,
                [IdentityCell.downRightSide] = Grounds.Water.instance,
                [IdentityCell.downLeftSide] = Grounds.Water.instance,
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
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Road.instance,
                [IdentityCell.upRightSide] = Grounds.Grass.instance,
                [IdentityCell.rightSide] = Grounds.Grass.instance,
                [IdentityCell.downRightSide] = Grounds.Grass.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance,
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
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Grass.instance,
                [IdentityCell.upRightSide] = Grounds.Road.instance,
                [IdentityCell.rightSide] = Grounds.Grass.instance,
                [IdentityCell.downRightSide] = Grounds.Grass.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance,
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
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Grass.instance,
                [IdentityCell.upRightSide] = Grounds.Grass.instance,
                [IdentityCell.rightSide] = Grounds.Road.instance,
                [IdentityCell.downRightSide] = Grounds.Grass.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(180)";
        }
    }

    public class ROAD_3T_60_120Tile : Tile
    {
        public override string tileName() => "ROAD_3T_60_120";

        public ROAD_3T_60_120Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Road.instance,
                [IdentityCell.upRightSide] = Grounds.Road.instance,
                [IdentityCell.rightSide] = Grounds.Grass.instance,
                [IdentityCell.downRightSide] = Grounds.Grass.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(60,120)";
        }
    }

    public class ROAD_3T_60_180Tile : Tile
    {
        public override string tileName() => "ROAD_3T_60_180";

        public ROAD_3T_60_180Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Road.instance,
                [IdentityCell.upRightSide] = Grounds.Grass.instance,
                [IdentityCell.rightSide] = Grounds.Road.instance,
                [IdentityCell.downRightSide] = Grounds.Grass.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance
            };
        }
        public override string ToString()
        {
            return "Tile:Road(60,180)";
        }
    }

    public class ROAD_3T_120_240Tile : Tile
    {
        public override string tileName() => "ROAD_3T_120_240";

        public ROAD_3T_120_240Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Grass.instance,
                [IdentityCell.upRightSide] = Grounds.Road.instance,
                [IdentityCell.rightSide] = Grounds.Grass.instance,
                [IdentityCell.downRightSide] = Grounds.Road.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance
            };
        }
        public override string ToString()
        {
            return "Tile:Road(120,240)";
        }
    }

    public class ROAD_3T_180_300Tile : Tile
    {
        public override string tileName() => "ROAD_3T_180_300";

        public ROAD_3T_180_300Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Grass.instance,
                [IdentityCell.upRightSide] = Grounds.Grass.instance,
                [IdentityCell.rightSide] = Grounds.Road.instance,
                [IdentityCell.downRightSide] = Grounds.Grass.instance,
                [IdentityCell.downLeftSide] = Grounds.Road.instance,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(180,300)";
        }
    }

    public class ROAD_4T_60_120_180Tile : Tile
    {
        public override string tileName() => "ROAD_4T_60_120_180";

        public ROAD_4T_60_120_180Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Road.instance,
                [IdentityCell.upRightSide] = Grounds.Road.instance,
                [IdentityCell.rightSide] = Grounds.Road.instance,
                [IdentityCell.downRightSide] = Grounds.Grass.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(60,120,180)";
        }
    }

    public class ROAD_4T_60_120_240Tile : Tile
    {
        public override string tileName() => "ROAD_4T_60_120_240";

        public ROAD_4T_60_120_240Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Road.instance,
                [IdentityCell.upRightSide] = Grounds.Road.instance,
                [IdentityCell.rightSide] = Grounds.Grass.instance,
                [IdentityCell.downRightSide] = Grounds.Road.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(60,120,240)";
        }
    }

    public class ROAD_4T_60_180_240Tile : Tile
    {
        public override string tileName() => "ROAD_4T_60_180_240";

        public ROAD_4T_60_180_240Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Road.instance,
                [IdentityCell.upRightSide] = Grounds.Grass.instance,
                [IdentityCell.rightSide] = Grounds.Road.instance,
                [IdentityCell.downRightSide] = Grounds.Road.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(60,180,240)";
        }
    }

    public class ROAD_5T_60_120_180_240Tile : Tile
    {
        public override string tileName() => "ROAD_5T_60_120_180_240";

        public ROAD_5T_60_120_180_240Tile()
        {
            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Road.instance,
                [IdentityCell.upRightSide] = Grounds.Road.instance,
                [IdentityCell.rightSide] = Grounds.Road.instance,
                [IdentityCell.downRightSide] = Grounds.Road.instance,
                [IdentityCell.downLeftSide] = Grounds.Grass.instance,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(60,120,180,240)";
        }
    }

    public class ROAD_6TTile : Tile
    {
        public override string tileName() => "ROAD_6T";
        public ROAD_6TTile()
        {

            sides = new Dictionary<IdentityCell, Grounds.Ground>(new IdentityCellEqualityComparer())
            {
                [IdentityCell.leftSide] = Grounds.Road.instance,
                [IdentityCell.upLeftSide] = Grounds.Road.instance,
                [IdentityCell.upRightSide] = Grounds.Road.instance,
                [IdentityCell.rightSide] = Grounds.Road.instance,
                [IdentityCell.downRightSide] = Grounds.Road.instance,
                [IdentityCell.downLeftSide] = Grounds.Road.instance,
            };
        }
        public override string ToString()
        {
            return "Tile:Road(All)";
        }
    }
}