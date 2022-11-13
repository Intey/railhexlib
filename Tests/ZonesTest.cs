using NUnit.Framework;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace RailHexLib.Tests
{
    [TestFixture]
    public class ZonesTest
    {

        [SetUp]
        public void SetUp()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            var stack = new TileStack();
            Game.Reset(stack, new DevTools.Logger());
            
        }

        [Test]
        public void TestZonesCreation()
        {
            var game = Game.GetInstance();

            game.stack.PushTile(new WaterTile());
            game.stack.PushTile(new WaterTile());

            game.NextTile();
            var placeRes = game.PlaceCurrentTile(new Cell(2, 0, 1));

            Assert.AreEqual(1, game.Zones.Count);
            Zone zone = game.Zones[0];
            Assert.AreEqual(1, zone.ResourceCount);
            Assert.AreEqual(new Cell(2, 0, 1), zone.Cells[0]);
        }
        [Test]
        public void TestMergeZones()
        {
            /*   /0\ /0\ /0\ / \ / \ / \ / \
                | 0 | 1 | 2 |   |   |   |   |
                 \ / \ / \ / \ / \ / \ / \ / 

            */

            var game = Game.GetInstance();
            game.stack.PushTile(new WaterTile());
            game.stack.PushTile(new WaterTile());
            game.stack.PushTile(new WaterTile());
            game.stack.PushTile(new WaterTile());

            game.NextTile();
            var cell00 = new Cell(0, 0, 1);
            var placeRes = game.PlaceCurrentTile(cell00);
            Assert.IsTrue(game.Zones[0].Cells.Contains(cell00));
            var cell20 = new Cell(2, 0, 1);
            placeRes = game.PlaceCurrentTile(cell20);
            Assert.AreEqual(2, game.Zones.Count);
            Assert.IsTrue(game.Zones[1].Cells.Contains(cell20));

            var summ = game.Zones.Aggregate(0, (acc, z) => acc + z.ResourceCount);

            placeRes = game.PlaceCurrentTile(new Cell(1, 0, 1));
            Assert.AreEqual(2, placeRes.NewJoins.Count);
            Assert.AreEqual(1, game.Zones.Count);
            Assert.AreEqual(new List<Cell>(){ 
                new Cell(1, 0, 1),
                new Cell(0, 0, 1), 
                new Cell(2, 0, 1)
            },
            game.Zones[0].Cells);
            Assert.AreEqual(summ, game.Zones[0].ResourceCount);
        }
    }

}