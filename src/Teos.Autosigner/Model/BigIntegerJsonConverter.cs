using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Teos.Autosigner.Model
{
	public class BigIntegerJsonConverter : JsonConverter<BigInteger>
	{
		public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return reader.TokenType switch
			{
				JsonTokenType.String => BigInteger.Parse(reader.GetString()),
				_ => throw new Exception()
			};
		}

		public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
