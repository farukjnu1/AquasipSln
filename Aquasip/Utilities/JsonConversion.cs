namespace Aquasip.Utilities
{
    public static class JsonConversion
    {
        public static T? DeserializeObject<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public static string SerializeObject<T>(T obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }
    }
}
