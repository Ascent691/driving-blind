namespace Infrastructure
{
    public abstract class TravelPoint(bool canTravelVertical, bool canTravelHorizontal)
    {
        public bool CanTravelVertical = canTravelVertical;
        public bool CanTravelHorizontal = canTravelHorizontal;
        public abstract TravelPoint WithVerticalHazard();
        public abstract TravelPoint WithHorizontalHazard();
    }

    public class OpenTravelPoint : TravelPoint
    {
        public OpenTravelPoint(): base(true, true) { }

        public override TravelPoint WithHorizontalHazard()
        {
            return new VerticalOnlyTravelPoint();
        }

        public override TravelPoint WithVerticalHazard()
        {
            return new HorizontalOnlyTravelPoint();
        }
    }

    public class VerticalOnlyTravelPoint : TravelPoint
    {
        public VerticalOnlyTravelPoint(): base(true, false) {}

        public override TravelPoint WithHorizontalHazard()
        {
            return new NonTravelPoint();
        }

        public override TravelPoint WithVerticalHazard()
        {
            return this;
        }
    }

    public class HorizontalOnlyTravelPoint : TravelPoint
    {
        public HorizontalOnlyTravelPoint(): base(false, true) {}

        public override TravelPoint WithHorizontalHazard()
        {
            return this;
        }

        public override TravelPoint WithVerticalHazard()
        {
            return new NonTravelPoint();
        }
    }

    public class NonTravelPoint : TravelPoint
    {
        public NonTravelPoint(): base(false, false) {}

        public override TravelPoint WithHorizontalHazard()
        {
            return this;
        }

        public override TravelPoint WithVerticalHazard()
        {
            return this;
        }
    }
}
