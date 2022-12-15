using System.Text.Json.Serialization;

namespace Teos.Autosigner.Model
{
	public class SignedTranasction
	{
		[JsonIgnore]
		public Guid TransactionId { get; set; }

		public string SignerAddress { get; set; }

		public string SignedTransaction { get; set; }

		public string Description { get; set; }
	}
}