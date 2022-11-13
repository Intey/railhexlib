using System.Collections.Generic;
using RailHexLib.Grounds;

namespace RailHexLib
{
    public class Zone
    {

        public List<Cell> Cells { get; } = new List<Cell>();

        public int ResourceCount { get; }
        public Ground ZoneType { get; }

        public Zone(int resorceCount, Ground zoneType)
        {
            ResourceCount = resorceCount;
            ZoneType = zoneType;
        }

        public void Extend(Cell newCell)
        {
            Cells.Add(newCell);
        }
        public void Extend(IEnumerable<Cell> cells)
        {
            Cells.AddRange(cells);
        }

        public bool Contains(Cell cell)
        {
            return Cells.Contains(cell);
        }

    }
}