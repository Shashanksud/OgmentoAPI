using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using System.Globalization;

namespace OgmentoAPI.Domain.Catalog.Services.Shared
{
	public class CatalogHelper
	{
		public static List<SourceModel> UploadCsvFile<SourceModel, TargetModel>(IFormFile csvFile) where TargetModel : ClassMap<SourceModel>
		{
			CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				Delimiter = typeof(SourceModel) == typeof(UploadPictureModel) ? "," : ";",
				TrimOptions = TrimOptions.Trim,
				BadDataFound = null,
			};

			using (StreamReader csvStreamReader = new StreamReader(csvFile.OpenReadStream()))
			using (CsvReader csvReader = new CsvReader(csvStreamReader, csvConfig))
			{
				csvReader.Context.RegisterClassMap<TargetModel>();
				List<SourceModel> records = csvReader.GetRecords<SourceModel>().ToList();
				return records;
			}
		}
	}
}
