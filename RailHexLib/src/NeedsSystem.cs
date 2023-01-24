using System;
using System.Collections.Generic;

namespace RailHexLib
{
    /// Handles needs of some object
    public class NeedsSystem : IUpdatable
    {

        public class Need
        {
            public Need(int cnt, int decTick)
            {
                count = cnt;
                decreaseOnTick = decTick;
            }
            int count;
            int decreaseOnTick;

            // how many ticks this need is fullfiled
            int comboTicks = 0;
            bool filled = false;
            public int Count { get => count; }
            public int ComboTicks { get => comboTicks; }

            public bool Fill(int count)
            {
                bool filled = this.count <= count;
                if (filled)
                    this.comboTicks += 1;
                else
                    this.comboTicks = 0;
                return filled;
            }
        }
        public class NeedsLevel
        {
            public Dictionary<Resource, Need> needs;

            /// consume reseurces from inventory and return count of unmeet needs
            public int FillNeeds(Inventory inventory)
            {
                int unmeetNeedsCount = 0;

                foreach (var (resource, need) in needs)
                {
                    var picked = inventory.PickResource(resource, need.Count);
                    if (!need.Fill(picked))
                        unmeetNeedsCount += 1;
                }
                return unmeetNeedsCount;
            }
        }

        public NeedsSystem(Inventory inventory, List<NeedsLevel> needs)
        {
            Inventory = inventory;
            this.levels = needs;
        }

        /*
        Should track time, while a need is fullfilled or not
        Create new needs, when basic needs is fullfilled
        */
        public void Tick(int ticks)
        {
            int unmeetNeedsCount = 0;
            foreach (var level in levels)
                level.FillNeeds(Inventory);

        }

        internal void fillBy(Trader trader)
        {
            foreach (var level in levels)
                foreach (var (resource, need) in level.needs)
                {
                    var picked = trader.Inventory.PickResource(resource, need.Count);
                }
        }

        public List<NeedsLevel> levels;
        Inventory Inventory;
    }

}