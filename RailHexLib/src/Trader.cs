using System.Collections.Generic;
using System;
namespace RailHexLib
{
    public class Trader : IUpdatable
    {
        // use list for possible batching/optimization
        public class PointReachedArgs: System.EventArgs {
            public PointReachedArgs(List<Structure> structures) {
                ReachedStructures = structures;
            }

            public readonly List<Structure> ReachedStructures;
        }

        public event EventHandler<Trader.PointReachedArgs> OnTraderArrivesToAStructure;

        public Trader(List<Cell> cells, Dictionary<Cell, Structure> tradePoints)
        {
            Cells = cells;
            TradePoints = tradePoints;
            Direction = 1;
            CurrentPositionIndex = 0;
        }

        public List<Cell> Cells;
        public Dictionary<Cell, Structure> TradePoints;

        public Cell CurrentPosition => Cells[CurrentPositionIndex];
        public void Tick(int ticks)
        {
            
            // calc that trader go over the settlement
            int maxIndex = Cells.Count - 1;
            for (int i = 0; i < ticks; i++)
            {
                CurrentPositionIndex += Direction;  

                // Switch direction
                var onTheEdge = CurrentPositionIndex == maxIndex || CurrentPositionIndex == 0;
                if (onTheEdge)
                {
                    Direction = Direction * -1;
                }

                if (TradePoints.ContainsKey(CurrentPosition))
                {
                    var ReachedStructures = new List<Structure>(){TradePoints[CurrentPosition]};
                    TradePoints[CurrentPosition].VisitTrader(this);
                    var tmp_event = OnTraderArrivesToAStructure;
                    if (tmp_event != null)
                    {
                        tmp_event(this, new Trader.PointReachedArgs(ReachedStructures));
                    }
                }
            }
        }


        int CurrentPositionIndex;
        int Direction; // positive - forward, negative - backward

    }

}
