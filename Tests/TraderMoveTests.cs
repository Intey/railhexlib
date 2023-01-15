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
            var cells = new List<Cell>() { new Cell(0, -3, CELL_SIZE), new Cell(0, -2, CELL_SIZE), new Cell(0, -1, CELL_SIZE) };
            var sampleData = new Dictionary<Cell, Structure>()
            {
                [structs[0].GetEnterCell()] = structs[0],
                [structs[1].GetEnterCell()] = structs[1]
            };
            route = new Trader(cells,
                sampleData);
        }
        private Trader route;

        [Test]
        public void TestMoveTrader()
        {
            Assert.AreEqual(new Cell(0, -3, CELL_SIZE), route.CurrentPosition);
            route.Tick(1);
            Assert.AreEqual(new Cell(0, -2, CELL_SIZE), route.CurrentPosition);
            route.Tick(1);
            Assert.AreEqual(new Cell(0, -1, CELL_SIZE), route.CurrentPosition);
        }
        [Test]
        public void TestMoveTraderReversing()
        {
            Assert.AreEqual(new Cell(0, -3, CELL_SIZE), route.CurrentPosition);
            route.Tick(3);
            Assert.AreEqual(new Cell(0, -2, CELL_SIZE), route.CurrentPosition);
        }

        [Test]
        public void TestTraderArrives()
        {
            var result = new List<Structure>();
            route.OnTraderArrivesToAStructure += (sender, args) =>
            {
                result.AddRange(args.ReachedStructures);
            };
            route.Tick(2);

            Assert.AreEqual(new List<Structure>() { route.TradePoints[new Cell(0, -1, CELL_SIZE)] }, result);
            
            result = new List<Structure>();
            route.Tick(6);
            Assert.AreEqual(
              new List<Structure>() { route.TradePoints[new Cell(0, -3, CELL_SIZE)]
                                    , route.TradePoints[new Cell(0, -1, CELL_SIZE)]
                                    , route.TradePoints[new Cell(0, -3, CELL_SIZE)]
                                    }
            , result);
        }
        [Test]
        public void TestTraderArrivesIncreaseLifeTime()
        {
             route.Tick(2);
             var expectedLife = 
             Config.Structure.InitialTicksToDie
             + Config.Structure.LifeTimeIncreaseOnTraderVisit;

             Assert.AreEqual(expectedLife, route.TradePoints[new Cell(0, -1, CELL_SIZE)].LifeTime);

        }

        [Test]
        public void TestTraderMovesResources()
        {
            // TODO
        }
    }
}