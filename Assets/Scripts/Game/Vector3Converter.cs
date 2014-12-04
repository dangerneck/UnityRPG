using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class Vector3Converter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (value == null) return;
		
		Vector3 v = (Vector3)value;
		
		if (v == new Vector3())
		{
			writer.WriteNull();
			return;
		}
		
		writer.WriteStartObject();
		
		writer.WritePropertyName("x");
		serializer.Serialize(writer, v.x);
		
		writer.WritePropertyName("y");
		serializer.Serialize(writer, v.y);
		
		writer.WritePropertyName("z");
		serializer.Serialize(writer, v.z);
		
		writer.WriteEndObject();
	}
	
	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.None) return null;
		
		reader.Read(); // Property X        
		reader.Read(); // Value X
		var x = (Single)serializer.Deserialize(reader, typeof(Single));
		
		reader.Read(); // Property Y
		reader.Read(); // Value Y
		var y = (Single)serializer.Deserialize(reader, typeof(Single));
		
		reader.Read(); // Property Z
		reader.Read(); // Value Z
		var z = (Single)serializer.Deserialize(reader, typeof(Single));
		
		reader.Read();
		
		return new Vector3(x, y, z);
	}
	
	public override bool CanConvert(Type objectType)
	{
		return (objectType.Equals(typeof(Vector3)));
	}
}