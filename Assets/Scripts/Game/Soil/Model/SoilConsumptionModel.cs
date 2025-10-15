namespace Game.Soil.Model
{
    public class SoilConsumptionModel
    {
        public ElementsModel ElementsUsage { get; }
        public float WaterUsage { get; private set; }

        public SoilConsumptionModel(ElementsModel elementsUsage, float waterUsage)
        {
            ElementsUsage = elementsUsage;
            WaterUsage = waterUsage;
        }

        public SoilConsumptionModel()
        {
            ElementsUsage = new();
            WaterUsage = 0f;
        }

        public void Add(ElementsModel elementsModel, float waterUsage)
        {
            ElementsUsage.Add(elementsModel);
            WaterUsage += waterUsage;
        }
    }
}