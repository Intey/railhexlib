using NUnit.Framework;
using RailHexLib.DevTools;

namespace RailHexLib.Tests
{
    [TestFixture]
    internal class EmptyGameFixture
    {
        protected Game game;
        public EmptyGameFixture()
        {
            game = new Game();
        }

        [SetUp]
        public void SetUp()
        {
            game = new Game(stack: new TileStack(), logger: new Logger());

        }
    }
}
