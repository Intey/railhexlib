using NUnit.Framework;
using RailHexLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailHexLib.Tests
{
    [TestFixture]
    public class HexNodeTests
    {
        [Test]
        public void Link2HexNodeReference()
        {
            var node1 = new HexNode(new Cell(0, 0));
            var node2 = new HexNode(new Cell(0, 1));

            node1.Right = new(new Cell(0, 3));

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
            var node1 = new HexNode(new Cell(0, 0));
            var node2 = new HexNode(new Cell(0, 1));
            node1.Right = new(new Cell(0, 2));
            node1.Left = new(new Cell(0, 3));
            node1.UpLeft = new(new Cell(0, 4));
            node1.UpRight = new(new Cell(0, 5));
            node1.DownRight = new(new Cell(0, 6));
            node1.DownLeft = new(new Cell(0, 7));
            node1.Right.Right = new(new(-1, 0));

            Assert.AreSame(node1, node1.FindCell(new Cell(0, 0)), "should find self cell");
            Assert.AreSame(node1.Right, node1.FindCell(new Cell(0, 2)), "should find cell in right child");
            Assert.AreSame(node1.Left, node1.FindCell(new Cell(0, 3)), "should find cell in left child");
            Assert.AreSame(node1.UpLeft, node1.FindCell(new Cell(0, 4)), "should find cell in upLeft child");
            Assert.AreSame(node1.UpRight, node1.FindCell(new Cell(0, 5)), "should find cell in upRight child");
            Assert.AreSame(node1.DownRight, node1.FindCell(new Cell(0, 6)), "should find cell in downRight child");
            Assert.AreSame(node1.DownLeft, node1.FindCell(new Cell(0, 7)), "should find cell in downLeft child");
            Assert.AreSame(node1.Right.Right, node1.FindCell(new Cell(-1, 0)), "should find cell in right.right child");


            var node3 = new HexNode(new(0, -4));
            node3.Right = new HexNode(new(0, -3));
            Assert.AreSame(node3.Right, node3.FindCell(new Cell(0, -3)));
        }
        [Test]
        public void EnumerationTest()
        {
            var node = new HexNode(new Cell(0, 0));
            var nodesList = from n in node select n;
            Assert.AreEqual(new Cell(0, 0), nodesList.First());
        }

    }
}
