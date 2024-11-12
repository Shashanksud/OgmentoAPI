using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Options;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Common.Abstractions;
namespace OgmentoAPI.Domain.Catalog.Services
{
	public class AzureQueueService: IAzureQueueService
	{
		private readonly QueueClient _queueClient;
		public AzureQueueService(QueueClient queueClient) {
			_queueClient = queueClient;
			_queueClient.CreateIfNotExists();
		}
		public async Task AddMessageAsync(string message)
		{
			if (_queueClient.Exists())
			{
				await _queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message)));
			}
		}

		public async Task<QueueMessage[]> GetMessagesAsync()
		{
			if (_queueClient.Exists())
			{
				QueueMessage[] messages = await _queueClient.ReceiveMessagesAsync();
				return messages;
			}
			return Array.Empty<QueueMessage>();
		}

		public async Task DeleteMessageAsync(QueueMessage message)
		{
			if (_queueClient.Exists())
			{
				await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
			}
		}
	}
}
