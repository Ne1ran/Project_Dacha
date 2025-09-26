namespace Game.GameMap.Soil.Model
{
    public class SoilConsumptionModel
    {
        public ElementsModel ElementsUsage { get; }
        public float WaterUsage { get; }
        public float HumusUsage { get; }
        public bool HasEnoughWater { get; }
        public bool HasEnoughNitrogen { get; }
        public bool HasEnoughPotassium { get; }
        public bool HasEnoughPhosphorus { get; }

        public SoilConsumptionModel(ElementsModel elementsUsage,
                                    float waterUsage,
                                    float humusUsage,
                                    bool hasEnoughWater,
                                    bool hasEnoughNitrogen,
                                    bool hasEnoughPotassium,
                                    bool hasEnoughPhosphorus)
        {
            ElementsUsage = elementsUsage;
            WaterUsage = waterUsage;
            HumusUsage = humusUsage;
            HasEnoughWater = hasEnoughWater;
            HasEnoughNitrogen = hasEnoughNitrogen;
            HasEnoughPotassium = hasEnoughPotassium;
            HasEnoughPhosphorus = hasEnoughPhosphorus;
        }

        public bool HasEverything()
        {
            return HasEnoughWater && HasEnoughNitrogen && HasEnoughPhosphorus && HasEnoughPotassium;
        }

        public bool HasNothing()
        {
            return !HasEnoughWater || !HasEnoughNitrogen || !HasEnoughPhosphorus || !HasEnoughPotassium;
        }
    }
}