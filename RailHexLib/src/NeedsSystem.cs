using System;
using System.Collections.Generic;
using System.Linq;
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
            // count of required resource
            public int ExpectedCount { get => count; }
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
            public bool Active = true;
            public Dictionary<Resource, Need> Needs = new Dictionary<Resource, Need>();

            public bool Filled => !Needs.Any((ResNeedPair) => !ResNeedPair.Value.Filled);
            /// consume reseurces from inventory and return count of unmeet needs
            public int FillNeeds(Inventory inventory)
            {
                int unmeetNeedsCount = 0;

                foreach (var (resource, need) in Needs)
                {
                    var picked = inventory.PickResource(resource, need.ExpectedCount);
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
                    level.Needs[resource] = new Need(count);
                }
            }
        }

        /*
        Should track time, while a need is fullfilled or not
        Create new needs, when basic needs is fullfilled
        */
        public void Tick(int ticks)
        {
            foreach (var level in levels)
            {
                 level.FillNeeds(Inventory);
            }
        }

        internal void fillBy(Trader trader)
        {
            foreach (var level in levels)
            {
                if (level.Active)
                {
                    foreach (var (resource, need) in level.Needs)
                    {
                        var picked = trader.Inventory.PickResource(resource, need.ExpectedCount);
                    }
                }
            }

        }

        public int UnmeetNeeds { 
            get {
                int count = 0;
                foreach (var level in levels)
                {
                    foreach(var (_, need) in level.Needs)
                    {
                        if (!need.Filled) count++;
                    }
                }
                return count;   
            }
        }

        public List<NeedsLevel> levels = new List<NeedsLevel>();
        Inventory Inventory;
    }

}