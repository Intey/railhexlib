namespace RailHexLib
{

    public static class Config
    {

        public static class Game
        {
            public static int TicksForFoolTile = 20;
        }
        public static class Trader
        {
            /// how many tiles move a trader in one tick
            public static int moveTilesPerTick = 1;
            public static double consumptionPercent = 0.5;
            public static int exchangeOutCount = 15;
            public static int maxResourceCountInInventory = 25;
        }
        public static class ScorePoints
        {
            public static int ForTradePointReached = 1;
        }
        public static class Stack
        {
            public static int TilesForTradePointReached = 1;
        }

        public static class Structure
        {
            public static int AbandonTimerTicks = 2;
            public static int LifeTimeIncreaseOnTraderVisit = 5;
            public static int InitialLife = 120;
            public static int ZoneConsumptionCount = 5;
            // when connect one tile to structure we got 5 resource
            // when connect 2 tiles of same resource to a structure we got (5+5)*1.5 = 15 resources
            public static float ZoneJoinsMultiplier = 1.5f;
        }
        public static class Zone
        {
            public static int defaultResourceCount = 100;
            
        }
        public static class NewSettlement
        {
            public static double FarPositionProbability = 0.8;
            public static int TicksForNewSettlement = 100;
            public static int minNearestCellOffset = 20;
            public static int offsetBufferLinesCount = 10;
        }
    }
}