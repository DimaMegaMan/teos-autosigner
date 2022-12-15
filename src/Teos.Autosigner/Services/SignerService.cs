using Microsoft.Extensions.Options;
using Nethereum.Hex.HexConvertors.Extensions;
using Teos.Autosigner.Model;

namespace Teos.Autosigner.Services
{
	internal class SignerService
	{
		private readonly TeosApi_v09_Client _teosApiClient;
		private readonly ILogger<SignerService> _logger;
		private readonly Dictionary<string, string> _wallets;

		public SignerService(IOptions<SignerServiceOptions> options, TeosApi_v09_Client teosApiClient, ILogger<SignerService> logger)
		{
			_wallets = options.Value.Wallets.ToDictionary(w => w.PublicAddress.ToLower(), w => w.PrivateKey);
			_teosApiClient = teosApiClient;
			_logger = logger;
		}

		public async Task SignPendingTransactionsAsync()
		{
			bool moreResults;
			var failedIds = new List<Guid>();
			do
			{
				var notSignedResponse = await _teosApiClient.GetNotSignedTransactionsAsync(_wallets.Keys, failedIds);
				var notSigned = notSignedResponse.Value;
				foreach (var transaction in notSigned)
				{
					try
					{
						var bcHash = await SingTransactionAsync(transaction);
						_logger.LogInformation("Succesfully submitted transaction (Id: '{txId}', bcHash: '{bcHash}')", transaction.Id, bcHash);
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "An error occured while signing transaction (Id: '{txId)'}", transaction.Id);
						failedIds.Add(transaction.Id);
					}
				}
				moreResults = notSignedResponse.MoreResults;
			}
			while (moreResults);
		}

		private async Task<string> SingTransactionAsync(Transaction transaction)
		{
			var signingParameters = await _teosApiClient.GetSigningParametersAsync(transaction.Id);

			var signedTransaction = new Nethereum.Signer.LegacyTransactionSigner().SignTransaction(
				privateKey: _wallets[transaction.SignedBy].HexToByteArray(),
				to: signingParameters.TargetAddress,
				amount: 0,
				nonce: signingParameters.Nonce,
				gasPrice: signingParameters.GasPrice,
				gasLimit: signingParameters.GasLimit,
				data: signingParameters.DataToSign
			);

			var signedTx = new SignedTranasction
			{
				SignerAddress = transaction.SignedBy,
				TransactionId = transaction.Id,
				SignedTransaction = signedTransaction,
				Description = "Hello, autosigner world"
			};

			var submittedTransaction = await _teosApiClient.SubmitSignedTransactionAsync(signedTx);
			return submittedTransaction.BlockchainTransactionId;
		}
	}
}
