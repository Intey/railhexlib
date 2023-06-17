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
        public NeedsSystem(Inventory inventory, List<NeedsLevel> needLevels)
        {
            Inventory = inventory;
            levels = needLevels;
        }
        public NeedsSystem(Inventory inventory, params NeedsLevel[] needLevels)
        {
            Inventory = inventory;
            levels = needLevels.ToList();
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

        public List<Need> UnmeetNeeds
        {

            get
            {
                List<Need> res = new();
                foreach (var level in levels)
                {
                    foreach (var need in level.Needs)
                    {
                        if (!need.Filled) res.Add(need);
                    }
                }
                return res;
            }
        }

        public List<NeedsLevel> levels = new List<NeedsLevel>();
        Inventory Inventory;

        public class Need
        {
            public Need(Resource res, int count, int consumptionTicks)
            {
                resource = res;
                this.ExpectedCount = count;
                this.consumptionTicks = consumptionTicks;
            }
            int filledCount = 0;
            Resource resource;
            public bool Filled => filledCount >= ExpectedCount;
            // count of required resource
            public readonly int ExpectedCount;
            public int FilledCount { get => filledCount; }
            readonly int consumptionTicks;
            int ticks = 0;
            /// Technical property shows that this need require to fill on next tick
            internal bool RequireReplenish => ticks >= consumptionTicks;

            public Resource Resource => resource;

            // TODO: fill with zero? how to reset filledCount?
            public bool Fill(int count)
            {
                ticks = 0; // Filled. Start timer again.
                filledCount = count;
                return Filled;
            }
            public void Tick(int ticks)
            {
                this.ticks += ticks;
                if (RequireReplenish)
                {
                    filledCount -= ExpectedCount;
                    if (filledCount <= 0)
                    {
                        filledCount = 0;
                    }
                    ticks = 0;
                }
            }

            public override string ToString()
            {
                return $"Need of {resource} {filledCount}/{ExpectedCount}";
            }
        }
        public class NeedsLevel
        {
            public NeedsLevel(List<Need> needs) { Needs = needs; }
            public NeedsLevel(params Need[] needs) { Needs = needs.ToList(); }
            public bool Active = true;
            public List<Need> Needs = new();

            public bool Filled => !Needs.Any((need) => !need.Filled);
            /// consume reseurces from inventory and return count of unmeet needs
            public int FillNeeds(Inventory inventory)
            {
                int unmeetNeedsCount = 0;

                foreach (var need in Needs)
                {
                    if (need.RequireReplenish)
                    {
                        var picked = inventory.PickResource(need.Resource, need.ExpectedCount);
                        if (!need.Fill(picked))
                            unmeetNeedsCount += 1;
                    }
                }
                return unmeetNeedsCount;
            }
            public void Tick(int ticks)
            {
                foreach (var need in Needs)
                {
                    need.Tick(ticks); ;
                }
            }

            private Timer timerNeeds;

        }
    }

}