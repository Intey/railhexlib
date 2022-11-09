using NUnit.Framework;
using RailHexLib.DevTools;
using RailHexLib;

namespace RailHexLib.Tests
{
    [TestFixture]
    internal class EmptyGameFixture
    {
        protected Game game;

        [SetUp]
        public void SetUp()
        {
            Game.Reset(stack: new TileStack(), logger: new Logger());
            game = Game.GetInstance();
        }
    }
}
