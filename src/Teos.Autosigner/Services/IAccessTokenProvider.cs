namespace Teos.Autosigner.Services
{
	public interface IAccessTokenProvider
	{
		Task<string> GetAccessTokenAsync();
	}
}
