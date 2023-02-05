using System;
using System.Collections.Generic;

namespace RailHexLib
{
    /// Handles needs of some object
    public class NeedsSystem : IUpdatable
    {

        public class Need
        {
            public Need(int cnt)
            {
                count = cnt;
            }
            int count;
            int filledCount = 0;

            // how many ticks this need is fullfiled
            int comboTicks = 0;
            public bool Filled => filledCount >= count;
            public int Count { get => count; }
            public int ComboTicks { get => comboTicks; }
            public int FilledCount { get => filledCount; }

            // TODO: fill with zero? how to reset filledCount?
            public bool Fill(int count)
            {
                filledCount = count;
                if (Filled)
                    this.comboTicks += 1;
                else
                    this.comboTicks = 0;
                return Filled;
            }
        }
        public class NeedsLevel
        {
            public bool Active = false;
            public Dictionary<Resource, Need> needs = new Dictionary<Resource, Need>();

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

        public NeedsSystem(Inventory inventory, List<Dictionary<Resource, int>> needLevels)
        {
            Inventory = inventory;
            foreach(var needs in needLevels)
            {
                var level = new NeedsLevel();
                this.levels.Add(level);
                foreach(var (resource, count) in needs)
                {
                    level.needs[resource] = new Need(count);
                }
            }
        }

        /*
        Should track time, while a need is fullfilled or not
        Create new needs, when basic needs is fullfilled
        */
        public void Tick(int ticks)
        {
            int unmeetNeedsCount = 0;
            foreach (var level in levels)
                unmeetNeedsCount += level.FillNeeds(Inventory);
        }

        internal void fillBy(Trader trader)
        {
            foreach (var level in levels)
            {
                if (level.Active)
                {
                    foreach (var (resource, need) in level.needs)
                    {
                        var picked = trader.Inventory.PickResource(resource, need.Count);
                    }
                }
            }

        }

        public int UnmeetNeeds { get => unmeetNeeds; }
        int unmeetNeeds = 0;

        public List<NeedsLevel> levels = new List<NeedsLevel>();
        Inventory Inventory;
    }

}