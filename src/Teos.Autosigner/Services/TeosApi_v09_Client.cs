using Microsoft.Extensions.Options;
using Teos.Autosigner.Model;

namespace Teos.Autosigner.Services
{
	internal class TeosApi_v09_Client : HttpClientBase
	{
		public TeosApi_v09_Client(IOptions<TeosApiClientOptions> options, ILogger<TeosApi_v09_Client> logger, SocketsHttpHandler socketsHttpHandler)
			: base(new ApiKeyAccessTokenProvider(options.Value.ApiKey), socketsHttpHandler, logger, options.Value)
		{
		}

		public async Task<GetTransactionsResponse> GetNotSignedTransactionsAsync(IEnumerable<string> signerAddresses, IEnumerable<Guid> txsToExclude = null)
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
			var select = "$select=Id,SignedBy";
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

		public Task<SubmittedTransaction> SubmitSignedTransactionAsync(SignedTranasction signedTranasction)
		{
			var path = $"Transactions({signedTranasction.TransactionId})/Submit";
			return SendRequestAsync<SubmittedTransaction>(HttpMethod.Post, path, signedTranasction);
		}
	}
}
