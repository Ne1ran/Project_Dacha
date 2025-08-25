using Game.Plants.Model;

namespace Game.Diseases.Model
{
    public class SavedDiseaseModel
    {
        public string Id { get; }
        public PlantFamilyType PlantFamilyType { get; }
        public int CropRotationNeeded { get; set; }
        public int RemoveDaysNeeded { get; set; }

        public SavedDiseaseModel(string id, PlantFamilyType plantFamilyType, int cropRotationNeeded, int removeDaysNeeded)
        {
            Id = id;
            PlantFamilyType = plantFamilyType;
            CropRotationNeeded = cropRotationNeeded;
            RemoveDaysNeeded = removeDaysNeeded;
        }
    }
}