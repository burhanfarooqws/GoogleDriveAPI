using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Polly;

namespace GoogleDrivePollyWeb
{
    
    public class GoogleDriveApi
    {
		private readonly HttpClient _client;
		private readonly IConfiguration _configuration;

		public GoogleDriveApi(HttpClient client, IConfiguration configuration)
		{
			_client = client;
			_configuration = configuration;

			_client.BaseAddress = new Uri("https://www.googleapis.com/");
		}

		public async Task<ListFilesResponse> ListFiles(string accessToken, string refreshToken, Func<string, Task> tokenRefreshed)
		{
			var policy = CreateTokenRefreshPolicy(tokenRefreshed);

			var response = await policy.ExecuteAsync(context =>
			{
				var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/drive/v3/files");
				requestMessage.Headers.Add("Authorization", $"Bearer {context["access_token"]}");

				return _client.SendAsync(requestMessage);
			}, new Dictionary<string, object>
			{
				{"access_token", accessToken},
				{"refresh_token", refreshToken}
			});

			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsAsync<ListFilesResponse>();
		}

		private AsyncRetryPolicy<HttpResponseMessage> CreateTokenRefreshPolicy(Func<string, Task> tokenRefreshed)
		{
			var policy = Policy
				.HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.Unauthorized)
				.RetryAsync(1, async (result, retryCount, context) =>
				{
					if (context.ContainsKey("refresh_token"))
					{
						var newAccessToken = await RefreshAccessToken(context["refresh_token"].ToString());
						if (newAccessToken != null)
						{
							await tokenRefreshed(newAccessToken);

							context["access_token"] = newAccessToken;
						}
					}
				});

			return policy;
		}

		private async Task<string> RefreshAccessToken(string refreshToken)
		{
			var refreshMessage = new HttpRequestMessage(HttpMethod.Post, "/oauth2/v4/token")
			{
				Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
				{
					new KeyValuePair<string, string>("client_id", "999739170261-aooqgvcuefpig9dk0onkl8ji42eqbp4b.apps.googleusercontent.com"),
					new KeyValuePair<string, string>("client_secret", "zal61wydSnolVILsz5FsWmGB"),
					new KeyValuePair<string, string>("refresh_token", refreshToken),
					new KeyValuePair<string, string>("grant_type", "refresh_token")
				})
			};

			var response = await _client.SendAsync(refreshMessage);

			if (response.IsSuccessStatusCode)
			{
				var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();

				return tokenResponse.AccessToken;
			}

			// return null if we cannot request a new token
			return null;
		}


	}

    public class File
    {
	    public string kind { get; set; }
	    public string id { get; set; }
	    public string name { get; set; }
	    public string mimeType { get; set; }
    }

    public class ListFilesResponse
	{
	    public string kind { get; set; }
	    public bool incompleteSearch { get; set; }
	    public List<File> files { get; set; }
    }

}
