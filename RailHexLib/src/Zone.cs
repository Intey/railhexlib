using System.Collections.Generic;

namespace RailHexLib
{
    public class Zone
    {
        public Zone(int resorceCount)
        {
            ResourceCount = resorceCount;
        }

        public void Expand(Cell newCell)
        {
            Cells.Add(newCell);
        }

        public List<Cell> Cells { get; }

        public int ResourceCount { get; }
    }
}