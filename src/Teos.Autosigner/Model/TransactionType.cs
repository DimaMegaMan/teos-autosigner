using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teos.Autosigner.Model
{
	public enum TransactionType
	{
		ActivateAsset = 1,
		CreateTokens = 2,
		DestroyTokens = 3,
		TransferTokens = 4,
		CreateSupply = 5,
		DeleteSupply = 6,
		RestockSupply = 7,
		ChangeSupplyRate = 8,
		ExtendSupply = 9,
		RunTrade = 10,
		CreateController = 12,
		ChangeAssetController = 15,
		DeleteAssetController = 37,
		AssetAddLink = 38,
		AssetDeleteLink = 39,
		AssetAddLinks = 43,
		AssetDeleteLinks = 44,
		SetWhitelistAddresses = 45,
		RemoveWhitelistAddresses = 46,
		SetBlacklistAddresses = 47,
		RemoveBlacklistAddresses = 48
	}
}
