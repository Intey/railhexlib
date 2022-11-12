using NUnit.Framework;
// using System;

namespace RailHexLib.Tests
{
    [TestFixture]
    public class ZonesTest
    {

        [Test]
        public void TestZonesCreation()
        {
            var stack = new TileStack();
            stack.PushTile(new WaterTile());
            stack.PushTile(new WaterTile());
            Game.Reset(stack);
            var game = Game.GetInstance();
            game.NextTile();
            var placeRes = game.PlaceCurrentTile(new Cell(2, 0, 1));
            
            Assert.AreEqual(1, game.Zones.Count);
            Zone zone = game.Zones[0];
            Assert.AreEqual(1, zone.ResourceCount);
        }
    }

}