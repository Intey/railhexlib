using RailHexLib.Grounds;
using System.Collections.Generic;

namespace RailHexLib
{
    public class PlacementResult
    {
        // TODO: prevent changes in this fields with factory
        public bool isPlaced = true;
        public Dictionary<Cell, Ground> NewJoins;
        public List<HexNode> NewOrphanRoads;
        public List<StructureRoad> NewStructureRoads;
        public List<Trader> NewTradeRoutes;
        public bool GameOver = false;
        public Tile PlacedTile = null;

        public PlacementResult(Tile placedTile, Dictionary<Cell, Ground> joins = null, bool gameover=false)
        {
            this.PlacedTile = placedTile;
            this.isPlaced = placedTile != null;
            this.NewJoins = joins ?? new Dictionary<Cell, Ground>();
            this.GameOver = gameover;
            this.NewOrphanRoads = new List<HexNode>();
            this.NewStructureRoads = new List<StructureRoad>();
            this.NewTradeRoutes = new List<Trader>();
        }

        public static implicit operator bool(PlacementResult value)
        {
            return value.isPlaced;
        }
        public static readonly PlacementResult Fail = new PlacementResult(null, null, false);
    }
}
