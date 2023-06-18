using System.Collections.Generic;
using System;

namespace RailHexLib
{
    public class TileStack
    {
        Random rnd = new Random();
        public bool Empty() => tiles.Count == 0;


        public TileStack()
        {
            tiles = new Queue<Tile>();
            availableTiles = new Dictionary<Type, int>()
            {
                [typeof(ROAD_180Tile)] = 10,
                [typeof(ROAD_120Tile)] = 10,
                [typeof(GrassTile)] = 30,
                [typeof(WaterTile)] = 20,
                [typeof(ForestTile)] = 20,
                [typeof(ROAD_3T_120_240Tile)] = 2,
                [typeof(ROAD_3T_60_120Tile)] = 5,
                [typeof(ROAD_3T_60_180Tile)] = 1,
            };
            foreach (var (type, count) in availableTiles)
            {
                for (int i = 0; i < count; i++)
                {
                    variants.Add(type);
                }
            }
        }
        public void InitializeInitialStack()
        {
            // first step should be easy for player
            PushTile(new ROAD_180Tile());
            PushTile(new ROAD_180Tile());
            PushTile(new ROAD_180Tile());
            PushTile(new ROAD_180Tile());
            PushTile(new ROAD_180Tile());
            PushTile(new ROAD_180Tile());
            PushTile(new ROAD_3T_60_180Tile());
            for (int i = 0; i < 20; i++)
            {
                PushRandomTile();
            }


        }
        public void PushTile(Tile t)
        {
            tiles.Enqueue(t);
        }
        public void PushRandomTile()
        {
            tiles.Enqueue(MakeRandomTileType());
        }
        public Tile PopTile()
        {
            if (tiles.Count == 0) return null;
            return tiles.Dequeue();
        }
        public IEnumerable<string> GetTiles()
        {
            foreach (var tile in tiles)
            {
                yield return tile.tileName();
            }
        }

        private void InitializeDefaultStack()
        {

            for (int i = 0; i < 60; i++)
            {
                var tile = MakeRandomTileType();
                PushTile(tile);
            }
        }
        Tile MakeRandomTileType()
        {
            var probability = rnd.NextDouble();

            var randomValue = variants[rnd.Next(variants.Count)];
            return (Tile)Activator.CreateInstance(randomValue);
        }
        readonly Queue<Tile> tiles;

        Dictionary<Type, int> availableTiles = new();
        List<Type> variants = new();

    }
}
