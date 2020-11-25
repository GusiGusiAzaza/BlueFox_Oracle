using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace BlueFoxTests_Oracle.Components
{
    public static class CustomSerializer
    {
        public static void BinSerialize(object obj, string path)
        {
            FileSystemManager.CheckPathValidity(path.Remove(path.LastIndexOf('\\')));
            var formatter = new BinaryFormatter();
            using var stream = new FileStream(path, FileMode.OpenOrCreate);
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        public static object BinDeserialize(string path)
        {
            FileSystemManager.CheckPathValidity(path);
            using var openFileStream = File.OpenRead(path);
            var deserializer = new BinaryFormatter();
            var obj = deserializer.Deserialize(openFileStream);
            openFileStream.Close();
            return obj;
        }

        public static void NewtonsoftSerialize(object obj, string path)
        {
            FileSystemManager.CheckPathValidity(path.Remove(path.LastIndexOf('\\')));
            File.WriteAllText(path, JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
        }

        public static object NewtonsoftDeserialize<T>(string path)
        {
            FileSystemManager.CheckPathValidity(path);
            var obj = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return obj;
        }
    }
}
