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
        // task "place struct": IPlaceable
        public Tile CurrentTile { get => currentTile; }

        public Dictionary<Cell, Structure> Structures => structureRoads.ToDictionary(kv => kv.Key, kv => kv.Value.structure);
        public Dictionary<Cell, StructureRoad> StructureRoads => structureRoads;


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
                this.structureRoads[structure.GetEnterCell()] = new StructureRoad(structure);
                foreach (var cell in structure.GetHexes())
                    placedTiles[cell.Key] = cell.Value;
            }
        }

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

            var placementResult = new PlacementResult(true);

            Dictionary<Cell, Ground> joinedNeighbors = FindJoinedNeighbors(placedCell);
            placementResult.NewJoins = joinedNeighbors;
            // each of the objects is exactly in one of the graphs: orphan roads, strcture roads or tradeRoutes
            var joinedRoads = from t in joinedNeighbors where t.Value is Road select t.Key;

            logger.Log($"Game.PlaceCurrentTile: joins {joinedRoads}");
            // make tree of current Hex
            var placedHexNodeRoads = new HexNode(placedCell);

            List<Cell> hasChangedOrphan = new List<Cell>();
            List<Cell> hasChangedRoads = new List<Cell>();
            List<Cell> hasChangedTradeRoute = new List<Cell>();

            // check orphans
            foreach (var joinedRoadCell in joinedRoads)
            {
                logger.Log($"Game.PlaceCurrentTile: process joinedCell {joinedRoadCell}");
                hasChangedOrphan = AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, orphanRoads);

                placementResult.NewOrphanRoads = hasChangedOrphan.Select(k => orphanRoads[k]).ToList();

                var roads = structureRoads.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.road);
                // TODO: should return list of merged structures to join them in tradeRoute

                hasChangedRoads = AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, roads);
                // hasChangedTradeRoute = AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, tradeRoutes);
                placementResult.NewStructureRoads = hasChangedRoads.Select(k => structureRoads[k]).ToList();
            }
            // promoted to structures. Remove from orphans
            if (hasChangedOrphan.Count >= 1 && hasChangedRoads.Count >= 1)
            {
                // clear old keys
                foreach(var k in hasChangedOrphan) orphanRoads.Remove(k);
                // remove created merged orphan - now it's in a structureRoad
                orphanRoads.Remove(placedHexNodeRoads.Cell);

            }
            if (hasChangedRoads.Count > 1)
            {
                // clear old keys
                foreach(var k in hasChangedRoads) structureRoads.Remove(k);

                var changedStructureRoads = hasChangedRoads.Select(cell => structureRoads[cell]);

                var items = new List<KeyValuePair<Cell, IEnumerable<StructureRoad>>>();
                items.Add(new KeyValuePair<Cell, IEnumerable<StructureRoad>>(placedHexNodeRoads.Cell, changedStructureRoads));

                BuildTradeRoutes(items);

                structureRoads.Remove(placedHexNodeRoads.Cell);
            }


            // check structures
            // if tile added to orphan (or merge multiple of them)
            //  - add to structure road whole orphan with current tile as parent and drop from orphans
            // if tile added to multiple structures, merge them in trade road
            // BuildRoads(placedHexNodeRoads);

            placementResult.GameOver = !NextTile();
            return placementResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="placedHexNodeRoads"></param>
        /// <param name="joineryCell"></param>
        /// <param name="roadsForMerge"></param>
        /// <param name="alreadyMergedRoads">Roads with it's merge joinery</param>
        /// <returns></returns>
        private List<Cell> AddToPlacedGraphs(HexNode placedHexNodeRoads, Cell joineryCell, Dictionary<Cell, HexNode> roadsForMerge)
        {
            var result = new List<Cell>();
            foreach (var roadKey in roadsForMerge.Keys.ToList())
            {
                // add the placedCell to an orphan graph 
                // if the joined road cell in the orphan graph

                logger.Log($"Game.AddToPlacedGraphs: process roadKey {roadKey}");

                HexNode roadJoineryOwner = roadsForMerge[roadKey].FindCell(joineryCell);
                logger.Log($"found joinery {roadJoineryOwner} on key {roadKey}");
                if (roadJoineryOwner != null)
                {
                    result.Add(roadKey);

                    HexNode parentForPlacedNode = roadJoineryOwner.Add(placedHexNodeRoads);

                    Debug.Assert(parentForPlacedNode.Cell.Equals(joineryCell), "possibly incosistence");

                    roadsForMerge[placedHexNodeRoads.Cell] = placedHexNodeRoads;
                }
            }
            return result.ToList();
        }

        public void RotateCurrentTile(int count=1)
        {
            for(int i=0; i < count; i++) {
                currentTile.Rotate60Clock();
            }
        }

        public bool NextTile()
        {
            if (stack == null)
            {
                throw new NullReferenceException("Stack doesn't initialized");
            }
            currentTile = stack.PopTile();

            if (currentTile == null) return false;

            return true;
        }

        /*
        private bool BuildRoads(HexNode roadGraph)
        {
            bool result = false;
            // StructureRoads paved by Key
            Dictionary<Cell, List<StructureRoad>> roadsToJoin = new Dictionary<Cell, List<StructureRoad>>();

            // TODO: check the order: if we try cell (0,-2) on graph (0,0) we fail.
            // But in next iteration we can have (0,-1) which linked with (0,-2) and (0,0)
            foreach (var structureGraph in structureRoads.Values)
            {
                if (structureGraph.AddToRoad(roadGraph) != null)
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

        */

        private void BuildTradeRoutes(IEnumerable<KeyValuePair<Cell, IEnumerable<StructureRoad>>> RoadsToJoin)
        {
            foreach (var group in RoadsToJoin)
            {
                var joineryCell = group.Key;
                var pairs = Utils.MakePairs<StructureRoad>(group.Value.ToList());
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
            Debug.Assert(currentTile != null, "Current tile should exists when searching neighbors for placedCell");
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
