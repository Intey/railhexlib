using RailHexLib.Grounds;
using System.Collections.Generic;

namespace RailHexLib
{
    public class PlacementResult
    {
        public bool isPlaced = true;
        public Dictionary<Cell, Ground> NewJoins;
        public List<HexNode> NewOrphanRoads;
        public List<StructureRoad> NewStructureRoads;
        public List<TradeRoute> NewTradeRoutes;
        public bool GameOver = false;

        public PlacementResult(bool isPlaced, Dictionary<Cell, Ground> joins = null, bool gameover=false)
        {
            this.isPlaced = isPlaced;
            this.NewJoins = joins ?? new Dictionary<Cell, Ground>();
            this.GameOver = gameover;
        }

        public static implicit operator bool(PlacementResult value)
        {
            return value.isPlaced;
        }
        public static readonly PlacementResult Fail = new PlacementResult(false, null, false);
        public static readonly PlacementResult Success = new PlacementResult(false, null, false);
    }
}
