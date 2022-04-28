using RailHexLib;
using RailHexLib.Grounds;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Random = System.Random;
using RailHexLib.Interfaces;

namespace RailHexLib
{

    public class Game
    {

        
        public Game(TileStack stack = null, ILogger logger=null)
        {
            this.logger = logger;
            placedTiles = new Dictionary<Cell, Tile>(new CellEqualityComparer());
            structures = new Dictionary<Cell, StructureRoads>();
            if (stack == null)
            {
                this.rnd = new Random(1);
                InitializeDefaultStack();
            }
            else
            {
                this.stack = stack;
            }
        }


        public void AddStructures(List<Structure> structures)
        {
            foreach (var structure in structures)
            {
                this.structures[structure.Center] = new StructureRoads(structure);
            }
        }

        // task "place struct": IPlaceable
        public Tile CurrentTile { get => currentTile; }

        public Dictionary<Cell, Structure> Structures => structures.ToDictionary(kv => kv.Key, kv => kv.Value.structure);

        public List<Route> Routes
        {
            get => tradeRoutes;
        }
        /// <summary>
        /// Place tile on game board
        /// </summary>
        /// <param name="placedCell"></param>
        /// <returns>false if tile is not placed</returns>
        public bool PlaceCurrentTile(Cell placedCell)
        {
            if (!CanPlaceCurrentTile(placedCell))
            {
                return false;
            }
            logger.Log($"place tile {currentTile} on {placedCell}");
            placedTiles[placedCell] = currentTile;

            // TODO: make class for JoinedCells:
            // because we can need to know which tile was placed and
            // which - is exist (joined)
            // also may require custom hightlight algorithms
            Dictionary<Cell, Ground> joinsOfPlacedCell = FindJoins(placedCell);
            BuildRoads(joinsOfPlacedCell);

            // checks joins: if some join contains village - 

            // now we know all joins of placed cell. 
            //rebuildStructures(joinsOfPlacedCell);
            return true;
        }
        public void RotateCurrentTile()
        {
            currentTile.Rotate60Clock();
        }
        public void NextTile()
        {
            if (stack == null)
            {
                throw new NullReferenceException("Stack doesn't initialized");
            }
            currentTile = stack.PopTile();
        }

        private void BuildRoads(Dictionary<Cell, Ground> joinsOfPlacedCell)
        {
            Dictionary<StructureRoads, Cell> paved = new Dictionary<StructureRoads, Cell>();
            foreach (var structureGraph in structures.Values)
            {
                foreach (var possiblyNewRoad in joinsOfPlacedCell)
                {
                    if (!(possiblyNewRoad.Value is Road)) continue;

                    if (structureGraph.PaveTheRoad(new GraphNode<Cell>(possiblyNewRoad.Key)))
                    {
                        paved[structureGraph] = possiblyNewRoad.Key;
                    }
                }
            }
            if (paved.Keys.Count > 1)
            {
                var res = from pp in paved
                          group pp.Key by pp.Value;

                foreach (var group in res)
                {
                    var joineryCell = group.Key;

                    Route newRoute = new Route();
                    foreach (var structure in group)
                    {
                        // CONTINUE
                        // ADD struct to newRoute
                        newRoute.tradePoints.Add(structure.StartPoint, structure.structure);
                        // ADD points to Route
                        tradeRoutes.Add(newRoute);

                    }
                }

            }
        }

        private Dictionary<Cell, Ground> FindJoins(Cell placedCell)
        {
            Dictionary<Cell, Ground> joinsOfPlacedCell = new Dictionary<Cell, Ground>();
            foreach (var neighbor in placedCell.Neighbours()) // structures.Boundaries()) // place struct: if "currentTile" will be the cell, then curentTile.Neighbours() may return complex boundary
            {
                if (placedTiles.ContainsKey(neighbor))
                {

                    IdentityCell currentTileSide = new IdentityCell(neighbor - placedCell);

                    var currentTileSideBiome = currentTile.GetSideBiome(currentTileSide);

                    // skip unjoinable tiles
                    if (!currentTileSideBiome.IsJoinable())
                    {
                        continue;
                    }

                    Tile neighborTile = placedTiles[neighbor];
                    var neighborSideBiome = neighborTile.GetSideBiome(currentTileSide.Inverted());

                    logger.Log($"Compare side of currentTile {currentTileSide}+[{currentTile.Rotation}]({currentTileSideBiome}) with neighbors({neighbor}+[{neighborTile.Rotation}]) side {currentTileSide.Inverted()}({neighborSideBiome})");

                    if (currentTileSideBiome == neighborSideBiome)
                    {
                        logger.Log($"tile {currentTile}({placedCell}) has join {currentTileSideBiome} on {currentTileSide}");
                        joinsOfPlacedCell[neighbor] = currentTileSideBiome;
                    }
                }
            }

            return joinsOfPlacedCell;
        }

        private bool CanPlaceCurrentTile(Cell cell)
        {
            bool exists = placedTiles.ContainsKey(cell);
            logger.Log($"check placability on {cell} = {!exists}");
            return !exists;
        }

        private Tile MakeRandomTileType()
        {
            var tilesTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Tile))).ToArray();
            var randomValue = rnd.Next(tilesTypes.Length);
            var newType = tilesTypes[randomValue];
            return (Tile)Activator.CreateInstance(newType);
        }

        private void InitializeDefaultStack()
        {
            stack = new TileStack();
            for (int i = 0; i < 20; i++)
            {
                var tile = MakeRandomTileType();
                stack.AddTile(tile);
            }
        }

        private TileStack stack;
        private Random rnd;
        private Dictionary<Cell, Tile> placedTiles;
        private Tile currentTile;
        /// <summary>
        /// Key is center of structure
        /// Value is Structures placed in Cell and it's 
        /// </summary>
        private Dictionary<Cell, StructureRoads> structures;
        private List<Route> tradeRoutes = new List<Route>();
        private ILogger logger;

    }

    public class Route
    {
        public List<Cell> cells;
        public Dictionary<Cell, Structure> tradePoints;
    }

    public class StructureRoads
    {
        public StructureRoads(Structure structure)
        {
            this.structure = structure;
            roads = new GraphNode<Cell>(structure.GetIcomeRoadCell());
        }

        public Cell StartPoint => structure.GetIcomeRoadCell();
        public Structure structure;
        /// contains cell and it's road tile to know how roads 
        public GraphNode<Cell> roads;

        /// <summary>
        /// place cell in graph if it has join with some of other cells
        /// </summary>
        /// <param name="newRoadCell"></param>
        /// <returns>is road placed</returns>
        internal bool PaveTheRoad(GraphNode<Cell> newRoadCell)
        {
            var currentNode = roads;
            if (newRoadCell.Children.Contains(currentNode))
            {
                newRoadCell.Children.Remove(currentNode);
                currentNode.Children.Add(newRoadCell);
                return true;
            }

            return false;
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

    public class GraphNode<T>
    {
        public GraphNode(T v)
        {
            Value = v;
            Children = new List<GraphNode<T>>();
        }
        public T Value;
        public List<GraphNode<T>> Children;

    }

}
