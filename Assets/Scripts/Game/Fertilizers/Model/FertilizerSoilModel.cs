namespace Game.Fertilizers.Model
{
    public class FertilizerSoilModel
    {
        public float Mass { get; }
        public float PhChange { get; }
        public float BreathabilityChange { get; }
        public float HumusMass { get; }
        public float NitrogenMass { get; }
        public float PotassiumMass { get; }
        public float PhosphorusMass { get; }

        public FertilizerSoilModel(float mass,
                                   float phChange,
                                   float breathabilityChange,
                                   float humusMass,
                                   float nitrogenMass,
                                   float potassiumMass,
                                   float phosphorusMass)
        {
            Mass = mass;
            PhChange = phChange;
            BreathabilityChange = breathabilityChange;
            HumusMass = humusMass;
            NitrogenMass = nitrogenMass;
            PotassiumMass = potassiumMass;
            PhosphorusMass = phosphorusMass;
        }
    }
}