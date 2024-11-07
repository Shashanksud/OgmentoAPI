using Azure.Storage.Queues.Models;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Services
{
	public interface IAzureQueueService
	{
		Task AddMessageAsync(string message);
		Task<QueueMessage[]> GetMessagesAsync();
		Task DeleteMessageAsync(QueueMessage message);
	}
}
