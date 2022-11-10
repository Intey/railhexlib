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
        private static Game instance;
        public static Game GetInstance(TileStack stack = null, ILogger logger = null)
        {
            if (instance == null)
            {
                instance = new Game();
            }
            return instance;
        }
        public static void Reset(TileStack stack = null, ILogger logger = null)
        {
            instance = null;
            instance = new Game(stack, logger);
        }
        public Tile CurrentTile => currentTile;
        public List<Structure> Structures => structures;
        public Dictionary<Cell, StructureRoad> StructureRoads => structureRoads;
        public Dictionary<Cell, HexNode> OrphanRoads = new Dictionary<Cell, HexNode>();

        public List<Trader> Traders => traders;
        public int ScorePoints => scorePoints;

        // Events
        public event EventHandler StructureAbandonEvent;
        public event EventHandler TraderArrivesToStructureEvent;

        private List<Type> availableTiles;
        public TileStack stack;
        private Random rnd;
        private Dictionary<Cell, Tile> placedTiles;
        private Tile currentTile;
        /// <summary>
        /// Key is center of structure
        /// Value is Structures placed in Cell and it's 
        /// </summary>
        private Dictionary<Cell, StructureRoad> structureRoads;

        private List<Structure> structures = new List<Structure>();
        /// <summary>
        /// Contains orphan roads where Key is root Node
        /// </summary>

        private List<Trader> traders = new List<Trader>();
        private ILogger logger;
        private int scorePoints;

        private Game(TileStack stack = null, ILogger logger = null)
        {
            this.logger = logger ?? new DefaultSilentLogger();
            placedTiles = new Dictionary<Cell, Tile>(new CellEqualityComparer());
            structureRoads = new Dictionary<Cell, StructureRoad>();

            availableTiles = new List<Type>(){
                typeof(ROAD_180Tile),
                typeof(ROAD_120Tile),
                // typeof(ROAD_60Tile),
                // typeof(GrassTile),
                //typeof(WaterTile),
                // new ROAD_3T_60_120Tile()
            };

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
            this.structures.AddRange(structures);

            foreach (var structure in structures)
            {

                this.structureRoads[structure.GetEnterCell()] = new StructureRoad(structure);
                foreach (var cell in structure.GetHexes())
                    placedTiles[cell.Key] = cell.Value;

                logger.Log("subscribe");
                structure.OnStructureAbandon += (object s, EventArgs e) =>
                {
                    Structure structure = s as Structure;
                    this.structureRoads.Remove(structure.GetEnterCell());
                    this.Traders.RemoveAll(t => t.TradePoints.ContainsKey(structure.GetEnterCell()));
                    logger.Log("Structure abandoned");

                    // race conditions about unsubscribe after the null check
                    var tmp_event = StructureAbandonEvent;
                    if (tmp_event != null)
                    {
                        tmp_event(s, e);
                    }
                };
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

            var placementResult = new PlacementResult(currentTile);

            Dictionary<Cell, Ground> joinedNeighbors = FindJoinedNeighbors(placedCell);
            placementResult.NewJoins = joinedNeighbors;
            // each of the objects is exactly in one of the graphs: orphan roads, strcture roads or tradeRoutes
            var joinedRoads = from t in joinedNeighbors where t.Value is Road select t.Key;
            // make tree of current Hex
            var placedHexNodeRoads = new HexNode(placedCell);

            HashSet<Cell> changedOrphanRoads = new HashSet<Cell>();
            HashSet<Cell> changedStructureRoads = new HashSet<Cell>();

            // no joins, make new orphan
            if (joinedRoads.Count() == 0 && currentTile.Sides.ContainsValue(Road.instance))
            {
                OrphanRoads[placedCell] = placedHexNodeRoads;
                placementResult.NewOrphanRoads.Add(OrphanRoads[placedCell]);
            }
            // check joins
            foreach (var joinedRoadCell in joinedRoads)
            {
                foreach (var (roadCell, road) in OrphanRoads)
                {
                    if (JoinWithExistRoads(placedHexNodeRoads, joinedRoadCell, road))
                    {
                        changedOrphanRoads.Add(roadCell);
                    }
                    // orphanRoads[placedHexNodeRoads.Cell] = placedHexNodeRoads;
                }
                placementResult.NewOrphanRoads.AddRange(changedOrphanRoads.Select(k => OrphanRoads[k]).ToList());

                var structureRoads = this.structureRoads.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.road);
                // TODO: should return list of merged structures to join them in tradeRoute
                foreach (var (roadCell, road) in structureRoads)
                {
                    if (JoinWithExistRoads(placedHexNodeRoads, joinedRoadCell, road))
                    {
                        changedStructureRoads.Add(roadCell);
                    }
                    // roads[placedHexNodeRoads.Cell] = placedHexNodeRoads;
                }


            }

            // hasChangedTradeRoute = AddToPlacedGraphs(placedHexNodeRoads, joinedRoadCell, tradeRoutes);
            placementResult.NewStructureRoads = changedStructureRoads.Select(k => structureRoads[k]).ToList();

            // promoted to structures. Remove from orphans
            if (changedOrphanRoads.Count >= 1 && changedStructureRoads.Count >= 1)
            {
                // clear old keys
                foreach (var k in changedOrphanRoads) OrphanRoads.Remove(k);
                // remove created merged orphan - now it's in a structureRoad
                OrphanRoads.Remove(placedHexNodeRoads.Cell);

            }
            if (changedStructureRoads.Count > 1)
            {

                var changedStructureRoads_ = changedStructureRoads.Select(cell => structureRoads[cell]);
                var roadsToJoin = new Dictionary<Cell, IEnumerable<StructureRoad>>()
                {
                    [placedHexNodeRoads.Cell] = changedStructureRoads_,
                };

                var newTraders = SpawnTraders(roadsToJoin);
                placementResult.NewTraders = newTraders;
                this.traders.AddRange(newTraders);

                // All structure roads from the changedStructureRoads become new trade route, so we can safely remove them
                foreach (var k in changedStructureRoads) structureRoads.Remove(k);
                structureRoads.Remove(placedHexNodeRoads.Cell);

            }


            // check structures
            // if tile added to (orphan) (or merge multiple of them)
            //  - add to structure road whole orphan with current tile as parent and drop from orphans
            // if tile added to multiple structures, merge them in trade road
            // BuildRoads(placedHexNodeRoads);

            placementResult.GameOver = !NextTile();
            return placementResult;
        }

        /// <summary>
        /// Try to emplace the placedHexNodeRoad in the targetRoad on the joineryCell position
        /// </summary>
        /// <param name="placedHexNodeRoad"></param>
        /// <param name="joineryCell"></param>
        /// <param name="targetRoad"></param>
        /// <returns></returns>
        private bool JoinWithExistRoads(HexNode placedHexNodeRoad, Cell joineryCell, HexNode targetRoad)
        {
            // joinery cell should be in one of all: orphan, road or trade route
            HexNode roadJoineryOwner = targetRoad.FindCell(joineryCell);
            if (roadJoineryOwner != null)
            {
                HexNode parentForPlacedNode = roadJoineryOwner.Add(placedHexNodeRoad);
                Debug.Assert(parentForPlacedNode.Cell.Equals(joineryCell), "possibly incosistence");
                return true;
            }
            return false;
        }

        public void RotateCurrentTile(int count = 1)
        {
            Debug.Assert(currentTile != null, "Current tile should exists. Forget to call NextTile()?");

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

        public void Tick(int ticks)
        {

            foreach (var t in traders)
            {
                t.Tick(ticks);
            }
            foreach (var (_, tile) in placedTiles)
            {
                tile.Tick(ticks);
            }
            foreach (var structure in Structures)
            {
                structure.Tick(ticks);
            }

        }

        private void TradeArrivesToAStructure(object r, Trader.PointReachedArgs e)
        {
            Trader route = (Trader)r;

            // TODO: move resources from trader to the settlement
            scorePoints += Config.ScorePoints.ForTradePointReached;

            for (int i = 0; i < Config.Stack.TilesForTradePointReached; i++)
            {
                PushTile(MakeRandomTileType());
            }
        }

        private List<Trader> SpawnTraders(IEnumerable<KeyValuePair<Cell, IEnumerable<StructureRoad>>> RoadsToJoin)
        {
            var result = new List<Trader>();
            foreach (var group in RoadsToJoin)
            {
                var joineryCell = group.Key;
                var pairs = Utils.MakePairs(group.Value.ToList());
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
                    Trader newRoute = new Trader(path, points);
                    newRoute.OnTraderArrivesToAStructure += this.TradeArrivesToAStructure;
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
                    IdentityCell currentTileSide = new IdentityCell(neighbor - placedCell);

                    var currentTileSideBiome = currentTile.GetSideBiome(currentTileSide);

                    // skip unjoinable tiles
                    if (!currentTileSideBiome.IsJoinable())
                    {
                        continue;
                    }

                    Tile neighborTile = placedTiles[neighbor];
                    var neighborSideBiome = neighborTile.GetSideBiome(currentTileSide.Inverted());


                    if (currentTileSideBiome == neighborSideBiome)
                    {
                        logger.Log($"tile {currentTile}({placedCell}) has join {currentTileSideBiome} on {currentTileSide}");
                        joinsOfPlacedCell[neighbor] = currentTileSideBiome;
                    }
                }
            }

            return joinsOfPlacedCell;
        }

        public bool CanPlaceCurrentTile(Cell cell)
        {
            bool exists = placedTiles.ContainsKey(cell);
            return !exists;
        }

        private Tile MakeRandomTileType()
        {
            var randomValue = availableTiles[rnd.Next(availableTiles.Count)];

            if (randomValue == typeof(ROAD_60Tile))
            {
                return new ROAD_60Tile();
            }
            else if (randomValue == typeof(ROAD_180Tile))
            {
                return new ROAD_180Tile();
            }
            else if (randomValue == typeof(GrassTile))
            {
                return new GrassTile();
            }
            else if (randomValue == typeof(WaterTile))
            {
                return new WaterTile();
            }
            else if (randomValue == typeof(ROAD_120Tile))
            {
                return new ROAD_120Tile();
            }
            else
            {
                throw new NotImplementedException("Make a normal factory method, bitch");
            }
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


    }
}
