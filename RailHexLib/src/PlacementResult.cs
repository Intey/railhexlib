using RailHexLib.Grounds;
using System.Collections.Generic;

namespace RailHexLib
{
    public class PlacementResult
    {
        public readonly bool isPlaced = true;
        public readonly Dictionary<Cell, Ground> NewJoins;
        public PlacementResult(bool isPlaced, Dictionary<Cell, Ground> joins = null)
        {
            this.isPlaced = isPlaced;
            this.NewJoins = joins ?? new Dictionary<Cell, Ground>();
        }

        public static implicit operator bool(PlacementResult value)
        {
            return value.isPlaced;
        }
        public static readonly PlacementResult Fail = new PlacementResult(false);
        public static readonly PlacementResult Success = new PlacementResult(false);
    }
}