using System.Collections.Generic;
using RailHexLib.Grounds;
using System.Linq;
using System.Diagnostics;
using System;

namespace RailHexLib
{
    // for making frontier of zone for better performan
    public class FrontierCell : Cell
    {
        List<IdentityCell> joinedSides = new List<IdentityCell>() { };

        public FrontierCell(Cell b) : base(b.R, b.Q, b.size) { }

        public void Join(FrontierCell other)
        {
            Debug.Assert(DistanceTo(other) == 1, "Joined cell should be siblings");
            joinedSides.Add(this.GetDirectionTo(other));
            other.joinedSides.Add(other.GetDirectionTo(this));
        }
        public bool Full() => joinedSides.Count == 6;
    }


    public class Zone
    {
        public event EventHandler OnZoneAbandon;
        public List<FrontierCell> Cells { get; } = new List<FrontierCell>();

        public int ResourceCount => resourceCount;
        private int resourceCount;
        public Ground ZoneType { get; }
        public Resource ResourceType { get; }

        // public List<Cell> Frontiers => Cells.Where(x => !x.Full()).Cast<Cell>().ToList();
        public Zone(Cell center, int resourceCount, Ground zoneType)
        {
            Extend(center);
            this.resourceCount = resourceCount;
            ZoneType = zoneType;
            switch(ZoneType) {
                case Ground.Water:
                    ResourceType = Resource.Fish;
                    break;
                case Ground.Grass:
                    ResourceType = Resource.Grass;
                    break;
                case Ground.Forest:
                    ResourceType = Resource.Wood;
                    break;
                default:
                    throw new System.NotImplementedException($"add new Resource type for ground {zoneType} in Zone constructor");
            }
        }

        public void Extend(Cell newCell)
        {
            var newFrontierCell = new FrontierCell(newCell);
            Cells.Add(newFrontierCell);
            foreach (var cell in Cells)
            {
                if (cell.DistanceTo(newCell) == 1)
                {
                    cell.Join(newFrontierCell);
                }
            }

        }
        public void Extend(IEnumerable<Cell> cells)
        {
            Cells.AddRange(cells.Cast<FrontierCell>());
        }

        public bool Contains(Cell cell)
        {
            return Cells.Contains(cell);
        }

        public int ConsumeResource(int count ) 
        {
            int consumed = 0;
            if (resourceCount <= count) 
            {
                consumed = resourceCount;
                // TODO: abandon zone
            } 
            else
            {

                consumed = count;
            }
            resourceCount -= consumed;
            
            return consumed;
        }
    }
}