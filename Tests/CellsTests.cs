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

        const float CELL_SIZE = 1f;
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
            Assert.AreEqual(1, new Cell(0, 0, CELL_SIZE).DistanceTo(new Cell(0,  -1, CELL_SIZE)));
            Assert.AreEqual(2, new Cell(0, 0, CELL_SIZE).DistanceTo(new Cell(1,  -2, CELL_SIZE)));
            Assert.AreEqual(2, new Cell(0, 0, CELL_SIZE).DistanceTo(new Cell(-1, -1, CELL_SIZE)));
            Assert.AreEqual(3, new Cell(0, 0, CELL_SIZE).DistanceTo(new Cell(-1,  3, CELL_SIZE)));
            Assert.AreEqual(3, new Cell(0, 0, CELL_SIZE).DistanceTo(new Cell(0,   3, CELL_SIZE)));
            Assert.AreEqual(2, new Cell(0, 1, CELL_SIZE).DistanceTo(new Cell(-1,  3, CELL_SIZE)));
            Assert.AreEqual(3, new Cell(0, 1, CELL_SIZE).DistanceTo(new Cell(-1,  4, CELL_SIZE)));
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
            var c1 = new Cell(0, 2, CELL_SIZE);
            var c2 = new Cell(-1, 4, CELL_SIZE);
            var r1 = c1.PathTo(c2);
            Assert.AreEqual(3, r1.Count);
            Assert.AreEqual(new Cell(0, 2, CELL_SIZE), r1[0]);
            Assert.AreEqual(new Cell(-1, 3, CELL_SIZE), r1[1]);
            Assert.AreEqual(new Cell(-1, 4, CELL_SIZE), r1[2]);

            c1 = new Cell(0, 0, CELL_SIZE);
            c2 = new Cell(2, 2, CELL_SIZE);
            r1 = c1.PathTo(c2);
            Assert.AreEqual(5, r1.Count);
            Assert.AreEqual(new Cell(0, 0, CELL_SIZE), r1[0]);
            Assert.AreEqual(new Cell(0, 1, CELL_SIZE), r1[1]);
            Assert.AreEqual(new Cell(1, 1, CELL_SIZE), r1[2]);
            Assert.AreEqual(new Cell(1, 2, CELL_SIZE), r1[3]);
            Assert.AreEqual(new Cell(2, 2, CELL_SIZE), r1[4]);
        }

        [Test]
        public void TestDirectionTo()
        {
            var cell = new Cell(0, 0, CELL_SIZE);
            Assert.AreEqual(IdentityCell.leftSide, cell.GetDirectionTo(new Cell(0, -1, CELL_SIZE)));
            Assert.AreEqual(IdentityCell.rightSide, cell.GetDirectionTo(new Cell(0, 1, CELL_SIZE)));

            cell = new Cell(0, -2, CELL_SIZE);
            Assert.AreEqual(IdentityCell.rightSide, cell.GetDirectionTo(new Cell(0, -1, CELL_SIZE)));
            Assert.AreEqual(IdentityCell.leftSide, cell.GetDirectionTo(new Cell(0, -3, CELL_SIZE)));
            cell = new Cell(0, -4, CELL_SIZE);
            Assert.AreEqual(IdentityCell.rightSide, cell.GetDirectionTo(new Cell(0, -3, CELL_SIZE)));            
            Assert.AreEqual(IdentityCell.leftSide, cell.GetDirectionTo(new Cell(0, -5, CELL_SIZE)));
            Assert.AreEqual(IdentityCell.upLeftSide, cell.GetDirectionTo(new Cell(-1, -4, CELL_SIZE)));
            Assert.AreEqual(IdentityCell.downRightSide, cell.GetDirectionTo(new Cell(1, -4, CELL_SIZE)));
        }

        [Test]
        public void InvertedTest()
        {
            Assert.AreEqual(IdentityCell.leftSide, IdentityCell.rightSide.Inverted());
            Assert.AreEqual(IdentityCell.rightSide, IdentityCell.leftSide.Inverted());

            Assert.AreEqual(IdentityCell.upLeftSide, IdentityCell.downRightSide.Inverted());
            Assert.AreEqual(IdentityCell.downLeftSide, IdentityCell.upRightSide.Inverted());
        }
    }
}
