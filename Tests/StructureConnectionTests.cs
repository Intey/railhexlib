using NUnit.Framework;
// using System;
using RailHexLib.DevTools;

namespace RailHexLib.Tests
{
    [TestFixture]
    public class StructureConnectionTests
    {

        [Test]
        public void TestWaterTileConnected()
        {
            var stack = new TileStack();
            stack.PushTile(new WaterTile());
            stack.PushTile(new WaterTile());

            var settlement = new Settlement(new Cell(0, 0, 1));
            Game.Reset(stack, new Logger());
            var game = Game.GetInstance();
            game.AddStructures(new(){settlement});
            Assert.IsTrue(game.NextTile());
            var placeRes = game.PlaceCurrentTile(new Cell(2, 0, 1));
            var structure = game.Structures[0];
            Assert.AreEqual(1, structure.ConnectedZones.Count);
        }
    }

}