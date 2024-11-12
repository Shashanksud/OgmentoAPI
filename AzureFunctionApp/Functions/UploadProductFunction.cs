using AzureFunctionApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
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

		[Function("upload-product")]
		public async Task ProcessProductMessage([QueueTrigger("%QueueName%", Connection = "AzureWebJobsStorage")] string message, FunctionContext context)
		{
			ILogger logger = context.GetLogger("UploadProductFunction");
			logger.LogInformation($"Queue trigger function processed: {message}");
			try
			{
				logger.LogInformation("deserializing message.");
				JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions(){ PropertyNameCaseInsensitive= true};
				ProductUploadMessage product = JsonSerializer.Deserialize<ProductUploadMessage>(message, jsonSerializerOptions);
				logger.LogInformation("deserializing message completed.");
				if (string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiryTime)
				{
					logger.LogInformation("token is null or empty or expired");
					await RefreshAuthTokenAsync();
					logger.LogInformation("token acquired");
				}
				logger.LogInformation("sending request to SaveProductUpload Api");
				await UploadProductAsync(product, _authToken);
				logger.LogInformation("products uploaded successfully");
			}
			catch (Exception ex)
			{
				logger.LogError($"Error processing message: {ex.Message}");
				logger.LogInformation(ex.ToString());
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
			TokenModel apiResponse = JsonSerializer.Deserialize<TokenModel>(responseContent);
			_authToken = apiResponse.token;
			_tokenExpiryTime = DateTime.UtcNow.AddMinutes(45);
		}

		private async Task UploadProductAsync(ProductUploadMessage product, string authToken)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
			string jsonString = JsonSerializer.Serialize(product);
			StringContent content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
			string uploadProductUrl = _configuration["UploadProductUrl"];
			HttpResponseMessage response = await _httpClient.PostAsync(uploadProductUrl, content);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				await RefreshAuthTokenAsync();
				_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
				response = await _httpClient.PostAsync(uploadProductUrl, content);
			}
			response.EnsureSuccessStatusCode();
		}
	}
}
