using RailHexLib.Grounds;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;
using RailHexLib.Interfaces;
using RailHexLib.DevTools;

namespace RailHexLib
{
    using NeedLevelList = List<Dictionary<Resource, (int, int)>>;
    enum Direction
    {
        QMAX,
        QMIN,
        RMAX,
        RMIN,
        SMAX,
        SMIN
    };
    public enum FeatureTypes
    {
        NewSettlementAppears
    }

    public class Game
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
        public List<Zone> Zones { get; set; } = new List<Zone>();
        // Events
        public event EventHandler StructureAbandonEvent;
        public event EventHandler TraderArrivesToStructureEvent;
        public event EventHandler NewStructureAppears;

        public Dictionary<FeatureTypes, bool> Features = new Dictionary<FeatureTypes, bool>();

        private List<Type> availableTiles;
        public TileStack stack;
        private Random rnd;
        private Dictionary<Cell, Tile> placedTiles;
        private Tile currentTile;
        /// <summary>
        /// Key is the enter cell of the structure because it's the begin of the road
        /// </summary>
        private Dictionary<Cell, StructureRoad> structureRoads;

        private List<Structure> structures = new List<Structure>();
        /// <summary>
        /// Contains orphan roads where Key is root Node
        /// </summary>

        private List<Trader> traders = new List<Trader>();
        private ILogger logger;
        private int scorePoints;

        private Timer newSettlementTimer = new Timer();
        private bool paused = false;
        private Game(TileStack stack = null, ILogger logger = null)
        {

            Features[FeatureTypes.NewSettlementAppears] = true;
            this.logger = logger ?? new DefaultSilentLogger();
            placedTiles = new Dictionary<Cell, Tile>(new CellEqualityComparer());
            structureRoads = new Dictionary<Cell, StructureRoad>();

            availableTiles = new List<Type>(){
                typeof(ROAD_180Tile),
                typeof(ROAD_120Tile),
                // typeof(ROAD_60Tile),
                typeof(GrassTile),
                typeof(WaterTile),
                typeof(ForestTile),
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
            newSettlementTimer.Timeout = Config.NewSettlement.TicksForNewSettlement;
            newSettlementTimer.OnTimeout += () =>
            {
                AddNewSettlement();
                AddNewSettlement();
            };
        }

        public void AddStructures(List<Structure> structures)
        {
            this.structures.AddRange(structures);

            foreach (var structure in structures)
            {

                this.structureRoads[structure.GetEnterCell()] = new StructureRoad(structure);
                foreach (var cell in structure.GetHexes())
                    placedTiles[cell.Key] = cell.Value;

                EventHandler handle = null;
                handle = (object s, EventArgs e) =>
                {
                    Structure structure = s as Structure;

                    this.structureRoads.Remove(structure.GetEnterCell());
                    this.Traders.RemoveAll(t => t.TradePoints.ContainsKey(structure.GetEnterCell()));
                    logger.Log("Structure abandoned");

                    // Call hanlers of the game event (like rethrow)
                    // race conditions about unsubscribe after the null check
                    var tmp_event = StructureAbandonEvent;
                    if (tmp_event != null)
                    {
                        tmp_event(s, e);
                    }
                    structure.OnStructureAbandon -= handle;
                };
                structure.OnStructureAbandon += handle;
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
            // if (stack.Empty()) {
            //     return PlacementResult.Fail;
            // }
            Debug.Assert(currentTile != null, "Current tile should exists. Forget to call NextTile() or maybe stack is empty");

            if (!CanPlaceCurrentTile(placedCell))
            {
                return PlacementResult.Fail;
            }
            logger.Log($"place tile {currentTile} on cell: {placedCell}. Rot: {currentTile.Rotation}");
            placedTiles[placedCell] = currentTile;

            var placementResult = new PlacementResult(currentTile);

            Dictionary<Cell, Ground> joinedNeighbors = FindJoinedNeighbors(placedCell);
            placementResult.NewJoins = joinedNeighbors;

            Dictionary<Ground, List<Cell>>
                            joinsByGround = joinedNeighbors
                                .GroupBy(b => b.Value)
                                .ToDictionary(pair => pair.Key, pair => pair.Select(k => k.Key).ToList());

            buildRoads(joinsByGround, placedCell, placementResult);

            // zones processing
            buildZones(joinsByGround, placedCell, placementResult);


            placementResult.GameOver = !NextTile();
            return placementResult;
        }
        private void buildZones(Dictionary<Ground, List<Cell>> joinsByGround, Cell placedCell, PlacementResult placementResult)
        {
            var joinableGround = Enum.GetValues(typeof(Ground)).OfType<Ground>().Where(g => g.IsJoinable() && g != Ground.Road);
            foreach (Ground groundType in joinableGround)
            {
                //logger.Log($"Process ground {groundType}");

                if (currentTile.HasBiome(groundType))
                {
                    if (joinsByGround.ContainsKey(groundType))
                    {
                        var joinsOfGround = joinsByGround[groundType];
                        var hasJoins = joinsOfGround.Count != 0;
                        // we are interested in zones that are of groundType 
                        // and contains one of joinedNeighbors
                        bool ZoneContainsJoinedCell(Zone z) => joinsOfGround.Any(c => z.Contains(c));

                        var zones = Zones
                            .Where(z => z.ZoneType == groundType && ZoneContainsJoinedCell(z));

                        // merge zones
                        var resourcesSumm = zones.Aggregate(0, (acc, z) =>
                        {
                            return acc + z.ResourceCount;
                        });
                        var mergedZone = new Zone(placedCell, resourcesSumm, groundType);
                        mergedZone.Extend(zones.Aggregate(
                            new List<Cell>(),
                            (acc, z) => { acc.AddRange(z.Cells); return acc; }
                        ));

                        Zones = Zones.Where(z => z.ZoneType != groundType).Append(mergedZone).ToList();
                    }
                    else
                    {
                        var newZone = new Zone(placedCell, Config.Zone.defaultResourceCount, groundType);
                        Zones.Add(newZone);
                        placementResult.NewZones.Add(newZone);
                        Structures.ForEach(s => s.ConnectZone(newZone));
                    }
                }
                else
                {
                    // logger.Log($"has no biome {groundType}");
                }
            }
        }

        private void buildRoads(Dictionary<Ground, List<Cell>> joinsByGround, Cell placedCell, PlacementResult placementResult)
        {
            var joinedRoads = joinsByGround.GetValueOrDefault(Grounds.Ground.Road, new List<Cell>());

            // prepare
            var placedHexNodeRoads = new HexNode(placedCell);

            HashSet<Cell> changedOrphanRoads = new HashSet<Cell>();
            HashSet<Cell> changedStructureRoads = new HashSet<Cell>();

            // no joins, make new orphan
            if (joinedRoads.Count() == 0)
            {
                OrphanRoads[placedCell] = placedHexNodeRoads;
                placementResult.NewOrphanRoads.Add(OrphanRoads[placedCell]);
            }

            // now add joins to exist roads or create orphans
            foreach (var joinedRoadCell in joinedRoads)
            {
                foreach (var (roadCell, road) in OrphanRoads)
                {
                    if (JoinWithExistRoads(placedHexNodeRoads, joinedRoadCell, road))
                    {
                        // collect cells of the exist orphan road on which we have the connection
                        changedOrphanRoads.Add(roadCell);
                    }
                }

                foreach (var (structureEnterCell, structRoad) in structureRoads)
                {
                    if (JoinWithExistRoads(placedHexNodeRoads, joinedRoadCell, structRoad.road))
                    {
                        // collect cells of the exist structure road on which we have the connection
                        changedStructureRoads.Add(structureEnterCell);
                    }
                }
            }

            placementResult.NewOrphanRoads.AddRange(changedOrphanRoads.Select(k => OrphanRoads[k]).ToList());
            placementResult.NewStructureRoads = changedStructureRoads.Select(k => structureRoads[k]).ToList();

            // if placed tile joins orphan and structure roads then we promote it to structures. 
            if (changedOrphanRoads.Count >= 1 && changedStructureRoads.Count >= 1)
            {
                // clear old keys
                foreach (var k in changedOrphanRoads) OrphanRoads.Remove(k);
                // remove created merged orphan - now it's in a structureRoad
                OrphanRoads.Remove(placedHexNodeRoads.Cell);
            }

            // if placed tile joins two or more structure roads then we create a trader between those structures
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
            if (paused) return;

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
            newSettlementTimer.Tick(ticks);

        }
        private void AddNewSettlement()
        {
            logger.Log($"Feature {FeatureTypes.NewSettlementAppears} state: {Features[FeatureTypes.NewSettlementAppears]}");
            if (!Features[FeatureTypes.NewSettlementAppears]) return;

            logger.Log("Add new settlement");
            var center = randomUnplacedCell();
            var needsList = new NeedLevelList()
            {
                //level 1
                (new(){
                    [Resource.Fish] = (2, 2),
                    [Resource.Wood] = (1, 4)
                }),
                // level 2
                (new(){
                    [Resource.Grass] = (4, 10),
                }),
            };

            var settlement = new Settlement(center, "Settlement", needsList);
            logger.Log($"Place new settlement in {center}");
            AddStructures(new() { settlement });
            var handler = NewStructureAppears;
            if (handler != null)
            {
                NewStructureAppears(settlement, EventArgs.Empty);
            }
        }

        // find random cell in area exclude filled convex hull of placed cells
        // return multiple cells distributed in specified area
        Cell randomUnplacedCell()
        {
            var placedCells = placedTiles.Keys;
            Cell maxQCell = placedCells.First();
            Cell maxRCell = placedCells.First();
            Cell maxSCell = placedCells.First();
            Cell minQCell = placedCells.First();
            Cell minRCell = placedCells.First();
            Cell minSCell = placedCells.First();

            foreach (var cell in placedCells)
            {
                if (cell.Q > maxQCell.Q)
                {
                    maxQCell = cell;
                }
                if (cell.R > maxRCell.R)
                {
                    maxRCell = cell;
                }
                if (cell.S > maxSCell.S)
                {
                    maxSCell = cell;
                }
                if (cell.Q < minQCell.Q)
                {
                    minQCell = cell;
                }
                if (cell.R < minRCell.R)
                {
                    minRCell = cell;
                }
                if (cell.S < minSCell.S)
                {
                    minSCell = cell;
                }
            }

            var directions = new List<(Direction, Cell, int)>(){
                (Direction.QMAX, maxQCell, maxQCell.Q), (Direction.QMIN, minQCell, minQCell.Q),
                (Direction.RMAX, maxRCell, maxRCell.R), (Direction.RMIN, minRCell, minRCell.R),
                (Direction.SMAX, maxSCell, maxSCell.S), (Direction.SMIN, minSCell, minSCell.S)
            }.OrderBy(x => Math.Abs(x.Item3)).ToList();


            // logger.Log("random new cell: has bounds \n" +
            // $"maxQ {maxQCell}, maxR {maxRCell}, maxS {maxSCell}\n" +
            // $"minQ {minQCell}, minR {minRCell}, minS {minSCell}");
            var rnd = new Random();
            double probability = rnd.NextDouble();
            bool makeFarestPosition = probability >= Config.NewSettlement.FarPositionProbability;
            if (!makeFarestPosition)
            {
                logger.Log("use nearest position");
                directions = directions.Take(directions.Count() / 2).ToList();
            }
            // 0 - MaxQ, 1 - MinQ, 2 - MaxR, 3 - MinR, 4 - MaxS, 5 - MinS
            var targetDirection = directions[rnd.Next(directions.Count())];

            switch (targetDirection.Item1)
            {
                case Direction.QMAX:
                    {
                        int qAxisPos = 0;
                        qAxisPos = targetDirection.Item3 + rnd.Next(Config.NewSettlement.minNearestCellOffset,
                          Config.NewSettlement.minNearestCellOffset + Config.NewSettlement.offsetBufferLinesCount);
                        int rAxisPos = -rnd.Next(Math.Abs(qAxisPos) + 1);
                        return new Cell(rAxisPos, qAxisPos);
                    }
                case Direction.QMIN:
                    {
                        int qAxisPos = 0;
                        // logger.Log("select direction on Q-");
                        qAxisPos = -Math.Abs(targetDirection.Item3 - rnd.Next(Config.NewSettlement.minNearestCellOffset,
                         Config.NewSettlement.minNearestCellOffset + Config.NewSettlement.offsetBufferLinesCount));
                        int rAxisPos = -rnd.Next(Math.Abs(qAxisPos) + 1);
                        return new Cell(rAxisPos, qAxisPos);
                        return targetDirection.Item2;
                    }
                case Direction.RMAX:
                    {
                        int rAxisPos = 0;
                        // logger.Log("select direction on R+");
                        rAxisPos = targetDirection.Item3 + rnd.Next(Config.NewSettlement.minNearestCellOffset,
                        Config.NewSettlement.minNearestCellOffset + Config.NewSettlement.offsetBufferLinesCount);
                        int qAxisPos = -rnd.Next(Math.Abs(rAxisPos) + 1);
                        return new Cell(rAxisPos, qAxisPos);
                    }
                case Direction.RMIN:
                    {
                        int rAxisPos = 0;
                        // logger.Log("select direction on R-");
                        rAxisPos = -Math.Abs(targetDirection.Item3 - rnd.Next(Config.NewSettlement.minNearestCellOffset,
                         Config.NewSettlement.minNearestCellOffset + Config.NewSettlement.offsetBufferLinesCount));
                        int qAxisPos = -rnd.Next(Math.Abs(rAxisPos) + 1);
                        return new Cell(rAxisPos, qAxisPos);
                    }
                case Direction.SMAX:
                    {
                        int sAxisPos = 0;
                        // logger.Log("select direction on S+");
                        sAxisPos = targetDirection.Item3 + rnd.Next(Config.NewSettlement.minNearestCellOffset,
                        Config.NewSettlement.minNearestCellOffset + Config.NewSettlement.offsetBufferLinesCount);
                        int rAxisPos = -rnd.Next(Math.Abs(sAxisPos) + 1);
                        int qAxisPos = -sAxisPos - rAxisPos;
                        return new Cell(rAxisPos, qAxisPos);
                    }
                case Direction.SMIN:
                    {
                        int sAxisPos = 0;
                        // logger.Log("select direction on S-");
                        sAxisPos = -Math.Abs(targetDirection.Item3 - rnd.Next(Config.NewSettlement.minNearestCellOffset,
                         Config.NewSettlement.minNearestCellOffset + Config.NewSettlement.offsetBufferLinesCount));
                        int rAxisPos = -rnd.Next(Math.Abs(sAxisPos) + 1);
                        int qAxisPos = -sAxisPos - rAxisPos;
                        return new Cell(rAxisPos, qAxisPos);
                    }
            }
            throw new Exception("unreachable random position");
        }

        private void TraderArrivesToStructure(object r, Trader.PointReachedArgs e)
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
                    var firstPath = pair[0].road.PathTo(joineryCell);
                    logger.Log($"first path {firstPath}");
                    firstPath = firstPath.Where(i => !i.Equals(joineryCell)).ToList();
                    path.AddRange(firstPath); // add income cell too
                    path.Add(joineryCell);
                    logger.Log($"SpawnTarders: Node {pair[1].road} - path to {joineryCell}");
                    foreach (var node in pair[1].road)
                    {
                        logger.Log($"SpawnTarders: Node item: {node}: L{node.TopLeft}, UL{node.Top}, UR{node.TopRight}, R{node.BottomRight}, DR{node.Bottom}, DL{node.BottomLeft}");
                    }
                    var pth = pair[1].road.PathTo(joineryCell);
                    logger.Log($"path: {pth}");
                    path.AddRange(pth.Where<Cell>(i => !i.Equals(joineryCell)).Reverse<Cell>());
                    Trader newRoute = new Trader(path, points);
                    newRoute.OnTraderArrivesToAStructure += this.TraderArrivesToStructure;
                    // ADD points to Route
                    result.Add(newRoute);
                }

            }
            return result;
        }

        /// <summary>
        /// find hexes with which newly placed tile has joins by a ground type
        /// </summary>
        /// <param name="placedCell"></param>
        /// <returns>Dictionary of neighbors that joined by Ground(in Value) with `placedCell`</returns>
        private Dictionary<Cell, Ground> FindJoinedNeighbors(Cell placedCell)
        {
            Dictionary<Cell, Ground> joinsOfPlacedCell = new Dictionary<Cell, Ground>();
            foreach (var (side, neighbor) in placedCell.Neighbours()) // structures.Boundaries()) // place struct: if "currentTile" will be the cell, then curentTile.Neighbours() may return complex boundary
            {
                if (placedTiles.ContainsKey(neighbor))
                {
                    IdentityCell currentTileSide = side;

                    var currentTileSideBiome = currentTile.GetSideBiome(currentTileSide);
                    // skip unjoinable tiles
                    if (!currentTileSideBiome.IsJoinable())
                    {
                        continue;
                    }

                    Tile neighborTile = placedTiles[neighbor];

                    var neighborSideBiome = neighborTile.GetSideBiome(currentTileSide.Inverted());
                    logger.Log($"JOIN CHECK: curr tile: {currentTileSide}[{currentTileSideBiome}] -- {currentTileSide.Inverted()}[{neighborSideBiome}]");
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
            else if (randomValue == typeof(ForestTile))
                return new ForestTile();
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
        public void InitializeLevel(int ln)
        {
            if (ln == 1)
            {
                var s1N = new List<Dictionary<Resource, (int, int)>>(){
                    (
                        new() {
                            [Resource.Fish] = (2,2)
                        }
                    ),
                    (
                        new() {
                            [Resource.Wood] = (4,4)
                        }
                    )
                };

                var s = new Settlement(new Cell(0, -10), "settlement1", s1N);
                s.Rotate60Clock(3); // 180

                var s2N = new List<Dictionary<Resource, (int, int)>>(){
                    (new(){[Resource.Fish] = (2, 2)})
                };

                var s2 = new Settlement(new Cell(0, 0), "Settlement2", s2N);

                var structs = new List<Structure>() { s2, s };
                AddStructures(structs);
            }
        }
        public void TogglePause()
        {
            paused = !paused;
        }
    }
}
