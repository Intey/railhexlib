using NUnit.Framework;
using RailHexLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace RailHexLib.Tests
{
    [TestFixture]
    public class HexNodeTests
    {
        const float CELL_SIZE = 1f;
        [SetUp]
        public void SetUp() {
            //Trace.Listeners.Add(new ConsoleTraceListener());
        }
        [Test]
        public void TestLink2HexNodeReference()
        {
            var node1 = new HexNode(new Cell(0, 0, CELL_SIZE));
            var node2 = new HexNode(new Cell(0, 1, CELL_SIZE));

            node1.BottomRight = new(new Cell(0, 3, CELL_SIZE));

            node2.BottomRight = node1;

            Assert.IsNotNull(node2.BottomRight.BottomRight, "Should save as reference");
            Assert.AreSame(node2, node2.BottomRight.TopLeft, "Should make backlink in node2");
            Assert.IsTrue(node2 == node2.BottomRight.TopLeft, "Check by reference");
        }

        [Test]
        [Ignore("Actually don't meet that case")]
        public void TestFindWithCycleInGraph()
        {
            Assert.IsTrue(
                false, "check that FindNode detects cycles and stop processing them");
        }

        [Test]
        public void TestFindCell()
        {
            var node1 = new HexNode(new  Cell(0, 0, CELL_SIZE));
            var node2 = new HexNode(new Cell(0, 1, CELL_SIZE));
            node1.BottomRight = new(new Cell(0, 2, CELL_SIZE));
            node1.TopLeft = new(new Cell(0, 3, CELL_SIZE));
            node1.Top = new(new Cell(0, 4, CELL_SIZE));
            node1.TopRight = new(new Cell(0, 5, CELL_SIZE));
            node1.Bottom = new(new Cell(0, 6, CELL_SIZE));
            node1.BottomLeft = new(new Cell(0, 7, CELL_SIZE));
            node1.BottomRight.BottomRight = new(new Cell(-1, 0, CELL_SIZE));

            Assert.AreSame(node1, node1.FindCell(new Cell(0, 0, CELL_SIZE)), "should find self cell");
            Assert.AreSame(node1.BottomRight, node1.FindCell(new Cell(0, 2, CELL_SIZE)), "should find cell in right child");
            Assert.AreSame(node1.TopLeft, node1.FindCell(new Cell(0, 3, CELL_SIZE)), "should find cell in left child");
            Assert.AreSame(node1.Top, node1.FindCell(new Cell(0, 4, CELL_SIZE)), "should find cell in upLeft child");
            Assert.AreSame(node1.TopRight, node1.FindCell(new Cell(0, 5, CELL_SIZE)), "should find cell in upRight child");
            Assert.AreSame(node1.Bottom, node1.FindCell(new Cell(0, 6, CELL_SIZE)), "should find cell in downRight child");
            Assert.AreSame(node1.BottomLeft, node1.FindCell(new Cell(0, 7, CELL_SIZE)), "should find cell in downLeft child");
            Assert.AreSame(node1.BottomRight.BottomRight, node1.FindCell(new Cell(-1, 0, CELL_SIZE)), "should find cell in right.right child");


            var node3 = new HexNode(new(0, -4, CELL_SIZE));
            node3.BottomRight = new HexNode(new(0, -3, CELL_SIZE));
            Assert.AreSame(node3.BottomRight, node3.FindCell(new Cell(0, -3, CELL_SIZE)));
        }


        [Test]
        public void TestFindCellLongCycle()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE));
            node.BottomRight = new HexNode(new Cell(0, 1, CELL_SIZE));
            node.BottomRight.BottomRight = new HexNode(new Cell(0, 2, CELL_SIZE));
            node.BottomRight.BottomRight.Top = new(new(-1, 2, CELL_SIZE));
            node.BottomRight.BottomRight.Top.Top = new(new(-2, 2, CELL_SIZE));
            node.BottomRight.BottomRight.Top.Top.TopLeft = new(new(-2, 1, CELL_SIZE));
            node.BottomRight.BottomRight.Top.Top.TopLeft.BottomLeft = new(new(-1, 0, CELL_SIZE));
            node.BottomRight.BottomRight.Top.Top.TopLeft.BottomLeft.Bottom = node;

            Assert.AreEqual(node.BottomRight.BottomRight.Top.Top.TopLeft, node.FindCell(new Cell(-2, 1, CELL_SIZE)));
            Assert.AreEqual(null, node.FindCell(new Cell(-3, 1, CELL_SIZE)));

        }
        [Test]
        public void TestNodeSideBecomesNullOnSetInconsistentCell()
        {
            var node = new HexNode(new(0, 0, CELL_SIZE));
            node.TopLeft = new HexNode(new(0, -1, CELL_SIZE));
            node.TopLeft.TopLeft = new HexNode(new(0, -2, CELL_SIZE));
            node.TopLeft.TopRight = new HexNode(new(-1, -1, CELL_SIZE));
            node.TopLeft.TopRight.Bottom = node.TopLeft;
            node.TopLeft.TopRight.BottomRight = new HexNode(new(-1, 0, CELL_SIZE));
            node.TopLeft.TopRight.BottomRight.BottomLeft = node.TopLeft;
            Assert.IsNull(node.TopLeft.TopRight.BottomRight);
        }
        [Test]
        public void TestFindCellUnexist()
        {
            var node = new HexNode(new(0, 0, CELL_SIZE));
            node.TopLeft = new HexNode(new(0, -1, CELL_SIZE));
            node.BottomRight = new HexNode(new(0, 1, CELL_SIZE));
            node.BottomRight.BottomLeft = node.TopLeft;
            Assert.AreEqual(null, node.FindCell(new(-1, -2, CELL_SIZE)));
        }
        [Test]
        public void TestFindCellCycle()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE));
            node.BottomRight = new HexNode(new Cell(0, 1, CELL_SIZE));
            node.BottomRight.Top = new HexNode(new Cell(-1, 1, CELL_SIZE));
            node.TopRight = node.BottomRight.Top;

            Assert.AreEqual(node.TopRight, node.FindCell(new Cell(-1, 1, CELL_SIZE)));
        }

        [Test]
        public void TestEnumeration()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE))
            {
                BottomRight = new HexNode(new Cell(0, 1, CELL_SIZE))
            };

            var nodesList = (from n in node select n.Cell).ToArray();
            Assert.AreEqual(new Cell(0, 0, CELL_SIZE), nodesList[0]);
            Assert.AreEqual(new Cell(0, 1, CELL_SIZE), nodesList[1]);
        }

        [Test]
        public void TestEnumeratorCycle()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE));
            node.BottomRight = new HexNode(new Cell(0, 1, CELL_SIZE));
            node.BottomRight.Top = new HexNode(new Cell(-1, 1, CELL_SIZE));
            node.TopRight = node.BottomRight.Top;

            var nodesList = (from n in node select n.Cell).ToArray();
            Assert.AreEqual(new Cell(0, 0, CELL_SIZE), nodesList[0]);
            Assert.AreEqual(new Cell(-1, 1, CELL_SIZE), nodesList[1]);
            Assert.AreEqual(new Cell(0, 1, CELL_SIZE), nodesList[2]);
        }

        [Test]
        public void TestEnumeratorMultiway()
        {
            var node = new HexNode(new Cell(0, 0, CELL_SIZE));
            node.TopLeft = new HexNode(new Cell(0, -1, CELL_SIZE));
            node.TopRight = new HexNode(new Cell(-1, 1, CELL_SIZE));
            node.TopRight.TopLeft = new HexNode(new Cell(-1, 0, CELL_SIZE));
            node.TopRight.TopRight = new HexNode(new Cell(-2, 2, CELL_SIZE));
            node.TopRight.TopRight.TopRight = new HexNode(new Cell(-3, 3, CELL_SIZE));

            var nodesList = (from n in node select n.Cell).ToArray();
            Assert.AreEqual(6, nodesList.Length);
            Assert.AreEqual(new Cell(0, 0, CELL_SIZE), nodesList[0]);
            Assert.AreEqual(new Cell(0, -1, CELL_SIZE), nodesList[1]);
            Assert.AreEqual(new Cell(-1, 1, CELL_SIZE), nodesList[2]);
            Assert.AreEqual(new Cell(-1, 0, CELL_SIZE), nodesList[3]);
            Assert.AreEqual(new Cell(-2, 2, CELL_SIZE), nodesList[4]);
            Assert.AreEqual(new Cell(-3, 3, CELL_SIZE), nodesList[5]);
        }

        [Test]
        public void TestGodotTileMapPositioning()
        {
            var node =  new HexNode(new Cell(0,-1, CELL_SIZE));
            node.TopLeft = new HexNode(new Cell(0,-2, CELL_SIZE));
            node = node.TopLeft;
            node.TopLeft = new HexNode(new Cell(0,-3, CELL_SIZE));
            node.TopLeft.Top = new HexNode(new Cell(1,-4, CELL_SIZE));
            Assert.IsNotNull(node.PathTo(new Cell(1,-4,CELL_SIZE)));
        }
        [Test]
        public void TestPathTo() 
        {
            var startNode = new HexNode(new Cell(0, -1, CELL_SIZE));
            var target = new Cell(0, -8, CELL_SIZE);

            startNode.TopLeft = new HexNode(new Cell(0,-2, CELL_SIZE));
            var next = startNode.TopLeft;
            next.TopLeft = new HexNode(new Cell(0,-3, CELL_SIZE));
            next = next.TopLeft;
            next.TopLeft = new HexNode(new Cell(0,-4, CELL_SIZE));
            next = next.TopLeft;
            next.TopLeft = new HexNode(new Cell(0,-5, CELL_SIZE));
            next = next.TopLeft;
            next.Top = new HexNode(new Cell(1,-6, CELL_SIZE));
            next = next.Top;
            next.TopLeft = new HexNode(new Cell(1, -7, CELL_SIZE));
            next = next.TopLeft;
            next.TopLeft = new HexNode(target);
            // next = next.TopLeft;
            // next.BottomLeft = new HexNode(new Cell(0, -8, CELL_SIZE));

            var path = startNode.PathTo(target);

            Assert.AreEqual(new List<Cell>(){
                new Cell(0,-1, CELL_SIZE),
                new Cell(0,-2, CELL_SIZE),
                new Cell(0,-3, CELL_SIZE),
                new Cell(0,-4, CELL_SIZE),
                new Cell(0,-5, CELL_SIZE),
                new Cell(1,-6, CELL_SIZE),
                new Cell(1,-7, CELL_SIZE),
                target
            },
            path);
        }
    }
}
