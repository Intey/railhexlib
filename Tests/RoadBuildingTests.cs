using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using RailHexLib.Grounds;


namespace RailHexLib.Tests
{

    [TestFixture]
    public class RoadBuildingTests
    {

        /* 
         *           -           -
         *        -+    -     -     -
         *     -    \      -           -
         *     -     +----+-+----+     -
         *     -           -    /      -
         *        -     -    +-+    -
         *           -      /    -
         *           -     +     -
         *           -    /      -
         *        -    +-+    -  
         *     -     /     -
         *     -    +      -
         *     -     \     -
         *        -    +-
         *           -
         */
        string showTile(Tile t)
        {
            string result = "      -      ";
            // every line prepare symbol for each tile side depending on biome and row and on count of cell and their position...
            result +=       "   -+    -   ";
            return result;
        }

        string showSides(Dictionary<IdentityCell, Ground> sides)
        {
            string result = "";
            foreach (var (side, value) in sides)
            {
                result += $",{side}->{value}";
            }
            return result;
        }


        const float CELL_SIZE = 1;

        private string PathToString(List<Cell> list)
        {
            if (list == null) return "";
            string result = "";
            if (list.Count() < 2) return result;
            int j = 1;
            for (int i = 0; i < list.Count() - 1; i++)
            {
                result += "-";
                var dir = list[i].GetDirectionTo(list[j]);
                if (dir.Equals(IdentityCell.topLeftSide))
                {
                    result += "L";
                }
                else if (dir.Equals(IdentityCell.topSide))
                {
                    result += "UL";
                }
                else if (dir.Equals(IdentityCell.topRightSide))
                {
                    result += "UR";
                }
                else if (dir.Equals(IdentityCell.bottomRightSide))
                {
                    result += "R";
                }
                else if (dir.Equals(IdentityCell.bottomSide))
                {
                    result += "DR";
                }
                else if (dir.Equals(IdentityCell.bottomLeftSide))
                {
                    result += "DL";
                }
                j++;
            }
            return result;
        }

        public TileStack stack = new();
        readonly List<Structure> structures = new();
        readonly DevTools.Logger logger = new();
        private Game game;
        private readonly Cell settle1Position = new(0, 0, CELL_SIZE);
        private readonly Cell settle2Position = new(0, -4, CELL_SIZE);

        [SetUp]
        public void Prepare()
        {
            stack = new();
            Trace.Listeners.Add(new ConsoleTraceListener());
            Settlement settlement1 = new(settle1Position, "settlement1");
            // settlement size is 3tiles on R, so from center we have 2 tiles.
            // -4  -3  -2  -1   0
            // S2  S2  --  S1  S1
            Settlement settlement2 = new(settle2Position, "settlement2");
            // rotate 180. Now it's enter points to settlement 1
            settlement2.Rotate60Clock(3);
            structures.Clear();
            structures.Add(settlement1);
            structures.Add(settlement2);

            Game.Reset(stack, logger);
            game = Game.GetInstance();
        }

        [Test]
        public void TestRoutewith1Tile()
        {
            game.AddStructures(structures);
            stack.PushTile(new ROAD_180Tile());
            game.NextTile();

            var placedCell = new Cell(0, -2, CELL_SIZE);
            var placeResult = game.PlaceCurrentTile(placedCell);

            Assert.IsTrue(placeResult);
            Assert.IsTrue(placeResult.GameOver);
            var routes = game.Traders;
            var expectedJoins = new Dictionary<Cell, Grounds.Ground>()
            {
                [new Cell(0, -3, CELL_SIZE)] = Grounds.Ground.Road,
                [new Cell(0, -1, CELL_SIZE)] = Grounds.Ground.Road,
            };

            Assert.AreEqual(expectedJoins, placeResult.NewJoins, $"{placeResult.NewJoins.Keys.First()}");
            Assert.AreEqual(1, routes.Count());
            var r1 = routes[0];
            Assert.AreEqual(structures[0].Center, r1.TradePoints[structures[0].GetEnterCell()].Center);
            Assert.AreEqual(structures[1].Center, r1.TradePoints[structures[1].GetEnterCell()].Center);
            List<Cell> expectedRoad = new()
            {
                structures[1].GetEnterCell(),
                placedCell,
                structures[0].GetEnterCell(),
            };
            Assert.AreEqual(expectedRoad, r1.Cells);
        }
        [Test]
        public void TestLongRoute()
        {
            // replace one
            structures[1] = new Settlement(new Cell(0, -6, CELL_SIZE));
            structures[1].Rotate60Clock(3); // 180
            game.AddStructures(structures);
            // we need 3 tiles
            for (int i = 0; i < 3; i++)
                stack.PushTile(new ROAD_180Tile());
            // initial start
            Assert.AreEqual(3, game.stack.GetTiles().Count());
            Assert.IsTrue(game.NextTile());

            var isPlaced = game.PlaceCurrentTile(new Cell(0, -2, CELL_SIZE));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Traders.Count, 0);

            isPlaced = game.PlaceCurrentTile(new Cell(0, -3, CELL_SIZE));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Traders.Count, 0);

            isPlaced = game.PlaceCurrentTile(new Cell(0, -4, CELL_SIZE));
            Assert.IsTrue(isPlaced);
            Assert.IsTrue(isPlaced.GameOver);
            var expected = new Dictionary<Cell, Grounds.Ground>()
            {
                [new Cell(0, -3, CELL_SIZE)] = Grounds.Ground.Road,
                [new Cell(0, -5, CELL_SIZE)] = Grounds.Ground.Road,
            };
            Assert.AreEqual(expected, isPlaced.NewJoins);
            Assert.AreEqual(game.Traders.Count, 1);
            var r1 = game.Traders[0];
            Assert.AreEqual(structures[0].Center, r1.TradePoints[structures[0].GetEnterCell()].Center);
            Assert.AreEqual(structures[1].Center, r1.TradePoints[structures[1].GetEnterCell()].Center);
            List<Cell> expectedRoad = new()
            {
                new Cell(0, -5, CELL_SIZE),
                new Cell(0, -4, CELL_SIZE),
                new Cell(0, -3, CELL_SIZE),
                new Cell(0, -2, CELL_SIZE),
                new Cell(0, -1, CELL_SIZE),
            };
            Assert.AreEqual(r1.Cells, expectedRoad);
            Assert.AreEqual(2, game.Structures.Count);

        }
        [Test]
        public void TestLongRouteConcurrentBuild()
        {
            /*
             * /==S1==\            /==S0==\
             *  / \ / \ / \ / \ / \ / \ / \
             * | -6| -5| -4| -3| -2| -1|  0|
             *  \ / \ / \ / \ / \ / \ / \ / 
             * 
             */
            structures[1] = new Settlement(new Cell(0, -6, CELL_SIZE));
            structures[1].Rotate60Clock(3); // rotate 180

            game.AddStructures(structures);

            for (int i = 0; i < 3; i++)
                stack.PushTile(new ROAD_180Tile());

            game.NextTile();

            var isPlaced = game.PlaceCurrentTile(new Cell(0, -2, CELL_SIZE));
            Assert.IsTrue(isPlaced && !isPlaced.GameOver);
            Assert.AreEqual(game.Traders.Count, 0);

            isPlaced = game.PlaceCurrentTile(new Cell(0, -4, CELL_SIZE));
            Assert.IsTrue(isPlaced && !isPlaced.GameOver);
            Assert.AreEqual(game.Traders.Count, 0);

            isPlaced = game.PlaceCurrentTile(new Cell(0, -3, CELL_SIZE));
            Assert.IsTrue(isPlaced && isPlaced.GameOver, $"{isPlaced}, {isPlaced.GameOver}");
            Assert.AreEqual(game.Traders.Count, 1);

            var r1 = game.Traders[0];
            Assert.AreEqual(structures[0].Center, r1.TradePoints[structures[0].GetEnterCell()].Center);
            Assert.AreEqual(structures[1].Center, r1.TradePoints[structures[1].GetEnterCell()].Center);
            List<Cell> expectedRoad = new()
            {
                structures[1].GetEnterCell(), //0, -5
                new Cell(0, -4, CELL_SIZE),
                new Cell(0, -3, CELL_SIZE),
                new Cell(0, -2, CELL_SIZE),
                structures[0].GetEnterCell(),//0,-1
            };
            Assert.AreEqual(r1.Cells, expectedRoad);
            Assert.AreEqual(2, game.Structures.Count);
        }
        [Test]
        public void TestLongRouteBuildFromOrphan()
        {
            /*
             * /==S1==\  2   1   3 /==S0==\
             *  / \ / \ / \ / \ / \ / \ / \
             * | -6| -5| -4| -3| -2| -1|  0|
             *  \ / \ / \ / \ / \ / \ / \ / 
             * 
             */
            structures[1] = new Settlement(new Cell(0, -6, CELL_SIZE));
            structures[1].Rotate60Clock(3); // rotate 180
            game.AddStructures(structures);

            for (int i = 0; i < 3; i++)
                stack.PushTile(new ROAD_180Tile());

            game.NextTile();
            var isPlaced = game.PlaceCurrentTile(new Cell(0, -3, CELL_SIZE));
            Assert.IsTrue(isPlaced && !isPlaced.GameOver);
            Assert.AreEqual(game.Traders.Count, 0);
            Assert.AreEqual(1, isPlaced.NewOrphanRoads.Count);
            Assert.AreEqual(1, game.OrphanRoads.Count);

            isPlaced = game.PlaceCurrentTile(new Cell(0, -4, CELL_SIZE));
            Assert.IsTrue(isPlaced && !isPlaced.GameOver);
            Assert.AreEqual(game.Traders.Count, 0);
            Assert.AreEqual(1, isPlaced.NewStructureRoads.Count);
            Assert.IsNotNull(isPlaced.NewStructureRoads[0].road.FindCell(new Cell(0, -3, CELL_SIZE)), "should preserve orphan cell previosly added");

            // place center road
            isPlaced = game.PlaceCurrentTile(new Cell(0, -2, CELL_SIZE));
            Assert.IsTrue(isPlaced && isPlaced.GameOver);
            Assert.AreEqual(game.Traders.Count, 1);


            var r1 = game.Traders[0];
            Assert.AreEqual(structures[0].Center, r1.TradePoints[structures[0].GetEnterCell()].Center);
            Assert.AreEqual(structures[1].Center, r1.TradePoints[structures[1].GetEnterCell()].Center);
            List<Cell> expectedRoad = new()
            {
                structures[1].GetEnterCell(), //0, -5
                new Cell(0, -4, CELL_SIZE),
                new Cell(0, -3, CELL_SIZE),
                new Cell(0, -2, CELL_SIZE),
                structures[0].GetEnterCell(),//0,-1
            };
            Assert.AreEqual(r1.Cells, expectedRoad);
            Assert.AreEqual(2, game.Structures.Count);
        }

        [Test]
        public void TestRotatedTilesRoad()
        {
            /*
             * /==S1==\ r|r  |  /==S0==\
             *  / \ / \r/r\r/r\ / \ / \
             * | -5| -4|r-3| -2| -1|  0|
             *  \ / \ / \ / \ / \ / \ / 
             * 
             */
            structures[1] = new Settlement(new Cell(0, -5, CELL_SIZE));
            structures[1].Rotate60Clock(3); // rotate 180
            game.AddStructures(structures);
            game.PushTile(new ROAD_60Tile());
            game.PushTile(new ROAD_60Tile());
            game.PushTile(new ROAD_120Tile());
            game.PushTile(new ROAD_120Tile());
            game.NextTile();

            var result = game.PlaceCurrentTile(new Cell(0, -3, CELL_SIZE));
            Assert.AreEqual(1, result.NewStructureRoads.Count());

            game.RotateCurrentTile(4);
            game.PlaceCurrentTile(new Cell(-1, -3, CELL_SIZE));
            Assert.AreEqual(1, result.NewJoins.Count());
            Assert.AreEqual(1, result.NewStructureRoads.Count());

            game.RotateCurrentTile(4);
            game.PlaceCurrentTile(new Cell(-1, -2, CELL_SIZE));
            Assert.AreEqual(1, result.NewJoins.Count());
            Assert.AreEqual(1, result.NewStructureRoads.Count());

            game.RotateCurrentTile(1);
            result = game.PlaceCurrentTile(new Cell(0, -2, CELL_SIZE));

            
            Assert.IsTrue(result.GameOver);
            // there we can have non road joins, so check only the appropriate cells
            Assert.AreEqual(Ground.Road, result.NewJoins[new Cell(-1, -2, CELL_SIZE)]);
            Assert.AreEqual(Ground.Road, result.NewJoins[new Cell(0, -1, CELL_SIZE)]);
            Assert.AreEqual(1, result.NewTraders.Count());

        }
        [Test]
        public void Test4Rotation()
        {
            /*
             *    -1,0    -    -1,1
             *         -     -  
             *      -           -
             * 0,-1 -    0,0    -  0,1
             *      -           -
             *         -     -
             *   1,-1     -    1,0
             */

             /*
             *          1,-1
             *   0,-1 -  -  -  1,0
             *      -         -
             *    -     0,0     - 
             *      -         -
             *   -1,0  -  -  - 0,1
             *          -1,1
             */
            structures[1] = new Settlement(new Cell(1,-6, CELL_SIZE));
            structures[1].Rotate60Clock(3);
            // structures.RemoveAt(1);
            game.AddStructures(structures);

            game.PushTile(new ROAD_180Tile());
            game.PushTile(new ROAD_120Tile());
            game.PushTile(new ROAD_120Tile());
            game.NextTile();

            var result = game.PlaceCurrentTile(new Cell(0, -2, CELL_SIZE));

            Assert.IsTrue(result.isPlaced);
            Assert.AreEqual(1, result.NewStructureRoads.Count());

            game.RotateCurrentTile(1);
            result = game.PlaceCurrentTile(new Cell(0, -3, CELL_SIZE));
            Assert.IsTrue(result);
            Assert.AreEqual(1, result.NewStructureRoads.Count());

            Assert.AreEqual(Grounds.Ground.Road, result.PlacedTile.Sides[IdentityCell.topSide], message: showSides(result.PlacedTile.Sides));
            Assert.AreEqual(Grounds.Ground.Road, result.PlacedTile.Sides[IdentityCell.bottomRightSide]);
            game.RotateCurrentTile(4);
            result = game.PlaceCurrentTile(new Cell(1, -4, CELL_SIZE));
            Assert.IsTrue(result.isPlaced);
            Assert.AreEqual(Ground.Road, result.PlacedTile.Sides[IdentityCell.bottomSide], showSides(result.PlacedTile.Sides));
            Assert.AreEqual(1, result.NewJoins.Count(), showSides(result.PlacedTile.Sides));
            Assert.AreEqual(1, result.NewTraders.Count());


        }
    }
}