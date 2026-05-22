
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonEntytyConverter : JsonConverter<Entyty>
{
    private static readonly JsonSerializer _entytyInitialSerializer = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
    public override Entyty ReadJson(JsonReader reader, Type objectType, Entyty existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var JsonObject = JObject.Load(reader);
        var tipe = JsonObject["Type"].ToObject<EntytyType>();

        return tipe switch
        {
            EntytyType.Door => JsonObject.ToObject<JsonDoor>(_entytyInitialSerializer),
            EntytyType.Rock => JsonObject.ToObject<JsonRock>(_entytyInitialSerializer),
            EntytyType.Character => JsonObject.ToObject<JsonCharacter>(_entytyInitialSerializer),
            _ => throw new NotImplementedException()
        };
    }

    public override void WriteJson(JsonWriter writer, Entyty value, JsonSerializer serializer)
    {
        _entytyInitialSerializer.Serialize(writer, value);
    }
}

public static class JsonProgectSeting
{
    public static void ApplayProgectSerializeationSeting()
    {
        var setting = new JsonSerializerSettings();
        setting.Converters.Add(new JsonEntytyConverter());

        JsonConvert.DefaultSettings = () => setting;
    }
}