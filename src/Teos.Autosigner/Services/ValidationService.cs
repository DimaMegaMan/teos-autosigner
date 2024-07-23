using System.Text.Json;
using Teos.Autosigner.Model;

namespace Teos.Autosigner.Services
{
	internal class ValidationService
	{
		private readonly TeosApi_v1_Client _teosApiClient;

		public ValidationService(TeosApi_v1_Client teosApiClient)
		{
			_teosApiClient = teosApiClient;
		}

		public async Task<ValidationResult> ValidateTransactionAsync(Transaction transaction)
		{
			return transaction.Type switch
			{
				TransactionType.ActivateAsset => ValidationResult.SuccessResult,

				TransactionType.CreateController => ValidationResult.SuccessResult,
				TransactionType.ChangeAssetController => ValidationResult.SuccessResult,
				TransactionType.DeleteAssetController => ValidationResult.SuccessResult,

				TransactionType.CreateSupply => ValidationResult.SuccessResult,
				TransactionType.ChangeSupplyRate => ValidationResult.SuccessResult,
				TransactionType.RestockSupply => ValidationResult.SuccessResult,
				TransactionType.ExtendSupply => ValidationResult.SuccessResult,
				TransactionType.DeleteSupply => ValidationResult.SuccessResult,

				TransactionType.SetWhitelistAddresses => ValidationResult.SuccessResult,
				TransactionType.RemoveWhitelistAddresses => ValidationResult.SuccessResult,
				TransactionType.SetBlacklistAddresses => ValidationResult.SuccessResult,
				TransactionType.RemoveBlacklistAddresses => ValidationResult.SuccessResult,

				TransactionType.CreateTokens => await RequireAssetController(transaction),
				TransactionType.TransferTokens => await RequireAssetController(transaction),
				TransactionType.DestroyTokens => await RequireAssetController(transaction),
				_ => ValidationResult.Failure($"The transaction type '{transaction.Type}' is not supported")
			};
		}

		public async Task<ValidationResult> RequireAssetController(Transaction transaction)
		{
			var metadata = JsonSerializer.Deserialize<TransactionMetadata>(transaction.DataAsJson);
			var asset = await _teosApiClient.GetAssetAsync(metadata.Asset.UniqueAssetId);
			var controller = asset.ControllerAddress;

			if (controller is null)
			{
				return ValidationResult.Failure($"The transaction type '{transaction.Type}' requires the associated asset to have a controller. Asset ID: {asset.Id}");
			}

			return ValidationResult.SuccessResult;
		}
	}
}
