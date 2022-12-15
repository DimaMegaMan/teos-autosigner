namespace Teos.Autosigner.Services
{
	public class ApiKeyAccessTokenProvider : IAccessTokenProvider
	{
		private readonly string _apiKey;

		public ApiKeyAccessTokenProvider(string apiKey)
		{
			_apiKey = apiKey;
		}

		public Task<string> GetAccessTokenAsync()
		{
			return Task.FromResult(_apiKey);
		}
	}
}
