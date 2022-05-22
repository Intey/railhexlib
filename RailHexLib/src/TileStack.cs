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
        public void PushTile(Tile t)
        {
            tiles.Enqueue(t);
        }
        public Tile PopTile()
        {
            if (tiles.Count == 0) return null;
            return tiles.Dequeue();
        }

    }
}
