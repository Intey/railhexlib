using System;
using System.Collections;

namespace RailHexLib.Grounds
{
    
    public abstract class Ground
    {
        protected Ground() { }

        public override bool Equals(object obj)
        {
            return obj is Ground;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public abstract bool IsJoinable();

    }


    public class Grass : Ground
    {
        public static readonly Grass instance = new Grass();

        private Grass() { }

        public override bool IsJoinable()
        {
            return false;
        }
        public override bool Equals(object obj)
        {
            return obj is Grass;
        }
        public override int GetHashCode()
        {
            return 1;
        }

        public override string ToString()
        {
            return "Grass";
        }
    }


    public class Water : Ground
    {
        public static readonly Water instance = new Water();

        private Water() { }
        public override bool IsJoinable()
        {
            return false;
        }
        public override bool Equals(object obj)
        {
            return obj is Water;
        }
        public override int GetHashCode()
        {
            return 2;
        }

        public override string ToString()
        {
            return "Water";
        }
    }


    public class Road : Ground
    {
        public static readonly Road instance = new Road();

        private Road() { }
        public override bool IsJoinable()
        {
            return true;
        }
        public override bool Equals(object obj)
        {
            return obj is Road;
        }
        public override int GetHashCode()
        {
            return 3;
        }

        public override string ToString()
        {
            return "Road";
        }
    }
}
