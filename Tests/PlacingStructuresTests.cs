using NUnit.Framework;
using RailHexLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using RailHexLib.Grounds;

namespace RailHexLib.Tests
{

    [TestFixture]
    public class PlacingStructuresTests
    {
        const float CELL_SIZE = 1f;
        public TileStack stack = new();
        readonly List<Structure> structures = new();
        readonly DevTools.Logger logger = new();
        private Game game = new Game();
        private readonly Cell settle1Position = new(0, 0, CELL_SIZE);
        private readonly Cell settle2Position = new(0, -4, CELL_SIZE);

        [SetUp]
        public void Prepare()
        {
            Settlement settlement1 = new(settle1Position, "settlement1");
            // settlement size is 3tiles on R, so from center we have 2 tiles.
            // -4  -3  -2  -1   0
            // S2  S2  --  S1  S1
            Settlement settlement2 = new(settle2Position, "settlement2");
            // rotate 180. Now it's enter points to settlement 1
            settlement2.Rotate60Clock(3);
            structures.Clear();
            structures.Add(settlement1);
            structures.Add(settlement2);

            game = new Game(stack, logger);
        }

        [Test]
        public void TestPlaceStructures()
        {
            game.PushTile(new GrassTile());
            game.AddStructures(structures);
            var structs = game.Structures.ToList();
            Assert.AreEqual(new Cell(0, -1, CELL_SIZE), structs[0].GetEnterCell());
            Assert.AreEqual(new Cell(0, -3, CELL_SIZE), structs[1].GetEnterCell());

            game.NextTile();

            foreach (var structure in structs)
            {
                foreach (var cell in structure.GetHexes())
                {
                    Assert.IsFalse(game.PlaceCurrentTile(cell.Key), $"structure tiles should be taken {cell.Key}({structure}");
                }
            }

            Assert.AreEqual(2, game.StructureRoads.Count);
            Assert.AreEqual(new Cell(0, -1, CELL_SIZE), game.StructureRoads[new Cell(0, -1, CELL_SIZE)].road.FindCell(new Cell(0, -1, CELL_SIZE))?.Cell);
            Assert.AreEqual(new Cell(0, -3, CELL_SIZE), game.StructureRoads[new Cell(0, -3, CELL_SIZE)].road.FindCell(new Cell(0, -3, CELL_SIZE))?.Cell);
        }
    }
}
