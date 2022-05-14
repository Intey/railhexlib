using NUnit.Framework;
using RailHexLib.DevTools;

namespace RailHexLib.Tests
{
    [TestFixture]
    internal class EmptyGameFixture
    {
        protected Game game;
        [SetUp]
        public void SetUp()
        {
            game = new Game(stack: new TileStack(), logger: new Logger());

        }
    }
}
