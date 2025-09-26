namespace Game.Plants.Descriptors
{
    public interface IPlantParameters
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public float MinPreferred { get; set; }
        public float MaxPreferred { get; set; }
        public float DamagePerDeviation { get; set; }
        public float GrowBuff { get; set; }
        public float StressGain { get; set; }
    }
}