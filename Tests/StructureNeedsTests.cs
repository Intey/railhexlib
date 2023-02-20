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
            var needs = new List<Dictionary<Resource, int>>(){
                new(){ 
                    [Resource.Fish] = 6,
                }
            };
            var settlement = new Settlement(new Cell(0, 0), "", needs);
            
            NeedsSystem.NeedsLevel needsLevel = settlement.NeedLevels[0];
            NeedsSystem.Need need = needsLevel.Needs[Resource.Fish];

            settlement.addResource(Resource.Fish, 10);
            settlement.Tick();
            Assert.IsFalse(settlement.Abandoned);
            Assert.AreEqual(settlement.Resources[Resource.Fish], 4);
            Assert.AreEqual(Config.Structure.InitialTicksToDie, settlement.LifeTime);
            Assert.AreEqual(6, need.FilledCount);

            settlement.Tick();
            Assert.IsFalse(settlement.Abandoned);
            Assert.AreEqual(settlement.Resources[Resource.Fish], 0);
            Assert.AreEqual(4, need.FilledCount);
            Assert.IsFalse(need.Filled);
            Assert.IsFalse(needsLevel.Filled);
            Assert.AreEqual(Config.Structure.InitialTicksToDie-1, settlement.LifeTime);
        }
    }
}