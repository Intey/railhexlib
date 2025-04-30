using System.Collections.Generic;
using System;
namespace RailHexLib
{
    public class Trader
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

        public Trader()
        {

        }

        public Trader(List<Cell> route, Dictionary<Cell, Structure> tradePoints)
        {
            routePath = route;
            foreach (var kv in tradePoints)
            {
                TradePoints[kv.Key] = (kv.Value, new Dictionary<Resource,int>());
            }
        }
        /// <summary>
        /// Send trader with given orders and route
        /// Route is a List of tradepoints, where each trade point is a structure, orders to exchange in the structure and a path to next structure
        /// </summary>
        /// <param name="route"></param>
        public void Send(Dictionary<Structure, (Dictionary<Resource, int>, List<Cell>)> route)
        {
            foreach (var (structure, (orders, path)) in route)
            {
                TradePoints[structure.GetEnterCell()] = (structure, orders);
                routePath.AddRange(path);
            }
        }

        public List<Cell> routePath = new();
        public Dictionary<Cell, (Structure, Dictionary<Resource, int>)> TradePoints = new();
        int CurrentPositionIndex = 0;
        int Direction = 1; // positive - forward, negative - backward
        private Inventory inventory = new();

        public Inventory Inventory { get => inventory; }
        public Cell CurrentPosition => routePath[CurrentPositionIndex];
        public void Tick(int ticks = 1)
        {

            // calc that trader go over the settlement
            int maxIndex = routePath.Count - 1;
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
                    var (reachedStructure, orders) = TradePoints[CurrentPosition];
                    // pick resources to fit inventory
                    visitStructure(reachedStructure, orders);
                }
            }
        }

        void visitStructure(Structure reachedStructure, Dictionary<Resource, int> orders)
        {
            // exchange
            foreach (var (resType, count) in orders)
            {
                var existResource = inventory.ResourceCount(resType);
                // should pick
                if (count > 0)
                {
                    inventory.AddResource(resType, reachedStructure.PickResource(resType, count));
                }
                else if (count < 0)
                {
                    reachedStructure.Inventory.AddResource(resType, inventory.PickResource(resType, -count));
                }
                
            }

            foreach (var resource in Inventory.Resources.Keys)
            {
                var picked = Inventory.PickResource(
                                                 resource,
                                                Config.Trader.maxResourceCountInInventory);
                reachedStructure.Inventory.AddResource(resource, picked);
            }
            reachedStructure.VisitTrader(this);

            // var tmp_event = OnTraderArrivesToAStructure;
            // if (tmp_event != null)
            // {
            //     tmp_event(this, new Trader.PointReachedArgs(reachedStructure));
            // }
            OnTraderArrivesToAStructure?.Invoke(this, new Trader.PointReachedArgs(reachedStructure));
        }

    } // class

} // namespace 
