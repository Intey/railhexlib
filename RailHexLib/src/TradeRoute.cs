using System.Collections.Generic;

namespace RailHexLib
{
    public class TradeRoute
    {
        public List<Cell> cells = new List<Cell>();
        public Dictionary<Cell, Structure> tradePoints = new Dictionary<Cell, Structure>();
    }

}
