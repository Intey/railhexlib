using RailHexLib.Grounds;
using System.Collections.Generic;
using System.Linq;

namespace RailHexLib
{
    public class StructureRoad
    {
        public StructureRoad(Structure structure)
        {
            this.structure = structure;
            road = new HexNode(structure.GetEnterCell());
        }

        public Cell StartPoint => structure.GetEnterCell();
        public Structure structure;
        /// contains cell and it's road tile to know how roads 
        public HexNode road;

        /// <summary>
        /// place cell in graph if it has join with some of other cells
        /// </summary>
        /// <param name="newRoadCell"></param>
        /// <returns>is road placed</returns>
        internal HexNode AddToRoad(HexNode newRoadCell)
        {
            return road.Add(newRoadCell);
        }

    }

}
