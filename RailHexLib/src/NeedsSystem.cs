using System;
using System.Collections.Generic;
using System.Linq;
namespace RailHexLib
{
    // List of resources needed. Each resource is a tuple of (count, ticks to consume)
    using NeedLevelList = List<Dictionary<Resource, (int, int)>>;
    /// Handles needs of some object
    public class NeedsSystem
    {
        public NeedsSystem(Inventory inventory, NeedLevelList needLevels)
        {
            Inventory = inventory;
            foreach (var needs in needLevels)
            {
                var level = new NeedsLevel();
                this.levels.Add(level);
                foreach (var (resource, (count, ticksToConsume)) in needs)
                {
                    level.Needs[resource] = new Need(count, ticksToConsume);
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
                level.Tick(ticks);
            }
            //try to fill by linked inventory
            fillBy(Inventory);
        }
        // Initiate process of filling needs
        internal void fillBy(Inventory inventory)
        {
            foreach (var level in levels)
            {
                if (level.Active)
                {
                    level.FillNeeds(inventory);
                }
            }

        }

        public int UnmeetNeeds
        {
            get
            {
                int count = 0;
                foreach (var level in levels)
                {
                    foreach (var (_, need) in level.Needs)
                    {
                        if (!need.Filled) count++;
                    }
                }
                return count;
            }
        }

        public List<NeedsLevel> levels = new List<NeedsLevel>();
        Inventory Inventory;

        public class Need
        {
            public Need(int cnt, int consumptionTicks)
            {
                count = cnt;
                this.consumptionTicks = consumptionTicks;
            }
            readonly int count;
            int filledCount = 0;

            public bool Filled => filledCount >= count;
            // count of required resource
            public int ExpectedCount { get => count; }
            public int FilledCount { get => filledCount; }
            readonly int consumptionTicks;
            int ticks = 0;
            public bool NeedToFill => ticks >= consumptionTicks;
            // TODO: fill with zero? how to reset filledCount?
            public bool Fill(int count)
            {
                ticks = 0; // reset NeedToFill
                filledCount = count;
                return Filled;
            }
            public void Tick(int ticks)
            {
                this.ticks += ticks;
                if (NeedToFill)
                {
                    filledCount -= count;
                    if (filledCount <= 0)
                    {
                        filledCount = 0;
                    }
                    ticks = 0;
                }
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
                    if (need.NeedToFill)
                    {
                        var picked = inventory.PickResource(resource, need.ExpectedCount);
                        if (!need.Fill(picked))
                            unmeetNeedsCount += 1;
                    }
                }
                return unmeetNeedsCount;
            }
            public void Tick(int ticks)
            {
                foreach (var (_, need) in Needs)
                {
                    need.Tick(ticks); ;
                }
            }

            private Timer timerNeeds;

        }
    }

}