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
    public class GraphNodeTests
    {
        GraphNode<Cell> graph;
        GraphNode<Cell> graph0m1;
        [SetUp]
        public void SetUp()
        {
            graph = new GraphNode<Cell>(new Cell(0, 0));
            graph0m1 = new GraphNode<Cell>(new Cell(0, -1));
        }

        [Test]
        public void CellTileToGraphNodeTest()
        {
            Assert.AreEqual(new Cell(0, 0), graph.Value);
            Assert.AreEqual(0, graph.Children.Count);
        }

        [Test]
        public void AddChild()
        {
            var child = new Cell(0, -2);
            graph.Children.Add(new GraphNode<Cell>(child));

            var target = new Cell(0, -3);

            graph.AddToChildrenBy(target, (nodeValue, n) => nodeValue.DistanceTo(n) == 1);

            List<GraphNode<Cell>> expectedOwnerChildren = graph.Children[0].Children;
            Assert.AreEqual(1, expectedOwnerChildren.Count);
            Assert.AreSame(target, expectedOwnerChildren[0].Value);

        }
    }
}