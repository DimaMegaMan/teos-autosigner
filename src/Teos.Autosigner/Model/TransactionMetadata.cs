namespace Teos.Autosigner.Model
{
	public class TransactionMetadata
	{
		public AssetMetadata Asset { get; set; }
	}

	public class AssetMetadata
	{
		public string UniqueAssetId { get; set; }
	}
}
