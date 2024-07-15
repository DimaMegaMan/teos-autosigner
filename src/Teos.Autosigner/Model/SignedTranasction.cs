using System.Text.Json.Serialization;

namespace Teos.Autosigner.Model
{
	public class SignedTransaction
	{
		[JsonIgnore]
		public Guid TransactionId { get; set; }

		public string SignerAddress { get; set; }

		public string SignedTransactionId { get; set; }

		public string Description { get; set; }
	}
}