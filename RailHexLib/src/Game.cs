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

    public class Game : IUpdatable
    {
        public Tile CurrentTile => currentTile;
        public Dictionary<Cell, Structure> Structures => structureRoads.ToDictionary(kv => kv.Key, kv => kv.Value.structure);
        public Dictionary<Cell, StructureRoad> StructureRoads => structureRoads;
        public List<TradeRoute> Routes => tradeRoutes;
        public int ScorePoints => scorePoints;

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


        /// <summary>
        /// Place tile on game board
        /// </summary>
        /// <param name="placedCell"></param>
        /// <returns>false if tile is not placed</returns>
        public PlacementResult PlaceCurrentTile(Cell placedCell)
        {
            Debug.Assert(currentTile != null, "Current tile should exists. Forget to call NextTile()?");
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
            // make tree of current Hex
            var placedHexNodeRoads = new HexNode(placedCell);

            HashSet<Cell> hasChangedOrphan = new HashSet<Cell>();
            HashSet<Cell> hasChangedRoads = new HashSet<Cell>();

            // no joins, make new orphan
            if (joinedRoads.Count() == 0 && currentTile.Sides.ContainsValue(Road.instance))
            {
                orphanRoads[placedCell] = placedHexNodeRoads;
                placementResult.NewOrphanRoads.Add(orphanRoads[placedCell]);
            }
            // check joins
            foreach (var joinedRoadCell in joinedRoads)
            {
                logger.Log($"Game.PlaceCurrentTile: process joinedCell {joinedRoadCell}");
                hasChangedOrphan.UnionWith(AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, orphanRoads));

                placementResult.NewOrphanRoads.AddRange(hasChangedOrphan.Select(k => orphanRoads[k]).ToList());

                var roads = structureRoads.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.road);
                // TODO: should return list of merged structures to join them in tradeRoute

                hasChangedRoads.UnionWith(AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, roads));

            }

            // hasChangedTradeRoute = AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, tradeRoutes);
            placementResult.NewStructureRoads = hasChangedRoads.Select(k => structureRoads[k]).ToList();

            // promoted to structures. Remove from orphans
            if (hasChangedOrphan.Count >= 1 && hasChangedRoads.Count >= 1)
            {
                // clear old keys
                foreach (var k in hasChangedOrphan) orphanRoads.Remove(k);
                // remove created merged orphan - now it's in a structureRoad
                orphanRoads.Remove(placedHexNodeRoads.Cell);

            }
            if (hasChangedRoads.Count > 1)
            {

                var changedStructureRoads = hasChangedRoads.Select(cell => structureRoads[cell]);

                var items = new List<KeyValuePair<Cell, IEnumerable<StructureRoad>>>();
                items.Add(new KeyValuePair<Cell, IEnumerable<StructureRoad>>(placedHexNodeRoads.Cell, changedStructureRoads));

                var newRoutes = BuildTradeRoutes(items);
                placementResult.NewTradeRoutes = newRoutes;
                this.tradeRoutes.AddRange(newRoutes);

                // clear old keys
                foreach (var k in hasChangedRoads) structureRoads.Remove(k);
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
                // joinery cell should be in one of all: orphan, road or trade route
                HexNode roadJoineryOwner = roadsForMerge[roadKey].FindCell(joineryCell);
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

        public void RotateCurrentTile(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
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

        public void Update(int ticks)
        {

            foreach (var t in tradeRoutes)
            {
                t.Update(ticks);
            }

        }
        internal void EmitTradePointReached()
        {
            scorePoints += Config.ScorePoints.ForTradePointReached;
            // TODO: add tiles to stack
            for (int i = 0; i < Config.Stack.TilesForTradePointReached; i++)
            {
                PushTile(MakeRandomTileType());
            }

        }

        private List<TradeRoute> BuildTradeRoutes(IEnumerable<KeyValuePair<Cell, IEnumerable<StructureRoad>>> RoadsToJoin)
        {
            var result = new List<TradeRoute>();
            foreach (var group in RoadsToJoin)
            {
                var joineryCell = group.Key;
                var pairs = Utils.MakePairs<StructureRoad>(group.Value.ToList());
                foreach (var pair in pairs)
                {
                    List<Cell> path = new List<Cell>();
                    // ADD struct to newRoute
                    Dictionary<Cell, Structure> points = new Dictionary<Cell, Structure>();
                    points.Add(pair[0].StartPoint, pair[0].structure);
                    points.Add(pair[1].StartPoint, pair[1].structure);

                    path.AddRange(pair[0].road.PathTo(joineryCell).Where<Cell>(i => !i.Equals(joineryCell))); // add income cell too
                    path.Add(joineryCell);
                    path.AddRange(pair[1].road.PathTo(joineryCell).Where<Cell>(i => !i.Equals(joineryCell)).Reverse<Cell>());
                    TradeRoute newRoute = new TradeRoute(path, points, this);
                    // ADD points to Route
                    result.Add(newRoute);
                }

            }
            return result;
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
                    logger.Log($"FindJoined: check neighbor {neighbor} for {placedCell}");
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
        private int scorePoints;

    }
}
