using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Calzolari.Grpc.Net.Client.Validation
{
    internal static class BinarySerializer
    {
        public static T FromBytes<T>(this byte[] arrayOfBytes) where T : class
        {
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();

            stream.Write(arrayOfBytes, 0, arrayOfBytes.Length);
            stream.Position = 0;

            return formatter.Deserialize(stream) as T;
        }
    }
}