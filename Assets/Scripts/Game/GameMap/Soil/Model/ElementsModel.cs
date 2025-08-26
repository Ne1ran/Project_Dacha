namespace Game.GameMap.Soil.Model
{
    public class ElementsModel
    {
        public float Nitrogen { get; set; }
        public float Potassium { get; set; }
        public float Phosphorus { get; set; }

        public ElementsModel(float nitrogen, float potassium, float phosphorus)
        {
            Nitrogen = nitrogen;
            Potassium = potassium;
            Phosphorus = phosphorus;
        }

        public void Add(ElementsModel elementsModel)
        {
            Nitrogen += elementsModel.Nitrogen;
            Potassium += elementsModel.Potassium;
            Phosphorus += elementsModel.Phosphorus;
        }

        public void Subtract(ElementsModel elementsModel)
        {
            Nitrogen -= elementsModel.Nitrogen;
            Potassium -= elementsModel.Potassium;
            Phosphorus -= elementsModel.Phosphorus;
        }

        public bool HasEnoughElements(ElementsModel elementsModel)
        {
            return !(Phosphorus < elementsModel.Phosphorus) && !(Nitrogen < elementsModel.Nitrogen) && !(Potassium < elementsModel.Potassium);
        }
    }
}