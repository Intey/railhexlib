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
        readonly DevTools.Logger logger = new("structure-life-test");
        private Game game = Game.GetInstance();
        private readonly Cell settle1Position = new(0, 0, CELL_SIZE);

        [SetUp]
        public void Prepare()
        {

            Trace.Listeners.Add(new ConsoleTraceListener());
            settlement = new Settlement(settle1Position, "settlement1",
                new List<NeedsSystem.NeedsLevel>(){
                    new NeedsSystem.NeedsLevel(
                        new NeedsSystem.Need(Resource.Fish, 1, 1)
                    )
                }
            );

            Game.Reset(1.0f, stack, logger);
            game = Game.GetInstance();
            game.Features[FeatureTypes.NewSettlementAppears] = false;
        }

        [Test]
        public void TestStructureWillDie()
        {
            settlement.Tick(4 * Config.Structure.AbandonTimerTicks);
            Assert.AreEqual(Config.Structure.InitialLife - 4, settlement.LifeTime);
        }
        [Test]
        public void TestAbandonEventRemoveStructureRoad()
        {
            game.AddStructures(new List<Structure>() { settlement });
            game.Tick(Config.Structure.InitialLife * Config.Structure.AbandonTimerTicks);
            Assert.IsTrue(settlement.Abandoned, $"settlement lifetime {settlement.LifeTime} should be zero");
            Assert.AreEqual(0, game.StructureRoads.Count, $"Structure road should be removed. Settlement lifetime: {settlement.LifeTime}");
        }

        [Test]
        public void TestAbandonEventRemoveTrader()
        {

            game.AddStructures(new List<Structure>() { settlement });
            game.Tick(Config.Structure.InitialLife * Config.Structure.AbandonTimerTicks);
            Assert.AreEqual(0, game.Traders.Count);
        }
    }
}
