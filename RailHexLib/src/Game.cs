using RailHexLib;
using RailHexLib.Grounds;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Random = System.Random;
using RailHexLib.Interfaces;
using RailHexLib.DevTools;

namespace RailHexLib
{

    public class Game
    {

        public Game(TileStack stack = null, ILogger logger = null)
        {
            this.logger = logger ?? new DefaultSilentLogger();
            placedTiles = new Dictionary<Cell, Tile>(new CellEqualityComparer());
            structureRoads = new Dictionary<Cell, StructureRoad>();
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
                this.structureRoads[structure.Center] = new StructureRoad(structure);
                foreach (var cell in structure.GetHexes())
                    placedTiles[cell.Key] = cell.Value;
            }
        }

        // task "place struct": IPlaceable
        public Tile CurrentTile { get => currentTile; }

        public Dictionary<Cell, Structure> Structures => structureRoads.ToDictionary(kv => kv.Key, kv => kv.Value.structure);

        public void PushTile(Tile newTile)
        {
            stack.PushTile(newTile);
        }

        public List<TradeRoute> Routes
        {
            get => tradeRoutes;
        }

        /// <summary>
        /// Place tile on game board
        /// </summary>
        /// <param name="placedCell"></param>
        /// <returns>false if tile is not placed</returns>
        public PlacementResult PlaceCurrentTile(Cell placedCell)
        {
            if (!CanPlaceCurrentTile(placedCell))
            {
                return PlacementResult.Fail;
            }
            logger.Log($"place tile {currentTile} on {placedCell}");
            placedTiles[placedCell] = currentTile;

            
            Dictionary<Cell, Ground> joinedNeighbors = FindJoinedNeighbors(placedCell);
            // each of the objects is exactly in one of the graphs: orphan roads, strcture roads or tradeRoutes
            var joinedRoads = from t in joinedNeighbors where t.Value is Road select t.Key;

            // make tree of current Hex
            var placedHexNodeRoads = new HexNode(placedCell);
            
            bool hasChangedOrphan = false;
            bool hasChangedRoads = false;
            bool hasChangedTradeRoute = false;

            // check orphans
            foreach (var joinedRoadCell in joinedRoads)
            {

                hasChangedOrphan = AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, orphanRoads);

                var roads = structureRoads.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.road);
                // TODO: should return list of merged structures to join them in tradeRoute

                hasChangedRoads = AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, roads);
                // hasChangedTradeRoute = AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, tradeRoutes);
            }
            // promoted to structures. Remove from orphans
            if (hasChangedOrphan && hasChangedRoads)
            {
                orphanRoads.Remove(placedHexNodeRoads.Cell);
            }
            if (hasChangedRoads && hasChangedTradeRoute)
            {
                structureRoads.Remove(placedHexNodeRoads.Cell);
            }


            // check structures
            // if tile added to orphan (or merge multiple of them)
            //  - add to structure road whole orphan with current tile as parent and drop from orphans
            // if tile added to multiple structures, merge them in trade road
            // BuildRoads(placedHexNodeRoads);

            return new PlacementResult(true, joinedNeighbors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="placedHexNodeRoads"></param>
        /// <param name="joineryCell"></param>
        /// <param name="roadsForMerge"></param>
        /// <param name="alreadyMergedRoads">Roads with it's merge joinery</param>
        /// <returns></returns>
        private bool AddToPlacedGraphs(HexNode placedHexNodeRoads,
                                       Cell joineryCell,
                                       Dictionary<Cell, HexNode> roadsForMerge)
        {

            bool done = false;
            foreach (var road in roadsForMerge)
            {
                // add the placedCell to an orphan graph 
                // if the joined road cell in the orphan graph

                HexNode roadJoineryOwner = road.Value.FindCell(joineryCell);
                if (roadJoineryOwner != null)
                {
                    // replace with new orphan with parent - current Tile
                    roadsForMerge.Remove(road.Key);

                    HexNode parentForPlacedNode = road.Value.Add(placedHexNodeRoads);

                    Debug.Assert(parentForPlacedNode.Cell == joineryCell, "possibly incosistence");

                    roadsForMerge[placedHexNodeRoads.Cell] = placedHexNodeRoads;
                    done = true;
                }
            }
            return done;
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

        private bool BuildRoads(HexNode roadGraph)
        {
            bool result = false;
            // StructureRoads paved by Key
            Dictionary<Cell, List<StructureRoad>> roadsToJoin = new Dictionary<Cell, List<StructureRoad>>();

            // TODO: check the order: if we try cell (0,-2) on graph (0,0) we fail.
            // But in next iteration we can have (0,-1) which linked with (0,-2) and (0,0)
            foreach (var structureGraph in structureRoads.Values)
            {
                if (structureGraph.AddToRoad(roadGraph))
                {
                    result = true;
                    // is first and just create empty list
                    if (!roadsToJoin.ContainsKey(roadGraph.Cell)) roadsToJoin[roadGraph.Cell] = new List<StructureRoad>();
                    // add current struct to paved
                    roadsToJoin[roadGraph.Cell].Add(structureGraph);
                }
            }
            // build trade route
            BuildTradeRoutes(roadsToJoin.Where(g => g.Value.Count > 1));
            return result;
        }

        private void BuildTradeRoutes(IEnumerable<KeyValuePair<Cell, List<StructureRoad>>> RoadsToJoin)
        {
            foreach (var group in RoadsToJoin)
            {
                var joineryCell = group.Key;
                var pairs = Utils.MakePairs(group.Value);
                foreach (var pair in pairs)
                {
                    TradeRoute newRoute = new TradeRoute();
                    // ADD struct to newRoute
                    newRoute.tradePoints.Add(pair[0].StartPoint, pair[0].structure);
                    newRoute.tradePoints.Add(pair[1].StartPoint, pair[1].structure);
                    newRoute.cells.AddRange(pair[0].road.PathTo(joineryCell).Reverse<Cell>()); // add income cell too
                    newRoute.cells.Add(joineryCell);
                    newRoute.cells.AddRange(pair[1].road.PathTo(joineryCell));
                    // ADD points to Route
                    tradeRoutes.Add(newRoute);
                }

            }
        }

        /// <summary>
        /// find hexes with wich newly placed tile has joins
        /// </summary>
        /// <param name="placedCell"></param>
        /// <returns>Dictionary of neighbors that joined by Ground(in Value) with `placedCell`</returns>
        private Dictionary<Cell, Ground> FindJoinedNeighbors(Cell placedCell)
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

                    logger.Log($"Compare side {currentTileSide} of currentTile {placedCell}+[{currentTile.Rotation}]({currentTileSideBiome}) with neighbors({neighbor}+[{neighborTile.Rotation}]) side {currentTileSide.Inverted()}({neighborSideBiome})");

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
                stack.PushTile(tile);
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
        private Dictionary<Cell, StructureRoad> structureRoads;
        /// <summary>
        /// Contains orphan roads where Key is root Node
        /// </summary>
        private Dictionary<Cell, HexNode> orphanRoads = new Dictionary<Cell, HexNode>();
        private List<TradeRoute> tradeRoutes = new List<TradeRoute>();
        private ILogger logger;


        /// <summary>
        /// Cells where Player can place new one
        /// </summary>
        /// <returns></returns>
        IEnumerable<Cell> PlacableCells()
        {
            // get neighbors for each cell in this list
            //var source = orphanRoads.Keys.Concat(structureRoads.Keys);
            return new List<Cell>();
        }
    }
}
