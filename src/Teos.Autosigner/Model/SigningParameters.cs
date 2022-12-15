using System.Numerics;

namespace Teos.Autosigner.Model
{
	public class SigningParameters
	{
		public string TargetAddress { get; set; }

		public BigInteger Nonce { get; set; }

		public BigInteger GasPrice { get; set; }

		public BigInteger GasLimit { get; set; }

		public string DataToSign { get; set; }
	}
}