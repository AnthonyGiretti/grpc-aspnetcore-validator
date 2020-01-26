using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Calzolari.Grpc.AspNetCore.FluentValidation.Internal
{
    internal static class BinarySerializer
    {
        public static byte[] ToBytes<T>(this T objectToSerialize)
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, objectToSerialize);

            return mStream.ToArray();
        }
    }
}