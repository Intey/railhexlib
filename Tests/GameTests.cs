using NUnit.Framework;
using RailHexLib;
// using System;
using System.Collections.Generic;
using System.Linq;

namespace RailHexLib.Tests
{
    [TestFixture]
    public class GameTests
    {
        const float CELL_SIZE = 1f;
        Game game; 
        TileStack stack = new();

        [SetUp]
        public void SetUp()
        {
            stack = new TileStack();
            stack.PushTile(new ROAD_120Tile());
            stack.PushTile(new ROAD_180Tile());
            
            Game.Reset(1.0f, stack);
            game = Game.GetInstance();
            Settlement settlement1 = new(new Cell(0, 0, CELL_SIZE), "settlement1");
            // settlement size is 3tiles on R, so from center we have 2 tiles.
            // -10 -9 ...  -3  -2  -1   0
            // S2  S2 ...  --      S1  S1
            Settlement settlement2 = new(new Cell(0, -10, CELL_SIZE), "settlement2");
            // rotate 180. Now it's enter points to settlement 1
            settlement2.Rotate60Clock(3);
            game.AddStructures(new List<Structure> {settlement1, settlement2});
        }

        [Test]
        public void TestPlaceOfStructureRoad()
        {
            game.NextTile();
            game.RotateCurrentTile();
            var result = game.PlaceCurrentTile(new Cell(0, -2, CELL_SIZE));
            Assert.IsTrue(result.NewJoins.ContainsKey(new Cell(0, -1, CELL_SIZE)));
            game.RotateCurrentTile();
            result = game.PlaceCurrentTile(new Cell(1, -3, CELL_SIZE));   
            Assert.IsTrue(result.NewJoins.ContainsKey(new Cell(0, -2, CELL_SIZE)), "Joins: " +string.Join(",", result.NewJoins.Keys));
            Assert.AreEqual(1, result.NewStructureRoads.Count);
            var list = (from c in result.NewStructureRoads[0].road select c.Cell).ToArray();
            Assert.AreEqual(new List<Cell>() {new(0, -1, CELL_SIZE), new(0, -2, CELL_SIZE), new (1, -3, CELL_SIZE)}, list);
        }
    }
}