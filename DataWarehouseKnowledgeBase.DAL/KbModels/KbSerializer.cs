using System.IO;
using System.Xml.Serialization;

namespace DataWarehouseKnowledgeBase.DAL.KbModels
{
    public class KbSerializer<T> : IKbSerializer<T>
    {
        private readonly string _filename;
        private readonly XmlSerializer _serializer;

        public KbSerializer(string filename)
        {
            _filename = filename;
            _serializer = new XmlSerializer(typeof(T));
        }

        public void Serialize(T obj)
        {
            using (var stream = new FileStream(_filename, FileMode.OpenOrCreate))
            {
                _serializer.Serialize(stream, obj);
            }
        }

        public T Deserialize()
        {
            using (var stream = new FileStream(_filename, FileMode.OpenOrCreate))
            {
                return (T)_serializer.Deserialize(stream);
            }
        }
    }
}
