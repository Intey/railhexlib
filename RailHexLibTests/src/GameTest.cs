using Microsoft.VisualStudio.TestTools.UnitTesting;
using RailHexLibrary;

using System;
using System.Collections.Generic;
using System.Linq;

namespace RailHexLibrary.Test
{

    [TestClass()]
    public class GameTest
    {
        public TileStack stack = new TileStack();
        readonly List<Structure> structures = new List<Structure>();
        Game game;

        public void Prepare()
        {
            Settlement settlement1 = new Settlement(new Cell(0, 0));
            // settlement size is 3tiles on R, so from center we have 2 tiles.
            // -4  -3  -2  -1   0
            // S2  S2  --  S1  S1
            Settlement settlement2 = new Settlement(new Cell(0, -4));
            // rotate 180. Now it's enter points to settlement 1
            settlement2.Rotate60Clock();
            settlement2.Rotate60Clock();
            settlement2.Rotate60Clock();

            structures.Add(settlement1);
            structures.Add(settlement2);

            game = new Game(stack);

        }

        [TestMethod()]
        public void TestPlaceStructures()
        {
            Prepare();
            game.AddStructures(structures);
            List<Tuple<Cell, Structure>> structs = game.Structures.Select(kvp => new Tuple<Cell, Structure>(kvp.Key, kvp.Value)).ToList();
            Assert.AreEqual(structs[0].Item1, structures[0].Center);
            Assert.AreEqual(structs[0].Item2, structures[0]);
            Assert.AreEqual(structs[1].Item1, structures[1].Center);
            Assert.AreEqual(structs[1].Item2, structures[1]);

        }
        [TestMethod()]
        public void TestRoutewith1Tile()
        {
            Prepare();
            game.AddStructures(structures);
            stack.AddTile(new ROAD_180Tile());
            game.NextTile();

            var isPlaced = game.PlaceCurrentTile(new Cell(0, -2));

            Assert.IsTrue(isPlaced);
            var routes = game.Routes;
            Assert.AreEqual(routes.Count, 1);
            var r1 = routes[0];
            Assert.AreEqual(r1.tradePoints[new Cell(0, 0)], structures[0]);
            Assert.AreEqual(r1.tradePoints[new Cell(0, -4)], structures[1]);
            List<Cell> expectedRoad = new List<Cell>()
            {
                new Cell(0, 0),
                new Cell(0, -1),
                new Cell(0, -2),
                new Cell(0, -3),
                new Cell(0, -4)
            };
            Assert.AreEqual(r1.cells, expectedRoad);
        }
        [TestMethod()]
        public void TestLongRoute()
        {
            Prepare();
            structures[1] = new Settlement(new Cell(0, -6));
            game.AddStructures(structures);

            for (int i = 0; i < 3; i++)
                stack.AddTile(new ROAD_180Tile());

            game.NextTile();
            var isPlaced = game.PlaceCurrentTile(new Cell(0, -2));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Routes.Count, 0);
            game.NextTile();
            isPlaced = game.PlaceCurrentTile(new Cell(0, -3));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Routes.Count, 0);
            game.NextTile();
            isPlaced = game.PlaceCurrentTile(new Cell(0, -4));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Routes.Count, 1);
            var r1 = game.Routes[0];
            Assert.AreEqual(r1.tradePoints[new Cell(0, 0)], structures[0]);
            Assert.AreEqual(r1.tradePoints[new Cell(0, -6)], structures[1]);
            List<Cell> expectedRoad = new List<Cell>()
            {
                new Cell(0, 0),
                new Cell(0, -1),
                new Cell(0, -2),
                new Cell(0, -3),
                new Cell(0, -4),
                new Cell(0, -5),
                new Cell(0, -6),
            };
            Assert.AreEqual(r1.cells, expectedRoad);
        }
        [TestMethod()]
        public void TestLongRouteConcurrentBuild()
        {
            Prepare();

            structures[1] = new Settlement(new Cell(0, -6));
            game.AddStructures(structures);

            for (int i = 0; i < 3; i++)
                stack.AddTile(new ROAD_180Tile());

            game.NextTile();
            var isPlaced = game.PlaceCurrentTile(new Cell(0, -3));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Routes.Count, 0);
            game.NextTile();
            isPlaced = game.PlaceCurrentTile(new Cell(0, -4));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Routes.Count, 0);
            game.NextTile();
            
            // place center road
            isPlaced = game.PlaceCurrentTile(new Cell(0, -2));
            Assert.IsTrue(isPlaced);
            Assert.AreEqual(game.Routes.Count, 1);
            var r1 = game.Routes[0];
            Assert.AreEqual(r1.tradePoints[new Cell(0, 0)], structures[0]);
            Assert.AreEqual(r1.tradePoints[new Cell( 0, -6)], structures[1]);
            List<Cell> expectedRoad = new List<Cell>()
            {
                new Cell(0, 0),
                new Cell(0, -1),
                new Cell(0, -2),
                new Cell(0, -3),
                new Cell(0, -4),
                new Cell(0, -5),
                new Cell(0, -6),
            };
            Assert.AreEqual(r1.cells, expectedRoad);
        }
    }
}
