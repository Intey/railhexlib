using Microsoft.VisualStudio.TestTools.UnitTesting;
using RailHexLibrary;
using RailHexLibrary.Grounds;

namespace RailHexLibrary.Test
{

    [TestClass()]
    public class TileRotationTest
    {
        [TestMethod()]
        public void TestRotate30()
        {
            Tile t = new ROAD_60Tile();
            t.Rotate60Clock();
            Assert.AreEqual(t.Sides[IdentityCell.leftSide], Grass.instance);
            Assert.AreEqual(t.Sides[IdentityCell.upLeftSide], Road.instance);
            Assert.AreEqual(t.Sides[IdentityCell.upRightSide], Road.instance);
            Assert.AreEqual(t.Sides[IdentityCell.rightSide], Grass.instance);
            Assert.AreEqual(t.Sides[IdentityCell.downRightSide], Grass.instance);
            Assert.AreEqual(t.Sides[IdentityCell.downLeftSide], Grass.instance);
        }
    }
}
