using System;
using System.Collections.Generic;



namespace RailHexLib.Grounds
{

    public static class Extensions
    {
        private static Dictionary<Ground, bool> joinsMap = new Dictionary<Ground, bool>(){
            [Ground.Water] = true,
            [Ground.Grass] = true,
            [Ground.Road] = true,
            [Ground.Ground] = false,

        };

        public static bool IsJoinable(this Ground t) {
            return joinsMap[t];
        }
    }
    public enum Ground
    {
        Water,
        Grass,
        Road,
        Ground
    }
}
