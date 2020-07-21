using System;

namespace HanumanInstitute.CommonWeb
{
    public interface ISerializationService
    {
        string Serialize<T>(T dataToSerialize);
        T Deserialize<T>(string xmlText) where T : class, new();
        T DeserializeFromFile<T>(string path);
        void SerializeToFile<T>(T dataToSerialize, string path);
    }
}
