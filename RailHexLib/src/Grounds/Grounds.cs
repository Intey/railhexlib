using System;
using System.Collections.Generic;



namespace RailHexLib.Grounds
{

    public static class Extensions
    {
        private static Dictionary<Ground, bool> JOINS_MAP = new Dictionary<Ground, bool>(){
            [Ground.Water] = true,
            [Ground.Grass] = true,
            [Ground.Road] = true,
            [Ground.Ground] = false,
            [Ground.Forest] = true

        };

        public static bool IsJoinable(this Ground t) {
            return JOINS_MAP[t];
        }
    }
    public enum Ground
    {
        Water,
        Grass,
        Road,
        Ground,
        Forest
    }
}
