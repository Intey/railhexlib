using NUnit.Framework;
using RailHexLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using RailHexLib.Grounds;

namespace RailHexLib.Tests
{
    using NeedsList = List<(Dictionary<Resource, int>, int)>;
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
            settlement = new Settlement(settle1Position, "settlement1", new(){
                (new(){[Resource.Fish] = 100000}, 1)
            });

            Game.Reset(stack, logger);
            game = Game.GetInstance();
        }

        [Test]
        public void TestStructureWillDie()
        {
            settlement.Tick(4);
            Assert.AreEqual(Config.Structure.InitialTicksToDie - 4, settlement.LifeTime);
        }
        [Test]
        public void TestAbandonEventRemoveStructureRoad()
        {
            game.AddStructures(new List<Structure>() { settlement });
            game.Tick(Config.Structure.InitialTicksToDie);
            Assert.AreEqual(0, game.StructureRoads.Count, $"settlement life:{settlement.LifeTime}");
        }

        [Test]
        public void TestAbandonEventRemoveTrader()
        {
            game.AddStructures(new List<Structure>() { settlement });
            game.Tick(Config.Structure.InitialTicksToDie);
            Assert.AreEqual(0, game.Traders.Count);
        }
    }
}
