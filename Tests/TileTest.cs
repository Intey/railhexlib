using NUnit.Framework;
using RailHexLib;
using RailHexLib.Grounds;

namespace RailHexLib.Tests
{

    [TestFixture]
    public class TileRotationTest
    {
        [Test]
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
