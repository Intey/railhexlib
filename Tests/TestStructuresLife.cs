using NUnit.Framework;
using RailHexLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using RailHexLib.Grounds;

namespace RailHexLib.Tests
{

    [TestFixture]
    public class TestStructuresLife
    {
        const float CELL_SIZE = 1f;
        public TileStack stack = new();
        Structure settlement;
        readonly DevTools.Logger logger = new();
        private Game game = Game.GetInstance();
        private readonly Cell settle1Position = new(0, 0, CELL_SIZE);
        
        [SetUp]
        public void Prepare()
        {
          //  Trace.Listeners.Add(new ConsoleTraceListener());
            settlement = new Settlement(settle1Position, "settlement1");
            Game.Reset(stack, logger);
            game = Game.GetInstance();
        }

        [Test]
        public void TestStructureWillDie()
        {
            var s = new Settlement(new Cell(0, 0, CELL_SIZE), "Unknown");
            s.Tick(4);
            Assert.AreEqual(Config.Structure.InitialTicksToDie - 4, s.LifeTime);
        }
        [Test]
        public void TestAbandonEventRemoveStructureRoad()
        {
            game.AddStructures(new List<Structure>(){settlement});
            game.Tick(Config.Structure.InitialTicksToDie);
            Assert.AreEqual(0, game.StructureRoads.Count);
        }

        [Test]
        public void TestAbandonEventRemoveTrader()
        {
            game.AddStructures(new List<Structure>(){settlement});
            game.Tick(Config.Structure.InitialTicksToDie);
            Assert.AreEqual(0, game.Traders.Count);
        }
    }
}
