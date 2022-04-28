using System.Collections.Generic;
namespace RailHexLib
{
    public class TileStack
    {
        private readonly Queue<Tile> tiles;

        public TileStack()
        {
            tiles = new Queue<Tile>();
        }
        public void AddTile(Tile t)
        {
            tiles.Enqueue(t);
        }
        public Tile PopTile()
        {
            return tiles.Dequeue();
        }

    }
}
