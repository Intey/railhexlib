using NUnit.Framework;
using RailHexLib;
using System.Collections.Generic;

namespace RailHexLib.Tests
{
    /**
      * Grids
      * 
      * ^ R Axis    \ Q Axis (Diagonal left-up to right-down)
      * |            v
      * ======================================================
      *  /R\
      * | Q |
      *  \S/
      * ======================================================
      *    /1\ /1\
      *   | - | 0 |
      *  /0\ /0\ /0\
      * | - | 0 | 1 |
      *  \ /-\ /-\ /
      *   | 0 | 1 |
      *    \ / \ /
      *    
      */
    [TestFixture]
    public class CellsTests
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
            Assert.AreEqual(3, new Cell(0, 0).DistanceTo(new Cell(-1, 3)));
            Assert.AreEqual(3, new Cell(0, 0).DistanceTo(new Cell(0, 3)));

            Assert.AreEqual(2, new Cell(0, 1).DistanceTo(new Cell(-1, 3)));
            Assert.AreEqual(3, new Cell(0, 1).DistanceTo(new Cell(-1, 4)));
        }


        /**
         * 
         *                  |       |       |       |       |       |       |     
         *                /-1 \   /-1 \   /-1 \   /-1 \   /   \   /-1 \   /-1.\    
         *              |   -2  |   -1 |    0  |    1  |       |    3  |  ...4..|
         *                \   / 0 \   / 0 \   / 0 \   / 0 \   /.0.\   / 0 \.../ 0 \
         *                  |   -2  |   -1  |    0  |    1  |....2..|    3  |    4  |
         *                /   \   /-1 \   / 1 \   / 1 \   / 1 \.../ 1 \   / 1 \   / 1 \
         *                      |   -2  |   -1  |    0  |    1  |    2  |    3  |    4  |
         *                \   /   \   /   \   /   \   / 2 \   / 2 \   / 2 \   / 2 \   /
         *                  |       |       |       |    0  |       |    2  |    3  |
         *                /   \   /   \   /   \   /   \   /   \   /   \   /   \   /
         *                      |       |       |       |       |       |       |
         *                \   /   \   /   \   /   \   /   \   /   \   /   \   /
         *  
         */
        [Test]
        public void TestPathTo()
        {
            var c1 = new Cell(0, 2);
            var c2 = new Cell(-1, 4);
            var r1 = c1.PathTo(c2);
            Assert.AreEqual(3, r1.Count);
            Assert.AreEqual(new Cell(0, 2), r1[0]);
            Assert.AreEqual(new Cell(-1, 3), r1[1]);
            Assert.AreEqual(new Cell(-1, 4), r1[2]);

            c1 = new Cell(0, 0);
            c2 = new Cell(2, 2);
            r1 = c1.PathTo(c2);
            Assert.AreEqual(5, r1.Count);
            Assert.AreEqual(new Cell(0, 0), r1[0]);
            Assert.AreEqual(new Cell(0, 1), r1[1]);
            Assert.AreEqual(new Cell(1, 1), r1[2]);
            Assert.AreEqual(new Cell(1, 2), r1[3]);
            Assert.AreEqual(new Cell(2, 2), r1[4]);
        }

        [Test]
        public void TestDirectionTo()
        {
            var cell = new Cell(0, 0);
            Assert.AreEqual(IdentityCell.leftSide, cell.GetDirectionTo(new Cell(0, -1)));
            Assert.AreEqual(IdentityCell.rightSide, cell.GetDirectionTo(new Cell(0, 1)));

            cell = new Cell(0, -2);
            Assert.AreEqual(IdentityCell.rightSide, cell.GetDirectionTo(new Cell(0, -1)));
            Assert.AreEqual(IdentityCell.leftSide, cell.GetDirectionTo(new Cell(0, -3)));
            cell = new Cell(0, -4);
            Assert.AreEqual(IdentityCell.rightSide, cell.GetDirectionTo(new Cell(0, -3)));
            Assert.AreEqual(IdentityCell.leftSide, cell.GetDirectionTo(new Cell(0, -5)));
            Assert.AreEqual(IdentityCell.upLeftSide, cell.GetDirectionTo(new Cell(-1, -4)));
            Assert.AreEqual(IdentityCell.downRightSide, cell.GetDirectionTo(new Cell(1, -4)));
        }
    }
}
