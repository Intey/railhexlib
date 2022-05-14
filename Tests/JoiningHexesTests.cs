using NUnit.Framework;
using RailHexLib.Grounds;
using System;
using System.Collections.Generic;

namespace RailHexLib.Tests
{
    [TestFixture]
    internal class JoiningHexesTests : EmptyGameFixture
    {
        [SetUp]
        public void Setup()
        {
            game.PushTile(new ROAD_180Tile());
            game.PushTile(new ROAD_180Tile());

        }

        [Test]
        public void join2Tiles()
        {
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


    }
}