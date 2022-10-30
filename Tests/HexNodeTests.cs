using NUnit.Framework;
using RailHexLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RailHexLib.Tests
{
    [TestFixture]
    public class HexNodeTests
    {
        const float CELL_SIZE = 1f;
        [Test]
        public void Link2HexNodeReference()
        {
            var node1 = new HexNode(new Cell(0, 0, CELL_SIZE));
            var node2 = new HexNode(new Cell(0, 1, CELL_SIZE));

            node1.Right = new(new Cell(0, 3, CELL_SIZE));

            node2.Right = node1;

            Assert.IsNotNull(node2.Right.Right, "Should save as reference");
            Assert.AreSame(node2, node2.Right.Left, "Should make backlink in node2");
            Assert.IsTrue(node2 == node2.Right.Left, "Check by reference");
        }

        [Test]
        [Ignore("Actually don't meet that case")]
        public void FindWithCycleInGraph()
        {
            Assert.IsTrue(
                false, "check that FindNode detects cycles and stop processing them");
        }

        [Test]
        public void FindCellTest()
        {
            var node1 = new HexNode(new  Cell(0, 0, CELL_SIZE));
            var node2 = new HexNode(new Cell(0, 1, CELL_SIZE));
            node1.Right = new(new Cell(0, 2, CELL_SIZE));
            node1.Left = new(new Cell(0, 3, CELL_SIZE));
            node1.UpLeft = new(new Cell(0, 4, CELL_SIZE));
            node1.UpRight = new(new Cell(0, 5, CELL_SIZE));
            node1.DownRight = new(new Cell(0, 6, CELL_SIZE));
            node1.DownLeft = new(new Cell(0, 7, CELL_SIZE));
            node1.Right.Right = new(new Cell(-1, 0, CELL_SIZE));

            Assert.AreSame(node1, node1.FindCell(new Cell(0, 0, CELL_SIZE)), "should find self cell");
            Assert.AreSame(node1.Right, node1.FindCell(new Cell(0, 2, CELL_SIZE)), "should find cell in right child");
            Assert.AreSame(node1.Left, node1.FindCell(new Cell(0, 3, CELL_SIZE)), "should find cell in left child");
            Assert.AreSame(node1.UpLeft, node1.FindCell(new Cell(0, 4, CELL_SIZE)), "should find cell in upLeft child");
            Assert.AreSame(node1.UpRight, node1.FindCell(new Cell(0, 5, CELL_SIZE)), "should find cell in upRight child");
            Assert.AreSame(node1.DownRight, node1.FindCell(new Cell(0, 6, CELL_SIZE)), "should find cell in downRight child");
            Assert.AreSame(node1.DownLeft, node1.FindCell(new Cell(0, 7, CELL_SIZE)), "should find cell in downLeft child");
            Assert.AreSame(node1.Right.Right, node1.FindCell(new Cell(-1, 0, CELL_SIZE)), "should find cell in right.right child");


            var node3 = new HexNode(new(0, -4, CELL_SIZE));
            node3.Right = new HexNode(new(0, -3, CELL_SIZE));
            Assert.AreSame(node3.Right, node3.FindCell(new Cell(0, -3, CELL_SIZE)));
        }


        [Test]
        public void FindCellLongCycleTest()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE));
            node.Right = new HexNode(new Cell(0, 1, CELL_SIZE));
            node.Right.Right = new HexNode(new Cell(0, 2, CELL_SIZE));
            node.Right.Right.UpLeft = new(new(-1, 2, CELL_SIZE));
            node.Right.Right.UpLeft.UpLeft = new(new(-2, 2, CELL_SIZE));
            node.Right.Right.UpLeft.UpLeft.Left = new(new(-2, 1, CELL_SIZE));
            node.Right.Right.UpLeft.UpLeft.Left.DownLeft = new(new(-1, 0, CELL_SIZE));
            node.Right.Right.UpLeft.UpLeft.Left.DownLeft.DownRight = node;

            Assert.AreEqual(node.Right.Right.UpLeft.UpLeft.Left, node.FindCell(new Cell(-2, 1, CELL_SIZE)));
            Assert.AreEqual(null, node.FindCell(new Cell(-3, 1, CELL_SIZE)));

        }
        [Test]
        public void nodeSideBecomesNullOnSetInconsistentCell()
        {
            var node = new HexNode(new(0, 0, CELL_SIZE));
            node.Left = new HexNode(new(0, -1, CELL_SIZE));
            node.Left.Left = new HexNode(new(0, -2, CELL_SIZE));
            node.Left.UpRight = new HexNode(new(-1, -1, CELL_SIZE));
            node.Left.UpRight.DownRight = node.Left;
            node.Left.UpRight.Right = new HexNode(new(-1, 0, CELL_SIZE));
            node.Left.UpRight.Right.DownLeft = node.Left;
            Assert.IsNull(node.Left.UpRight.Right);
        }
        [Test]
        public void findCellUnexistTest()
        {
            var node = new HexNode(new(0, 0, CELL_SIZE));
            node.Left = new HexNode(new(0, -1, CELL_SIZE));
            node.Right = new HexNode(new(0, 1, CELL_SIZE));
            node.Right.DownLeft = node.Left;
            Assert.AreEqual(null, node.FindCell(new(-1, -2, CELL_SIZE)));
        }
        [Test]
        public void FindCellCycleTest()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE));
            node.Right = new HexNode(new Cell(0, 1, CELL_SIZE));
            node.Right.UpLeft = new HexNode(new Cell(-1, 1, CELL_SIZE));
            node.UpRight = node.Right.UpLeft;

            Assert.AreEqual(node.UpRight, node.FindCell(new Cell(-1, 1, CELL_SIZE)));
        }

        [Test]
        public void EnumerationTest()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE))
            {
                Right = new HexNode(new Cell(0, 1, CELL_SIZE))
            };

            var nodesList = (from n in node select n.Cell).ToArray();
            Assert.AreEqual(new Cell(0, 0, CELL_SIZE), nodesList[0]);
            Assert.AreEqual(new Cell(0, 1, CELL_SIZE), nodesList[1]);
        }

        [Test]
        public void EnumeratorCycleTest()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE));
            node.Right = new HexNode(new Cell(0, 1, CELL_SIZE));
            node.Right.UpLeft = new HexNode(new Cell(-1, 1, CELL_SIZE));
            node.UpRight = node.Right.UpLeft;

            var nodesList = (from n in node select n.Cell).ToArray();
            Assert.AreEqual(new Cell(0, 0, CELL_SIZE), nodesList[0]);
            Assert.AreEqual(new Cell(-1, 1, CELL_SIZE), nodesList[1]);
            Assert.AreEqual(new Cell(0, 1, CELL_SIZE), nodesList[2]);
        }

        [Test]
        public void EnumeratorMultiwayTest()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE));
            node.Left = new HexNode(new Cell(0, -1, CELL_SIZE));
            node.UpRight = new HexNode(new Cell(-1, 1, CELL_SIZE));
            node.UpRight.Left = new HexNode(new Cell(-1, 0, CELL_SIZE));
            node.UpRight.UpRight = new HexNode(new Cell(-2, 2, CELL_SIZE));
            node.UpRight.UpRight.UpRight = new HexNode(new Cell(-3, 3, CELL_SIZE));

            var nodesList = (from n in node select n.Cell).ToArray();
            Assert.AreEqual(6, nodesList.Length);
            Assert.AreEqual(new Cell(0, 0, CELL_SIZE), nodesList[0]);
            Assert.AreEqual(new Cell(0, -1, CELL_SIZE), nodesList[1]);
            Assert.AreEqual(new Cell(-1, 1, CELL_SIZE), nodesList[2]);
            Assert.AreEqual(new Cell(-1, 0, CELL_SIZE), nodesList[3]);
            Assert.AreEqual(new Cell(-2, 2, CELL_SIZE), nodesList[4]);
            Assert.AreEqual(new Cell(-3, 3, CELL_SIZE), nodesList[5]);
        }
    }
}
