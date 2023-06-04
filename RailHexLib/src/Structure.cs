using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RailHexLib
{
    using NeedLevelList = List<Dictionary<Resource, (int, int)>>;
    public abstract class Structure : Interfaces.IRotatable<Tile>
    {
        public event EventHandler OnStructureAbandon;

        public abstract string TileName();

        public abstract Cell GetEnterCell();

        /// <summary>
        /// Return all hexes that included in settlement. 
        /// Also includes center hex.
        /// </summary>
        /// <returns>Pairs of cells (positons) and tiles (values)</returns>
        public virtual Dictionary<Cell, Tile> GetHexes()
        {
            var result = new Dictionary<Cell, Tile>
            {
                [center] = new GrassTile()
            };
            return result;

        }
        public Structure(Cell center,
                         string name,
                         NeedLevelList needs = null,
                         Dictionary<Resource, int> resources = null
        )
        {
            this.center = center; this.name = name;
            if (needs != null)
            {
                this.needsSystem = new NeedsSystem(Inventory, needs);
            }
            else
            {
                needsSystem = new NeedsSystem(Inventory, new NeedLevelList());
            }
            if (resources != null)
            {
                foreach (var (r, c) in resources)
                {
                    this.Inventory.AddResource(r, c);
                }
            }
        }

        public Dictionary<Resource, int> Resources => Inventory.Resources;

        public List<NeedsSystem.NeedsLevel> NeedLevels { get => needsSystem.levels; }

        public bool Abandoned => abandoned;

        public Cell Center { get => center; }

        public int LifeTime { get => lifeTime; }
        // cell is connection point
        public Dictionary<Cell, Zone> ConnectedZones { get; } = new Dictionary<Cell, Zone>();
        public string Name
        {
            get => name;
            set
            {
                if (value.Length > 0)
                {
                    name = value;
                }
                else
                {
                    throw new ArgumentException("name should be non empty string");
                }
            }
        }
        public Inventory Inventory = new Inventory();

        public void ConnectZone(Zone zone)
        {
            foreach (var zoneCell in zone.Cells)
            {
                Debug.WriteLine($"check {zone} {zoneCell}...");
                foreach (var (c, _) in GetHexes())
                {
                    // search connection point
                    if (zoneCell.DistanceTo(c) == 1)
                    {
                        ConnectedZones[zoneCell] = zone;
                        // TODO: subscibe zone actions
                        // zone.OnCuncsumedOut
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"Structure ${name}";
        }

        public virtual void Tick(int ticks = 1)
        {
            if (abandoned)
            {
                return;
            }

            for (int t = 0; t < ticks; ++t)
            {
                tickZones();
            }

            needsSystem.Tick(ticks);

            if (lifeTime > 0)
            {
                // lifeTime -= ticks;

                int unmeetNeeds = needsSystem.UnmeetNeeds;
                lifeTime -= ticks * unmeetNeeds;
                if (lifeTime <= 0)
                {
                    abandoned = true;
                    abandon();
                }
            }
        }


        public virtual void VisitTrader(Trader trader)
        {
            lifeTime += Config.Structure.LifeTimeIncreaseOnTraderVisit;
            needsSystem.fillBy(trader.Inventory);
        }

        public int PickResource(Resource name, int count)
        {
            return Inventory.PickResource(name, count);
        }
        public void addResource(Resource name, int count)
        {
            Inventory.AddResource(name, count);
        }

        void abandon()
        {
            EventHandler handler = OnStructureAbandon;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        void tickZones()
        {
            var toDisconnect = new List<Cell>();
            foreach (var (c, zone) in ConnectedZones)
            {
                if (!Inventory.canAcceptResource(zone.ResourceType, Config.Structure.ZoneConsumptionCount))
                {
                    continue;
                }
                int consumed = zone.ConsumeResource(Config.Structure.ZoneConsumptionCount);
                if (zone.ResourceCount == 0)
                {
                    toDisconnect.Add(c);
                }
                Inventory.AddResource(zone.ResourceType, consumed);
            }
            foreach (var c in toDisconnect)
            {
                ConnectedZones.Remove(c);
            }
        }
        protected Cell center;
        protected string name = "Unnamed";
        protected NeedsSystem needsSystem;

        private int lifeTime = Config.Structure.InitialTicksToDie;
        private bool abandoned = false;

    } // class

} // namespace
