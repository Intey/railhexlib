using NUnit.Framework;
using RailHexLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using RailHexLib.Grounds;

namespace RailHexLib.Test
{

    [TestFixture]
    public class PlacingStructuresTests
    {
        public TileStack stack = new();
        readonly List<Structure> structures = new();
        readonly DevTools.Logger logger = new();
        private Game game = new Game();
        private readonly Cell settle1Position = new(0, 0);
        private readonly Cell settle2Position = new(0, -4);

        [SetUp]
        public void Prepare()
        {
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

            game = new Game(stack, logger);
        }

        [Test]
        public void TestPlaceStructures()
        {
            game.PushTile(new GrassTile());
            game.AddStructures(structures);
            var structs = game.Structures.Select(kvp => kvp.Value).ToList();
            Assert.AreEqual(settle1Position, structures[0].Center);
            Assert.AreEqual(settle2Position, structures[1].Center);
            Assert.AreEqual(new Cell(0, -1), structures[0].GetEnterCell());
            Assert.AreEqual(new Cell(0, -3), structures[1].GetEnterCell());

            game.NextTile();

            foreach (var structure in structs)
            {
                foreach (var cell in structure.GetHexes())
                {
                    Assert.IsFalse(game.PlaceCurrentTile(cell.Key), $"structure tiles should be taken {cell.Key}({structure}");
                }
            }

            Assert.AreEqual(2, game.StructureRoads.Count);
            Assert.AreEqual(new Cell(0, -1), game.StructureRoads[new Cell(0, -1)].road.FindCell(new Cell(0, -1))?.Cell);
            Assert.AreEqual(new Cell(0, -3), game.StructureRoads[new Cell(0, -3)].road.FindCell(new Cell(0, -3))?.Cell);
        }
        [Test]
        public void TestRoutewith1Tile()
        {
            game.AddStructures(structures);
            stack.PushTile(new ROAD_180Tile());
            game.NextTile();

            var placedCell = new Cell(0, -2);
            var isPlaced = game.PlaceCurrentTile(placedCell);

            Assert.IsTrue(isPlaced);
            var routes = game.Routes;
            var expectedJoins = new Dictionary<Cell, Grounds.Ground>() {
                [new Cell(0, -3)] = Grounds.Road.instance,
                [new Cell(0, -1)] = Grounds.Road.instance,
            };

            Assert.AreEqual(expectedJoins, isPlaced.NewJoins);
            Assert.AreEqual(2, isPlaced.NewStructureRoads.Count, "should return 2 changed structure roads");
            Assert.AreEqual(1, routes.Count);
            var r1 = routes[0];
            Assert.AreEqual(structures[0].Center, r1.tradePoints[structures[0].GetEnterCell()].Center);
            Assert.AreEqual(structures[1].Center, r1.tradePoints[structures[1].GetEnterCell()].Center);
            List<Cell> expectedRoad = new()
            {
                structures[1].GetEnterCell(),
                placedCell,
                structures[0].GetEnterCell(),
            };
            Assert.AreEqual(expectedRoad, r1.cells);
        }
        [Test]
        public void TestLongRoute()
        {
            // replace one
            structures[1] = new Settlement(new Cell(0, -6));
            structures[1].Rotate60Clock(3); // 180

            game.AddStructures(structures);

            // we need 3 tiles
            for (int i = 0; i < 3; i++)
                stack.PushTile(new ROAD_180Tile());

            game.NextTile(); // initial start

            var isPlaced = game.PlaceCurrentTile(new Cell(0, -2));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Routes.Count, 0);

            isPlaced = game.PlaceCurrentTile(new Cell(0, -3));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Routes.Count, 0);

            isPlaced = game.PlaceCurrentTile(new Cell(0, -4));
            Assert.IsTrue(isPlaced && isPlaced.GameOver);
            Assert.AreEqual(game.Routes.Count, 1);
            var r1 = game.Routes[0];
            Assert.AreEqual(structures[0].Center, r1.tradePoints[structures[0].GetEnterCell()].Center);
            Assert.AreEqual(structures[1].Center, r1.tradePoints[structures[1].GetEnterCell()].Center);
            List<Cell> expectedRoad = new()
            {
                new Cell(0, -5),
                new Cell(0, -4),
                new Cell(0, -3),
                new Cell(0, -2),
                new Cell(0, -1),
            };
            Assert.AreEqual(r1.cells, expectedRoad);
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
            structures[1] = new Settlement(new Cell(0, -6));
            structures[1].Rotate60Clock(3); // rotate 180

            game.AddStructures(structures);

            for (int i = 0; i < 3; i++)
                stack.PushTile(new ROAD_180Tile());

            game.NextTile();

            var isPlaced = game.PlaceCurrentTile(new Cell(0, -2));
            Assert.IsTrue(isPlaced && !isPlaced.GameOver);
            Assert.AreEqual(game.Routes.Count, 0);
            
            isPlaced = game.PlaceCurrentTile(new Cell(0, -4));
            Assert.IsTrue(isPlaced && !isPlaced.GameOver);
            Assert.AreEqual(game.Routes.Count, 0);
            
            isPlaced = game.PlaceCurrentTile(new Cell(0, -3));
            Assert.IsTrue(isPlaced && isPlaced.GameOver);
            Assert.AreEqual(game.Routes.Count, 1);

            var r1 = game.Routes[0];
            Assert.AreEqual(structures[0].Center, r1.tradePoints[structures[0].GetEnterCell()].Center);
            Assert.AreEqual(structures[1].Center, r1.tradePoints[structures[1].GetEnterCell()].Center);
            List<Cell> expectedRoad = new()
            {
                structures[1].GetEnterCell(), //0, -5
                new Cell(0, -4),
                new Cell(0, -3),
                new Cell(0, -2),
                structures[0].GetEnterCell(),//0,-1
            };
            Assert.AreEqual(r1.cells, expectedRoad);
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
            structures[1] = new Settlement(new Cell(0, -6)); 
            structures[1].Rotate60Clock(3); // rotate 180
            game.AddStructures(structures);

            for (int i = 0; i < 3; i++)
                stack.PushTile(new ROAD_180Tile());
            
            game.NextTile();
            var isPlaced = game.PlaceCurrentTile(new Cell(0, -3));
            Assert.IsTrue(isPlaced && !isPlaced.GameOver);
            Assert.AreEqual(game.Routes.Count, 0);
            Assert.AreEqual(1, isPlaced.NewOrphanRoads.Count);
            
            isPlaced = game.PlaceCurrentTile(new Cell(0, -4));
            Assert.IsTrue(isPlaced && !isPlaced.GameOver);
            Assert.AreEqual(game.Routes.Count, 0);
            Assert.AreEqual(1, isPlaced.NewStructureRoads.Count);
            Assert.IsNotNull(isPlaced.NewStructureRoads[0].road.FindCell(new Cell(0, -3)), "should preserve orphan cell previosly added");

            // place center road
            isPlaced = game.PlaceCurrentTile(new Cell(0, -2));
            Assert.IsTrue(isPlaced && isPlaced.GameOver);
            Assert.AreEqual(game.Routes.Count, 1);

            
            var r1 = game.Routes[0];
            Assert.AreEqual(structures[0].Center, r1.tradePoints[structures[0].GetEnterCell()].Center);
            Assert.AreEqual(structures[1].Center, r1.tradePoints[structures[1].GetEnterCell()].Center);
            List<Cell> expectedRoad = new()
            {
                structures[1].GetEnterCell(), //0, -5
                new Cell(0, -4),
                new Cell(0, -3),
                new Cell(0, -2),
                structures[0].GetEnterCell(),//0,-1
            };
            Assert.AreEqual(r1.cells, expectedRoad);
        }
    }
}
