﻿using System;
using System.Collections.Generic;

namespace RailHexLib.Interfaces
{
    public abstract class IRotatable<T>
    {
        public Dictionary<IdentityCell,T> Sides { get => sides; }
        public RotationType Rotation = RotationType._0;

        /**
         * 0 - is left 
         */
        public enum RotationType
        {
            _0,
            _60,
            _120,
            _180,
            _240,
            _300
        }

        public virtual void Rotate60Clock()
        {
            var degreeses = Enum.GetValues(typeof(RotationType));

            RotationType newRot;
            if (Rotation == RotationType._300)
                newRot = RotationType._0;
            else
                newRot = (RotationType)degreeses.GetValue((int)Rotation + 1);
            Rotation = newRot;
            var leftSide = sides[IdentityCell.leftSide];
            sides[IdentityCell.leftSide] = sides[IdentityCell.downLeftSide];
            sides[IdentityCell.downLeftSide] = sides[IdentityCell.downRightSide];
            sides[IdentityCell.downRightSide] = sides[IdentityCell.rightSide];
            sides[IdentityCell.rightSide] = sides[IdentityCell.upRightSide];
            sides[IdentityCell.upRightSide] = sides[IdentityCell.upLeftSide];
            sides[IdentityCell.upLeftSide] = leftSide;
        }

        public void Rotate60Clock(int count)
        {
            for(int i =0; i< count; i++)
            {
                Rotate60Clock();
            }
        }

        protected Dictionary<IdentityCell, T> sides = new Dictionary<IdentityCell, T>(new IdentityCellEqualityComparer());
    }
}
