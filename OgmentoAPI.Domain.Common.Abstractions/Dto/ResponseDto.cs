namespace OgmentoAPI.Domain.Common.Abstractions.Dto
{
	public class ResponseDto
	{
		public bool IsSuccess {  get; set; }
		public string ErrorMessage { get; set; } = "No Error";
	}
}
