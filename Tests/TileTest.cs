using NUnit.Framework;
using RailHexLib;
using RailHexLib.Grounds;
using System.Diagnostics;
namespace RailHexLib.Tests
{

    [TestFixture]
    public class TileRotationTest
    {
        [SetUp]
        public void setup()
        {
               Trace.Listeners.Add(new ConsoleTraceListener());
        }
        [Test]
        public void TestRotate30()
        {
            Tile t = new ROAD_60Tile();
            t.Rotate60Clock();
            Assert.AreEqual(t.Sides[IdentityCell.topLeftSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.topSide], Ground.Road);
            Assert.AreEqual(t.Sides[IdentityCell.topRightSide], Ground.Road);
            Assert.AreEqual(t.Sides[IdentityCell.bottomRightSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.bottomSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.bottomLeftSide], Ground.Ground);
        }

        [Test]
        public void Test240Rotation()
        {
         
            Tile t = new ROAD_120Tile();
            Assert.AreEqual(t.Sides[IdentityCell.topLeftSide], Ground.Road);
            Assert.AreEqual(t.Sides[IdentityCell.topSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.topRightSide], Ground.Road);
            Assert.AreEqual(t.Sides[IdentityCell.bottomRightSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.bottomSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.bottomLeftSide], Ground.Ground);

            t.Rotate60Clock();
            t.Rotate60Clock();
            t.Rotate60Clock();
            t.Rotate60Clock();
            foreach( var s in t.Sides)
                Debug.Print($"{s}");

            Assert.AreEqual(t.Sides[IdentityCell.topLeftSide], Ground.Road);
            Assert.AreEqual(t.Sides[IdentityCell.topSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.topRightSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.bottomRightSide], Ground.Ground);
            Assert.AreEqual(t.Sides[IdentityCell.bottomSide], Ground.Road);
            Assert.AreEqual(t.Sides[IdentityCell.bottomLeftSide], Ground.Ground);

        }
    }
}
