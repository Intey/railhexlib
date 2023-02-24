using NUnit.Framework;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using RailHexLib.Grounds;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

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

            Assert.AreEqual(1, game.Zones.Count, "expect game add new zone to its list");
            Zone zone = game.Zones[0];
            Assert.AreEqual(Config.Zone.defaultResourceCount, zone.ResourceCount, "zone has default resource count");
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
            var cell00 = new Cell(0, 0);
            var placeRes = game.PlaceCurrentTile(cell00);
            Assert.IsTrue(game.Zones[0].Cells.Contains(cell00));
            var cell20 = new Cell(2, 0);
            placeRes = game.PlaceCurrentTile(cell20);
            Assert.AreEqual(2, game.Zones.Count);
            Assert.IsTrue(game.Zones[1].Cells.Contains(cell20));

            var summ = game.Zones.Aggregate(0, (acc, z) => acc + z.ResourceCount);

            placeRes = game.PlaceCurrentTile(new Cell(1, 0));
            Assert.AreEqual(2, placeRes.NewJoins.Count);
            Assert.AreEqual(1, game.Zones.Count);
            Assert.AreEqual(new List<Cell>(){
                new Cell(1, 0),
                new Cell(0, 0),
                new Cell(2, 0)
            },
            game.Zones[0].Cells);

            Assert.AreEqual(summ, game.Zones[0].ResourceCount);
        }
        [Test]
        public void TestZoneConnection()
        {
            var game = Game.GetInstance();
            game.stack.PushTile(new WaterTile());
            game.NextTile();

            var settlement = new Settlement(new Cell(0, 0));
            game.AddStructures(new List<Structure>(){
                settlement
            });

            var topOfSettlementCell = settlement.Center + IdentityCell.topSide + IdentityCell.topSide;
            var res = game.PlaceCurrentTile(topOfSettlementCell);
            Assert.AreEqual(1, res.NewZones.Count);
            Assert.AreEqual(1, settlement.ConnectedZones.Count);
        }
        [Test]
        public void TestZoneConsumptionOut()
        {
            var game = Game.GetInstance();
            game.stack.PushTile(new WaterTile());
            game.NextTile();

            var settlement = new Settlement(new Cell(0, 0));
            game.AddStructures(new List<Structure>(){
                settlement
            });
            var resCount = 10;
            var topOfSettlementCell = settlement.Center + IdentityCell.topSide + IdentityCell.topSide;
            var zone = new Zone(topOfSettlementCell, resCount, Ground.Water);
            Assert.AreEqual(1, topOfSettlementCell.DistanceTo(settlement.Center + IdentityCell.topSide));
            settlement.ConnectZone(zone);
            // settlement.setConsumptionSpeed(consumptionSpeed);
            game.Tick(1);
            // TODO: Check that connection will be removed when zone dissapears
            Assert.AreEqual(resCount - Config.Structure.ZoneConsumptionCount, zone.ResourceCount, "resources not consumed");
            Assert.AreEqual(Config.Structure.ZoneConsumptionCount, settlement.Resources[Resource.Fish], "consumed resources should be in the settlement");
            game.Tick(1);
            Assert.AreEqual(0, zone.ResourceCount, "resources not consumed");
            Assert.AreEqual(0, settlement.ConnectedZones.Count, "conntection to empty zone should be broken");
            Assert.AreEqual(resCount, settlement.Resources[Resource.Fish], "consumed resources should be in the settlement");
        }
    }

}