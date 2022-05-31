using System.Collections.Generic;
using System;
namespace RailHexLib
{
    public class TradeRoute : IUpdatable
    {

        public TradeRoute(List<Cell> cells, Dictionary<Cell, Structure> tradePoints, Action tradePointReachedHandler)
        {
            Cells = cells;
            TradePoints = tradePoints;
            Direction = 1;
            CurrentPositionIndex = 0;
            OnTradePointHandler = tradePointReachedHandler;
        }

        public List<Cell> Cells;
        public Dictionary<Cell, Structure> TradePoints;

        public Cell CurrentTraderPosition => Cells[CurrentPositionIndex];
        public void Update(int ticks)
        {
            int maxIndex = Cells.Count - 1;
            for (int i = 0; i < ticks; i++)
            {
                CurrentPositionIndex += Config.Trader.moveTileTicks * Direction;
                // when greater we get reminder, change direction and substruct from current position. It's overrun
                if (CurrentPositionIndex >= maxIndex || CurrentPositionIndex < 0)
                {
                    Direction = Direction * -1;
                    CurrentPositionIndex -= (Math.Abs(CurrentPositionIndex) % maxIndex);
                    OnTradePointHandler();
                }

            }
        }
        int CurrentPositionIndex;
        int Direction;
        private Action OnTradePointHandler;
    }

}
