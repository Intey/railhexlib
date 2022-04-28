namespace RailHexLib
{
    public abstract class Structure: Interfaces.IRotatable<Tile>
    {
        public Structure(Cell center) => this.center = center;
        public Cell Center { get => center; }
        protected Cell center;
        public abstract string TileName();

        public abstract Cell GetIcomeRoadCell();
    }
}
