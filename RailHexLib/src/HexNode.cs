using RailHexLib.Grounds;
using System;
using System.Collections.Generic;

namespace RailHexLib
{
    public class HexNode
    {
        public Cell Value;
        // links
        private HexNode left;
        private HexNode upLeft;
        private HexNode upRight;
        private HexNode right;
        private HexNode downRight;
        private HexNode downLeft;

        public HexNode(Cell c)
        {
            Value = c;
        }

        public HexNode Left
        {
            get => left;
            set {
                left = value;
                value.right = this;
            }
        }
        public HexNode UpLeft
        {
            get => upLeft; 
            set
            {
                upLeft = value;
                value.downRight = this;
            }
        }
        public HexNode UpRight
        {
            get => upRight;
            set
            {
                upRight = value;
                value.downLeft = this;
            }
        }
        public HexNode Right
        {
            get => right;
            set
            {
                right = value;
                value.left = this;
            }
        }
        public HexNode DownRight
        {
            get => downRight;
            set
            {
                downRight = value;
                value.upLeft = this;
            }
        }
        public HexNode DownLeft
        {
            get => downLeft;
            set
            {
                downLeft = value;
                value.upRight = this;
            }
        }
    }
}
