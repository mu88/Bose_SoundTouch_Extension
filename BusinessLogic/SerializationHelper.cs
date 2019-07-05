using System.IO;
using System.Xml.Serialization;

namespace BusinessLogic
{
    public static class SerializationHelper
    {
        public static T Deserialize<T>(string s)
        {
            T result;

            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(s))
            {
                result = (T)serializer.Deserialize(reader);
            }

            return result;
        }

        public static string Serialize(object o)
        {
            var serializer = new XmlSerializer(o.GetType());
            using(var textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, o);
                return textWriter.ToString();
            }
        }
    }
}