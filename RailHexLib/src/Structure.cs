﻿using System;
using System.Collections.Generic;

namespace RailHexLib
{
    public abstract class Structure : Interfaces.IRotatable<Tile>
    {
        public Structure(Cell center, string name)
        {
            this.center = center; this.name = name;
        }
        public Cell Center { get => center; }
        protected Cell center;
        protected string name = "Unnamed";
        public string Name
        {
            get => name;
            set
            {
                if (value.Length > 0)
                {
                    name = value;
                }
                else
                {
                    throw new ArgumentException("name should be non empty string");
                }
            }
        }

        public abstract string TileName();

        public abstract Cell GetEnterCell();

        /// <summary>
        /// Return all hexes that included in settlement. 
        /// Also includes center hex.
        /// </summary>
        /// <returns>Pairs of cells (positons) and tiles (values)</returns>
        public virtual Dictionary<Cell, Tile> GetHexes()
        {
            var result = new Dictionary<Cell, Tile>
            {
                [center] = new GrassTile()
            };
            return result;

        }
        public override string ToString()
        {
            return $"Structure ${name}";
        }

        public virtual void Tick(int ticks) 
        {
            if (WillAbandon(ticks))
            {
                EventHandler handler = OnStructureAbandon;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }            
            }
        }

        private bool WillAbandon(int ticks)
        {
            if (abandoned) return true;

            if (!abandoned && lifeTime > 0)
            {
                lifeTime -= ticks;
                if (lifeTime <= 0)
                {
                    abandoned = true;
                    return true;
                }
            }
            return false;
        }
        public event EventHandler OnStructureAbandon;
        private int lifeTime = Config.Settlements.InitialTicksToDie; 
        private bool abandoned = false;

    }

}
