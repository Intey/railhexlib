using NUnit.Framework;
using System.Linq;

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
        [Test]
        public void TestMergeZones()
        {
            var stack = new TileStack();
            stack.PushTile(new WaterTile());
            stack.PushTile(new WaterTile());
            stack.PushTile(new WaterTile());
            stack.PushTile(new WaterTile());

            Game.Reset(stack);
            var game = Game.GetInstance();
            game.NextTile();
            
            var placeRes = game.PlaceCurrentTile(new Cell(0, 0, 1));
            placeRes = game.PlaceCurrentTile(new Cell(2, 0, 1));
            Assert.AreEqual(2, game.Zones.Count);
            
            var summ = game.Zones.Aggregate(0, (acc, z) => acc + z.ResourceCount);
            
            placeRes = game.PlaceCurrentTile(new Cell(1, 0, 1));
            Assert.AreEqual(1, placeRes.NewJoins.Count);
            Assert.AreEqual(1, game.Zones.Count);
            Assert.AreEqual(summ, game.Zones[0].ResourceCount);
        }
    }

}