using NUnit.Framework;
using System.Collections.Generic;

namespace RailHexLib.Tests
{
    [TestFixture]
    class TraderMoveTests
    {
        const float CELL_SIZE = 1f;
        [SetUp]
        public void Prepare()
        {
            var s2 = new Settlement(new Cell(0, -4, CELL_SIZE), "four");
            s2.Rotate60Clock(3); // 180
            var structs = new List<Structure>() {
            new Settlement(new Cell(0,0, CELL_SIZE), "zero"),
            s2
        };

            structs[0].addResource(Resource.Fish, 10);
            structs[1].addResource(Resource.Fish, 10);

            var cells = new List<Cell>() {
            new Cell(0, -3, CELL_SIZE),
            new Cell(0, -2, CELL_SIZE),
            new Cell(0, -1, CELL_SIZE)
        };
            var tradePoints = new Dictionary<Cell, Structure>()
            {
                [structs[0].GetEnterCell()] = structs[0],
                [structs[1].GetEnterCell()] = structs[1]
            };
            trader = new Trader(cells, tradePoints);
        }
        private Trader trader;

        [Test]
        public void TestMoveTrader()
        {
            Assert.AreEqual(new Cell(0, -3, CELL_SIZE), trader.CurrentPosition);
            trader.Tick(1);
            Assert.AreEqual(new Cell(0, -2, CELL_SIZE), trader.CurrentPosition);
            trader.Tick(1);
            Assert.AreEqual(new Cell(0, -1, CELL_SIZE), trader.CurrentPosition);
        }
        [Test]
        public void TestMoveTraderReversing()
        {
            Assert.AreEqual(new Cell(0, -3, CELL_SIZE), trader.CurrentPosition);
            trader.Tick(3);
            Assert.AreEqual(new Cell(0, -2, CELL_SIZE), trader.CurrentPosition);
        }

        [Test]
        public void TestTraderArrives()
        {
            var result = new List<Structure>();
            trader.OnTraderArrivesToAStructure += (sender, args) =>
            {
                result.Add(args.ReachedStructure);
            };
            trader.Tick(2);

            Assert.AreEqual(new List<Structure>() { trader.TradePoints[new Cell(0, -1, CELL_SIZE)] }, result);

            result = new List<Structure>();
            trader.Tick(6);
            Assert.AreEqual(
                new List<Structure>() { trader.TradePoints[new Cell(0, -3, CELL_SIZE)]
                                , trader.TradePoints[new Cell(0, -1, CELL_SIZE)]
                                , trader.TradePoints[new Cell(0, -3, CELL_SIZE)]
                                    }
            , result);
        }
        [Test]
        public void TestTraderArrivesIncreaseLifeTime()
        {
            trader.Tick(2);
            var expectedLife =
            Config.Structure.InitialTicksToDie
            + Config.Structure.LifeTimeIncreaseOnTraderVisit;

            Assert.AreEqual(expectedLife, trader.TradePoints[new Cell(0, -1, CELL_SIZE)].LifeTime);

        }

        [Test]
        public void TestTraderMovesResources()
        {
            int count = 0;
            trader.OnTraderArrivesToAStructure += (sender, args) =>
            {
                count = args.ReachedStructure.Resources[Resource.Fish];
            };

            var traderShouldPick = (int)(10 * Config.Trader.consumptionPercent);
            var restInSettlement = 10 - traderShouldPick;
            trader.Tick();
            trader.Tick();
            var resources = trader.TradePoints[new Cell(0, -1)].Resources;
            Assert.AreEqual(
                restInSettlement,
                count
            );
            var traderHasRosourceCount = trader.Inventory.Resources[Resource.Fish];
            Assert.AreEqual(
                    traderShouldPick,
                    traderHasRosourceCount
            );
            
            traderShouldPick = 2;
            trader.Tick();
            trader.Tick();
            traderHasRosourceCount = trader.Inventory.Resources[Resource.Fish];
            Assert.AreEqual(
                Config.Trader.maxResourceCountInInventory,
                traderHasRosourceCount
            );

            restInSettlement = 10 - traderShouldPick;
            resources = trader.TradePoints[new Cell(0, -3)].Resources;
            Assert.AreEqual(
                    restInSettlement,
                    resources[Resource.Fish]
            );
            // there we meet second settlement
        }
    }
}