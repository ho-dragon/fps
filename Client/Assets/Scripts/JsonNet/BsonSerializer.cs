using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public static class BsonSerializer {
    public static T Deserialize<T>(byte[] data) {
        MemoryStream ms = new MemoryStream(data);
        ms.Seek(0, SeekOrigin.Begin);
        byte[] byteBSON = ms.ToArray();
        JsonSerializer deserializaer = new JsonSerializer();
        BsonReader reader = new BsonReader(ms);
        T result = deserializaer.Deserialize<T>(reader);
        //Logger.Debug("[-------Json Viewer------]" + JsonConvert.SerializeObject(result));//for test
        return result;
    }

    public static byte[] SerializeToByte(object data) {
        MemoryStream ms = new MemoryStream();
        JsonSerializer serializer = new JsonSerializer();
        BsonWriter writer = new BsonWriter(ms);
        serializer.Serialize(writer, data);
        ms.Seek(0, SeekOrigin.Begin);
        return ms.ToArray();
    }
}
