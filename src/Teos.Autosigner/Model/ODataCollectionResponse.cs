using System.Text.Json.Serialization;

namespace Teos.Autosigner.Model
{
	public class ODataCollectionResponse<T>
	{
		[JsonPropertyName("@odata.nextLink")]
		public string NextPage { get; set; }

		public bool MoreResults { get; set; }

		public T[] Value { get; set; }
	}
}