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
            Assert.AreEqual(t.Sides[IdentityCell.leftSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.upLeftSide], Ground.Road);
            Assert.AreEqual(t.Sides[IdentityCell.upRightSide], Ground.Road);
            Assert.AreEqual(t.Sides[IdentityCell.rightSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.downRightSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.downLeftSide], Ground.Ground);
        }
    }
}
