using Microsoft.VisualStudio.TestTools.UnitTesting;
using RailHexLibrary;
using System.Collections.Generic;

namespace RailHexLibrary.Test
{

    [TestClass()]
    public class CellsTest
    {
        [TestMethod()]
        public void TestCompare()
        {
            var c1 = new IdentityCell(0, 1);
            var c2 = new IdentityCell(0, 1);
            Assert.IsFalse(c1 == c2); // reference compare
            Assert.AreEqual(c1, c2); // require public override bool Equals(object obj)
            Assert.IsTrue(c1.Equals(c2));
            Assert.IsTrue(c2.Equals(c1));

            Dictionary<IdentityCell, int> testStruct = new Dictionary<IdentityCell, int>
            {
                [c1] = 0
            };
            Assert.IsNotNull(testStruct[c2]);
            Assert.AreEqual(testStruct[c2], 0);
        }
    }
}
