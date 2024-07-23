using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
