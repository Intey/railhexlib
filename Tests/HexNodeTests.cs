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
    }
}