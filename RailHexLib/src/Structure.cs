using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RailHexLib
{
    public abstract class Structure : Interfaces.IRotatable<Tile>
    {
        public Structure(Cell center,
                         string name,
                         List<Dictionary<Resource, int>> needs = null,
                         Dictionary<Resource, int> resources = null
        )
        {
            this.center = center; this.name = name;
            if (needs != null)
            {
                this.needsSystem = new NeedsSystem(this.Inventory, needs);
            }
            else {
                needsSystem = new NeedsSystem(Inventory, new List<Dictionary<Resource, int>>());
            }
            if (resources != null)
            {
                foreach (var (r, c) in resources)
                {
                    this.Inventory.AddResource(r, c);
                }
            }
        }
        public Cell Center { get => center; }
        protected Cell center;
        protected string name = "Unnamed";

        public event EventHandler OnStructureAbandon;
        public int LifeTime { get => lifeTime; }
        private int lifeTime = Config.Structure.InitialTicksToDie;
        private bool abandoned = false;
        // cell is connection point
        public Dictionary<Cell, Zone> ConnectedZones { get; } = new Dictionary<Cell, Zone>();

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
        public override string ToString()
        {
            return $"Structure ${name}";
        }

        public virtual void Tick(int ticks)
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
                lifeTime -= ticks;

                int unmeetNeeds = needsSystem.UnmeetNeeds;
                lifeTime -= ticks * unmeetNeeds;
                if (lifeTime <= 0)
                {
                    abandoned = true;
                    abandon();
                }
            }
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

        public virtual void VisitTrader(Trader trader)
        {
            lifeTime += Config.Structure.LifeTimeIncreaseOnTraderVisit;
            needsSystem.fillBy(trader);
        }

        public int PickResource(Resource name, int count)
        {
            return Inventory.PickResource(name, count);
        }
        public void addResource(Resource name, int count)
        {
            Inventory.AddResource(name, count);
        }

        public Dictionary<Resource, int> Resources => Inventory.Resources;

        public Inventory Inventory = new Inventory();
        NeedsSystem needsSystem;

        public List<NeedsSystem.NeedsLevel> Needs { get => needsSystem.levels; }
    } // class

} // namespace
