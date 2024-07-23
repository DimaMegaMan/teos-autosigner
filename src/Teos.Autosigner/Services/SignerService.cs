using Microsoft.Extensions.Options;
using Nethereum.Hex.HexConvertors.Extensions;
using Teos.Autosigner.Model;

namespace Teos.Autosigner.Services
{
	internal class SignerService
	{
		private readonly TeosApi_v1_Client _teosApiClient;
		private readonly Dictionary<string, string> _wallets;
		private readonly ValidationService _validationService;
		private readonly ILogger<SignerService> _logger;


		public SignerService(TeosApi_v1_Client teosApiClient, IOptions<SignerServiceOptions> options, ValidationService validationService, ILogger<SignerService> logger)
		{
			_teosApiClient = teosApiClient;
			_wallets = options.Value.Wallets.ToDictionary(w => w.PublicAddress.ToLower(), w => w.PrivateKey);
			_validationService = validationService;
			_logger = logger;
		}

		public async Task SignPendingTransactionsAsync()
		{
			bool moreResults;
			var failedIds = new List<Guid>();
			do
			{
				var getUnsignedTxResponse = await _teosApiClient.GetUnsignedTransactionsAsync(_wallets.Keys, failedIds);
				var unsignedTransactions = getUnsignedTxResponse.Value;
				foreach (var transaction in unsignedTransactions)
				{
					try
					{
						var validationResult = await _validationService.ValidateTransactionAsync(transaction);
						if (!validationResult.Success)
						{
							_logger.LogInformation(validationResult.ErrorMessage);
							continue;
						}

						var bcHash = await SingTransactionAsync(transaction);
						_logger.LogInformation("Successfully submitted transaction (Id: '{txId}', bcHash: '{bcHash}')", transaction.Id, bcHash);
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "An error occurred while signing transaction (Id: '{txId)'}", transaction.Id);
						failedIds.Add(transaction.Id);
					}
				}
				moreResults = getUnsignedTxResponse.MoreResults;
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

			var signedTx = new SignedTransaction
			{
				SignerAddress = transaction.SignedBy,
				TransactionId = transaction.Id,
				SignedTransactionId = signedTransaction,
				Description = "Hello, autosigner world"
			};

			var submittedTransaction = await _teosApiClient.SubmitSignedTransactionAsync(signedTx);
			return submittedTransaction.BlockchainTransactionId;
		}
	}
}
