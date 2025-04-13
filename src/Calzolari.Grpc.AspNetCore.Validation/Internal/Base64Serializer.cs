using System;
using System.Text;
using System.Text.Json;

namespace Calzolari.Grpc.AspNetCore.Validation.Internal;

public static class Base64Serializer
{
    public static string ToBase64(this object obj)
    {
        string json = JsonSerializer.Serialize(obj);

        byte[] bytes = Encoding.Default.GetBytes(json);

        return Convert.ToBase64String(bytes);
    }
}