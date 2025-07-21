namespace Core.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(string serialized);
        string Serialize<T>(T serialized);
    }
}