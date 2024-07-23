using Nethereum.Signer;

namespace Teos.Autosigner.Model
{
	public class Transaction
	{
		public Guid Id { get; set; }

		public string SignedBy { get; set; }
		public TransactionType Type { get; set; }
		public string DataAsJson { get; set; }
	}
}