namespace RailHexLib {

    public static class Config {
        public static class Trader {
            /// how many tiles move a trader in one tick
            public static int moveTilesPerTick = 1;
            public static double consumptionPercent = 0.3;
            public static int maxResourceCountInInventory = 5;
        }
        public static class ScorePoints {
            public static int ForTradePointReached = 1;
        }
        public static class Stack {
            public static int TilesForTradePointReached = 1;
        }

        public static class Structure {
            public static int LifeTimeIncreaseOnTraderVisit = 5;
            public static int InitialTicksToDie = 30;
            public static int zoneConsumptionCount = 2;
            public static int MaxLifetime = 40;
        }
    }
}