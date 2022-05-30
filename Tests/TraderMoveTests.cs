using NUnit.Framework;
using System.Collections.Generic;

namespace RailHexLib.Tests
{
    [TestFixture]
    class TraderMoveTests
    {
        [SetUp]
        public void Prepare()
        {
            var s2 = new Settlement(new Cell(0, -4), "four");
            s2.Rotate60Clock(3); // 180
            var structs = new List<Structure>() {
                new Settlement(new Cell(0,0), "zero"),
                s2
            };
            var cells = new List<Cell>() { new Cell(0, -3), new Cell(0, -2), new Cell(0, -1) };
            route = new TradeRoute(cells,
            new Dictionary<Cell, Structure>() { 
                [structs[0].GetEnterCell()] = structs[0], 
                [structs[1].GetEnterCell()] = structs[1] }, 
                () => { }
            );
        }
        private TradeRoute route;

        [Test]
        public void TestMoveTrader()
        {
            Assert.AreEqual(new Cell(0, -3), route.CurrentTraderPosition);
            route.Update(1);
            Assert.AreEqual(new Cell(0, -2), route.CurrentTraderPosition);
            route.Update(1);
            Assert.AreEqual(new Cell(0, -1), route.CurrentTraderPosition);
        }
        [Test]
        public void TestMoveTraderReversing()
        {
            Assert.AreEqual(new Cell(0, -3), route.CurrentTraderPosition);
            route.Update(3);
            Assert.AreEqual(new Cell(0, -2), route.CurrentTraderPosition);
        }
    }
}