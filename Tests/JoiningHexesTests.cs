using NUnit.Framework;
using RailHexLib.Grounds;
using System;
using System.Collections.Generic;

namespace RailHexLib.Tests
{
    [TestFixture]
    internal class JoiningHexesTests : EmptyGameFixture
    {
        [Test]
        public void join2Tiles()
        {
            game.PushTile(new ROAD_180Tile());
            game.PushTile(new ROAD_180Tile());

            game.NextTile();

            var placeResult = game.PlaceCurrentTile(new Cell(0, 0));
            Assert.IsTrue(placeResult);
            Assert.AreEqual(placeResult.NewJoins.Count, 0);

            placeResult = game.PlaceCurrentTile(new Cell(0, -1));
            Assert.IsTrue(placeResult);
            Assert.AreEqual(placeResult.NewJoins.Count, 1);
            Assert.IsTrue(placeResult.NewJoins.ContainsKey(new Cell(0, 0)));
            Assert.AreEqual(placeResult.NewJoins[new Cell(0, 0)], Road.instance);
        }

        [Test]
        public void joinBy2siblingsOfOneRoad() {

            game.PushTile(new ROAD_60Tile());
            game.PushTile(new ROAD_120Tile());
            game.PushTile(new ROAD_60Tile());
            game.PushTile(new ROAD_120Tile());
            
            game.NextTile();

            game.RotateCurrentTile();
            game.PlaceCurrentTile(new Cell(0, 1));

            game.RotateCurrentTile();
            game.PlaceCurrentTile(new Cell(-1, 1));

            game.RotateCurrentTile(2);
            game.PlaceCurrentTile(new Cell(0, 0));

            game.RotateCurrentTile(4);
            var result = game.PlaceCurrentTile(new Cell(1, 0));

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
