using System.Net.Http.Headers;
using System.Text.Json;
using Teos.Autosigner.Model;

namespace Teos.Autosigner.Services
{
	public abstract class HttpClientBase
	{
		private static JsonSerializerOptions _jsonSerializerOptions;

		static HttpClientBase()
		{
			_jsonSerializerOptions = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
			};
			_jsonSerializerOptions.Converters.Add(new BigIntegerJsonConverter());
		}

		private readonly IAccessTokenProvider _accessTokenProvider;
		private readonly SocketsHttpHandler _httpHandler;
		private readonly ILogger _logger;
		private readonly HttpClientOptions _options;

		protected HttpClientBase(IAccessTokenProvider accessTokenProvider, SocketsHttpHandler httpHandler, ILogger logger, HttpClientOptions options)
		{
			_accessTokenProvider = accessTokenProvider;
			_httpHandler = httpHandler;
			_logger = logger;
			_options = options;
		}

		protected async Task SendRequestAsync(HttpMethod httpMethod, string path, object payload = null)
		{
			await SendRequestInternalAsync(httpMethod, path, payload);
		}

		protected async Task<TResp> SendRequestAsync<TResp>(HttpMethod httpMethod, string path, object payload = null)
		{
			var respBody = await SendRequestInternalAsync(httpMethod, path, payload);
			var result = JsonSerializer.Deserialize<TResp>(respBody, _jsonSerializerOptions);
			return result;
		}

		private async Task<string> SendRequestInternalAsync(HttpMethod httpMethod, string path, object payload = null)
		{
			var request = BuildHttpRequestMessage(httpMethod, path, payload);
			using var requestMessage = request.requestMessage;
			using var httpClient = await BuildHttpClient();
			var resp = await httpClient.SendAsync(requestMessage);
			var respBody = await resp.Content.ReadAsStringAsync();

			if (!resp.IsSuccessStatusCode)
			{
				_logger.LogError("Status code: {statusCode}, url: {url},\nrequest body: {reqBbody},\nresponse body: {respBbody}",
					resp.StatusCode, resp.RequestMessage.RequestUri, request.jsonContent, respBody);
				throw new Exception("Error occured while executing http-request");
			}

			return respBody;
		}

		private async Task<HttpClient> BuildHttpClient()
		{
			const string AUTH_SCHEME_BEARER = "Bearer";

			var httpClient = new HttpClient(_httpHandler, false);
			httpClient.BaseAddress = new Uri(_options.ApiUrl);
			var accessToken = await _accessTokenProvider.GetAccessTokenAsync();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AUTH_SCHEME_BEARER, accessToken);

			return httpClient;
		}

		private static (HttpRequestMessage requestMessage, string jsonContent) BuildHttpRequestMessage(HttpMethod httpMethod, string path, object payload = null)
		{
			var req = new HttpRequestMessage();
			req.RequestUri = new Uri(path, UriKind.Relative);
			req.Method = httpMethod;

			string json = null;
			if (payload != null)
			{
				json = JsonSerializer.Serialize(payload);
				var content = new StringContent(json);
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				req.Content = content;
			}

			return (req, json);
		}
	}
}
