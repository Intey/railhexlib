using System.Collections.Generic;
using System;
namespace RailHexLib
{
    public class Trader : IUpdatable
    {
        // use list for possible batching/optimization
        public class PointReachedArgs : System.EventArgs
        {
            public PointReachedArgs(Structure structure)
            {
                ReachedStructure = structure;
            }

            public readonly Structure ReachedStructure;
        }

        public event EventHandler<Trader.PointReachedArgs> OnTraderArrivesToAStructure;

        public Trader(List<Cell> cells, Dictionary<Cell, Structure> tradePoints)
        {
            Cells = cells;
            TradePoints = tradePoints;
        }

        public List<Cell> Cells;
        public Dictionary<Cell, Structure> TradePoints;
        int CurrentPositionIndex = 0;
        int Direction = 1; // positive - forward, negative - backward
        Dictionary<Resource, int> inventory = new Dictionary<Resource, int>();

        public Dictionary<Resource, int> Inventory { get => inventory; }
        public Cell CurrentPosition => Cells[CurrentPositionIndex];
        public void Tick(int ticks = 1)
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
                    var reachedStructure = TradePoints[CurrentPosition];
                    // pick resources to fit inventory
                    foreach (var (resType, count) in reachedStructure.Resources)
                    {
                        if (!inventory.ContainsKey(resType))
                        {
                            inventory[resType] = 0;
                        }
                        // we will pick lowest value, some percent or rest count to fill inventory 
                        var toPick = Math.Min(
                            (int)(count * Config.Trader.consumptionPercent),
                            Config.Trader.maxResourceCountInInventory - inventory[resType]
                        );
                        reachedStructure.PickResources(resType, toPick);
                        inventory[resType] += toPick;
                    }

                    reachedStructure.VisitTrader(this);
                    var tmp_event = OnTraderArrivesToAStructure;
                    if (tmp_event != null)
                    {
                        tmp_event(this, new Trader.PointReachedArgs(reachedStructure));
                    }
                }
            }
        }

    }

}
