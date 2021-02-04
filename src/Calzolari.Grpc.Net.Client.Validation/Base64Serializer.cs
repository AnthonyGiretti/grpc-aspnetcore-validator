using System;
using System.Text;
using System.Text.Json;

namespace Calzolari.Grpc.Net.Client.Validation
{
    public static class Base64Serializer
    {
        public static T FromBase64<T>(this string base64Text)
        {
            byte[] bytes = Convert.FromBase64String(base64Text);

            string json = Encoding.Default.GetString(bytes);

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}