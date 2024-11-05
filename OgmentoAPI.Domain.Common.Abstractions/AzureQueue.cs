namespace OgmentoAPI.Domain.Common.Abstractions
{
	public class AzureQueue
	{
		public string ConnectionString {  get; set; }
		public string QueueName { get; set; }
	}
}
