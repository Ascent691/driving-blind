namespace Infrastructure
{
    public struct TravelCell : IEquatable<TravelCell>
    {
        public CellType CellType;
        public bool CanTravelHorizontal;
        public bool CanTravelVertical;

        public TravelCell(CellType cellType)
        {
            CellType = cellType;
            CanTravelVertical = cellType != CellType.Wall && cellType != CellType.Hazard;
            CanTravelHorizontal = cellType != CellType.Wall && cellType != CellType.Hazard;
        }

        public TravelCell WithHorizontalHazard()
        {
            CanTravelHorizontal = false;
            return this;
        }

        public TravelCell WithVerticalHazard() {
            CanTravelVertical = false;
            return this;
        }

        public readonly bool Equals(TravelCell other)
        {
            return CanTravelVertical == other.CanTravelVertical 
                && CanTravelHorizontal == other.CanTravelHorizontal 
                && CellType == other.CellType;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is TravelCell cell && Equals(cell);
        }

        public override int GetHashCode()
        {
            return CellType.GetHashCode();
        }
                
        public static bool operator ==(TravelCell l, TravelCell r)
        {
            return l.CanTravelVertical == r.CanTravelVertical && l.CanTravelHorizontal == r.CanTravelHorizontal && l.CellType == r.CellType;
        }

        public static bool operator !=(TravelCell l, TravelCell r)
        {
            return l.CanTravelVertical != r.CanTravelVertical || l.CanTravelHorizontal != r.CanTravelHorizontal || l.CellType != r.CellType;
        }

        public override readonly string ToString()
        {
            return CellType.Identifier.ToString();
        }
    }
}
