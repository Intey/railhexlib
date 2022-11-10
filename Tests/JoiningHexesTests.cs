using NUnit.Framework;
using RailHexLib.Grounds;
using System;
using System.Collections.Generic;

namespace RailHexLib.Tests
{
    [TestFixture]
    internal class JoiningHexesTests : EmptyGameFixture
    {
        const float CELL_SIZE = 1f;
        [Test]
        public void TestJoin2Tiles()
        {
            game.PushTile(new ROAD_180Tile());
            game.PushTile(new ROAD_180Tile());

            game.NextTile();

            var placeResult = game.PlaceCurrentTile(new Cell(0, 0, CELL_SIZE));
            Assert.IsTrue(placeResult,  $"{placeResult}");
            Assert.AreEqual(placeResult.NewJoins.Count, 0);

            placeResult = game.PlaceCurrentTile(new Cell(0, -1, CELL_SIZE));
            Assert.IsTrue(placeResult);
            Assert.AreEqual(placeResult.NewJoins.Count, 1);
            Assert.IsTrue(placeResult.NewJoins.ContainsKey(new Cell(0, 0, CELL_SIZE)));
            Assert.AreEqual(placeResult.NewJoins[new Cell(0, 0, CELL_SIZE)], Road.instance);
        }

        [Test]
        [Ignore("Doesn't had such case becase")]
        public void TestJoinBy2siblingsOfOneRoad() {

            game.PushTile(new ROAD_60Tile());
            game.PushTile(new ROAD_120Tile());
            game.PushTile(new ROAD_60Tile());
            game.PushTile(new ROAD_120Tile());
            
            game.NextTile();

            game.RotateCurrentTile();
            game.PlaceCurrentTile(new Cell(0, 1, CELL_SIZE));

            game.RotateCurrentTile();
            game.PlaceCurrentTile(new Cell(-1, 1, CELL_SIZE));

            game.RotateCurrentTile(2);
            game.PlaceCurrentTile(new Cell(0, 0, CELL_SIZE));

            game.RotateCurrentTile(4);
            var result = game.PlaceCurrentTile(new Cell(1, 0, CELL_SIZE));

            Assert.IsTrue(false);
            /**
              *        /1\
              *       | 0 |     Cycle  /*\
              *      /0\ /0\        * /   \ *
              *     | 0 | 1 |         \   /
              *      \ /-\ /           \ /
              *       | 1 |             *
              *        \ /
              */
            // Expected 
            // - We have 1 Orphan road 
            // - Upper tile have both links
            // - Same case, but we should already have a some Strcture road
            // that have cells on 0,0 and 0,1. Prevent making tradeRoute with
            // self
        }
    }
}
