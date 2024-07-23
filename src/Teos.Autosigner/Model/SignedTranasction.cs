using System.Text.Json.Serialization;

namespace Teos.Autosigner.Model
{
	public class SignedTransactionModel
	{
		[JsonIgnore]
		public Guid TransactionId { get; set; }

		public string SignerAddress { get; set; }

		public string SignedTransaction { get; set; }

		public string Description { get; set; }
	}
}