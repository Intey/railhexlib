using NUnit.Framework;
using RailHexLib;
using System.Collections.Generic;

namespace RailHexLib.Test
{

    [TestFixture]
    public class CellsTest
    {
        [Test]
        public void TestCompare()
        {
            var c1 = new IdentityCell(0, 1);
            var c2 = new IdentityCell(0, 1);
            Assert.IsFalse(c1 == c2); // reference compare
            Assert.AreEqual(c1, c2); // require public override bool Equals(object obj)
            Assert.IsTrue(c1.Equals(c2));
            Assert.IsTrue(c2.Equals(c1));

            Dictionary<IdentityCell, int> testStruct = new()
            {
                [c1] = 0
            };
            Assert.IsNotNull(testStruct[c2]);
            Assert.AreEqual(testStruct[c2], 0);
        }
        [Test]
        public void TestDistance()
        {
            Assert.AreEqual(1, new Cell(0, 0).DistanceTo(new Cell(0, -1)));
            Assert.AreEqual(2, new Cell(0, 0).DistanceTo(new Cell(1, -2)));
            Assert.AreEqual(2, new Cell(0, 0).DistanceTo(new Cell(-1, -1)));
        }
    }
}
