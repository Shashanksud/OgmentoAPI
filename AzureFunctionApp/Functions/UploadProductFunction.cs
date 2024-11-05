using AzureFunctionApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AzureFunctionApp.Functions
{
	public class UploadProductFunction
	{
		private readonly HttpClient _httpClient = new HttpClient();
		private static DateTime _tokenExpiryTime;
		private readonly IConfiguration _configuration;
		private static string _authToken;
		public UploadProductFunction(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[Function("Upload-Product")]
		public async Task ProcessProductMessage([QueueTrigger("%QueueName%", Connection = "AzureWebJobsStorage")] string message, FunctionContext context)
		{
			ILogger logger = context.GetLogger("UploadProductFunction");
			logger.LogInformation($"Queue trigger function processed: {message}");
			try
			{
				UploadProductModel product = JsonSerializer.Deserialize<UploadProductModel>(message);

				if (string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiryTime)
				{
					await RefreshAuthTokenAsync();
				}
				await UploadProductAsync(product, _authToken);
			}
			catch (Exception ex)
			{
				logger.LogError($"Error processing message: {ex.Message}");
			}

		}
		private async Task RefreshAuthTokenAsync()
		{
			LoginModel loginDto = new LoginModel { Email = _configuration["ApiCredentials:Email"], Password = _configuration["ApiCredentials:Password"] };
			StringContent content = new StringContent(JsonSerializer.Serialize(loginDto), System.Text.Encoding.UTF8, "application/json");

			string loginUrl = _configuration["LoginUrl"];
			HttpResponseMessage response = await _httpClient.PostAsync(loginUrl, content);
			response.EnsureSuccessStatusCode();

			string responseContent = await response.Content.ReadAsStringAsync();
			TokenModel tokenModel = JsonSerializer.Deserialize<TokenModel>(responseContent);

			_authToken = tokenModel.Token;
			_tokenExpiryTime = DateTime.UtcNow.AddMinutes(45);
		}

		private async Task UploadProductAsync(UploadProductModel product, string authToken)
		{
			StringContent content = new StringContent(JsonSerializer.Serialize(product), System.Text.Encoding.UTF8, "application/json");
			_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

			string uploadProductUrl = _configuration["UploadProductUrl"];
			HttpResponseMessage response = await _httpClient.PostAsync(uploadProductUrl, content);
			response.EnsureSuccessStatusCode();
		}
	}
}
