using NUnit.Framework;
using RailHexLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using RailHexLib.Grounds;

namespace RailHexLib.Tests
{

    using NeedLevelList = List<Dictionary<Resource, (int, int)>>;
    [TestFixture]
    public class StructureNeedsTests
    {
        [SetUp]
        public void Prepare()
        {
            //  Trace.Listeners.Add(new ConsoleTraceListener());
        }

        [Test]
        public void TestStructureConsumeNeeds()
        {
            var needs = new List<NeedsSystem.NeedsLevel>(){
            new NeedsSystem.NeedsLevel(
                    new NeedsSystem.Need(Resource.Fish, 6, 1)
                )
            };
            var settlement = new Settlement(new Cell(0, 0), "", needs);

            NeedsSystem.NeedsLevel needsLevel = settlement.NeedLevels[0];
            NeedsSystem.Need need = needsLevel.Needs.Find(x => x.Resource == Resource.Fish);
            Assert.NotNull(need);
            settlement.addResource(Resource.Fish, 10);
            settlement.Tick();
            Assert.IsFalse(settlement.Abandoned);
            Assert.AreEqual(4, settlement.Resources[Resource.Fish]);
            Assert.AreEqual(Config.Structure.InitialLife, settlement.LifeTime);
            Assert.AreEqual(6, need.FilledCount);

            settlement.Tick(Config.Structure.AbandonTimerTicks);
            Assert.IsFalse(settlement.Abandoned);
            Assert.AreEqual(0, settlement.Resources[Resource.Fish]);
            Assert.AreEqual(4, need.FilledCount);
            Assert.IsFalse(need.Filled);
            Assert.IsFalse(needsLevel.Filled);
            Assert.AreEqual(Config.Structure.InitialLife - 1, settlement.LifeTime);
        }
        [Test]
        public void TestStructureNeedsTimer()
        {
            var needs = new List<NeedsSystem.NeedsLevel>(){
                new NeedsSystem.NeedsLevel(
                    new NeedsSystem.Need(Resource.Fish, 6, 1),
                    new NeedsSystem.Need(Resource.Wood, 2, 2)
                )
            };
            var settlement = new Settlement(new Cell(0, 0), "", needs);

            NeedsSystem.NeedsLevel needsLevel = settlement.NeedLevels[0];

            settlement.addResource(Resource.Fish, 10);
            settlement.addResource(Resource.Wood, 10);
            settlement.Tick();
            Assert.IsFalse(settlement.Abandoned);
            Assert.AreEqual(4, settlement.Resources[Resource.Fish]);
            Assert.AreEqual(10, settlement.Resources[Resource.Wood]);
            settlement.Tick();
            Assert.IsFalse(settlement.Abandoned);
            Assert.AreEqual(0, settlement.Resources[Resource.Fish]);
            Assert.AreEqual(8, settlement.Resources[Resource.Wood]);
            settlement.Tick(2);
            Assert.AreEqual(6, settlement.Resources[Resource.Wood]);
        }
    }
}