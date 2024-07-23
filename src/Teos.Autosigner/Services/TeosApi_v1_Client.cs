using Microsoft.Extensions.Options;
using Teos.Autosigner.Model;

namespace Teos.Autosigner.Services
{
	internal class TeosApi_v1_Client : HttpClientBase
	{
		public TeosApi_v1_Client(IOptions<TeosApiClientOptions> options, ILogger<TeosApi_v1_Client> logger, SocketsHttpHandler socketsHttpHandler)
			: base(new ApiKeyAccessTokenProvider(options.Value.ApiKey), socketsHttpHandler, logger, options.Value)
		{
		}

		public async Task<GetTransactionsResponse> GetUnsignedTransactionsAsync(IEnumerable<string> signerAddresses, IEnumerable<Guid> txsToExclude = null)
		{
			if (signerAddresses?.Any() == false)
			{
				throw new ArgumentException("At least one signer address must be provided", nameof(signerAddresses));
			}

			var addressesStr = string.Join("','", signerAddresses);
			var filter = $"$filter=SignedBy in ('{addressesStr}') and State eq 'Received'";
			if (txsToExclude?.Any() == true)
			{
				var txStr = string.Join("','", txsToExclude);
				var excludeCondition = $"not (Id in ('{txStr}'))";
				filter += $" and {excludeCondition}";
			}
			var orderBy = "$orderby=OnCreated";
			var select = "$select=Id,Type,SignedBy,DataAsJson";
			var path = $"Transactions?{filter}&{orderBy}&{select}";

			var response = await SendRequestAsync<GetTransactionsResponse>(HttpMethod.Get, path);
			response.MoreResults = response.NextPage != null;
			return response;
		}

		public Task<SigningParameters> GetSigningParametersAsync(Guid txId)
		{
			var path = $"Transactions({txId})/GetSigningParameters";
			return SendRequestAsync<SigningParameters>(HttpMethod.Get, path);
		}

		public Task<SubmittedTransaction> SubmitSignedTransactionAsync(SignedTransactionModel signedTransaction)
		{
			var path = $"Transactions({signedTransaction.TransactionId})/Submit";
			return SendRequestAsync<SubmittedTransaction>(HttpMethod.Post, path, signedTransaction);
		}

		public async Task<Asset> GetAssetAsync(string assetId)
		{
			var path = $"Assets('{assetId}')";
			var response = await SendRequestAsync<Asset>(HttpMethod.Get, path);
			return response;
		}
	}
}
