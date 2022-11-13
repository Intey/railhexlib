using RailHexLib.Grounds;
using System.Collections.Generic;

namespace RailHexLib
{
    public class PlacementResult
    {
        // TODO: prevent changes in this fields with factory
        public bool isPlaced = true;
        public Dictionary<Cell, Ground> NewJoins = new Dictionary<Cell, Ground>();
        public List<HexNode> NewOrphanRoads = new List<HexNode>();
        public List<StructureRoad> NewStructureRoads = new List<StructureRoad>();
        public List<Trader> NewTraders = new List<Trader>();

        public List<Zone> NewZones = new List<Zone>();
        public bool GameOver = false;
        public Tile PlacedTile = null;

        public PlacementResult(Tile placedTile, Dictionary<Cell, Ground> joins = null, bool gameover=false)
        {
            this.PlacedTile = placedTile;
            this.isPlaced = placedTile != null;
            this.NewJoins = joins ?? new Dictionary<Cell, Ground>();
            this.GameOver = gameover;
        }

        public static implicit operator bool(PlacementResult value)
        {
            return value.isPlaced;
        }
        public static readonly PlacementResult Fail = new PlacementResult(null, null, false);
    }
}
