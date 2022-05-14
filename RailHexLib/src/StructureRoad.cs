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
            road = new GraphNode<Cell>(structure.GetEnterCell());
        }

        public Cell StartPoint => structure.GetEnterCell();
        public Structure structure;
        /// contains cell and it's road tile to know how roads 
        public GraphNode<Cell> road;

        /// <summary>
        /// place cell in graph if it has join with some of other cells
        /// </summary>
        /// <param name="newRoadCell"></param>
        /// <returns>is road placed</returns>
        internal bool TryAddToRoad(GraphNode<Cell> newRoadCell)
        {
            // find where newRoadChild placed in current structure road
            return road.AddToChildrenBy(newRoadCell.Value, (roadNode, _) =>
            {
                foreach (var newRoadChild in newRoadCell.Children)
                {
                    if (roadNode.Equals(newRoadChild.Value)) return true;
                }
                return false;
            }) != null;

        }
        public static GraphNode<Cell> CellTileToGraphNode(Cell position, Tile tile)
        {
            var roadSides = from s in tile.Sides
                            where s is Road
                            select s.Key;
            var result = new GraphNode<Cell>(position);
            foreach (var cell in roadSides)
            {
                result.Children.Add(new GraphNode<Cell>(position + cell));
            }
            return result;
        }

    }

}
