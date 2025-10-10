namespace Game.Soil.Model
{
    public class SoilConsumptionModel
    {
        public ElementsModel ElementsUsage { get; }
        public float WaterUsage { get; }
        public float HumusUsage { get; }

        public SoilConsumptionModel(ElementsModel elementsUsage,
                                    float waterUsage,
                                    float humusUsage)
        {
            ElementsUsage = elementsUsage;
            WaterUsage = waterUsage;
            HumusUsage = humusUsage;
        }

        public SoilConsumptionModel()
        {
            ElementsUsage = new();
            WaterUsage = 0f;
            HumusUsage = 0f;
        }
    }
}