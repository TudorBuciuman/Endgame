using System;
using FullSerializer;

public static class Serializer
{
    private static readonly fsSerializer _serializer = new fsSerializer();

    public static string Serialize(Type type, object value)
    {
        _serializer.TrySerialize(type, value, out var data).AssertSuccessWithoutWarnings();
        return fsJsonPrinter.PrettyJson(data);
    }

    public static object Deserialize(Type type, string serializedState)
    {
        fsData data = fsJsonParser.Parse(serializedState);
        object result = null;
        _serializer.TryDeserialize(data, type, ref result).AssertSuccessWithoutWarnings();
        return result;
    }
}
