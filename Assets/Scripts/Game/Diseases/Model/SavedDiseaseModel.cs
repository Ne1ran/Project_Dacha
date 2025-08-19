namespace Game.Diseases.Model
{
    public class SavedDiseaseModel
    {
        public string Id { get; }
        public int CropRotationNumber { get; set; }
        public int DaysPassed { get; set; }

        public SavedDiseaseModel(string id, int cropRotationNumber, int daysPassed)
        {
            Id = id;
            CropRotationNumber = cropRotationNumber;
            DaysPassed = daysPassed;
        }
    }
}