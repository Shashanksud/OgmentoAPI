using OgmentoAPI.Domain.Common.Abstractions.Models;

namespace OgmentoAPI.Domain.Common.Abstractions.Dto
{
	public static class PictureDtoMapping
	{
		public static PictureDto ToDto(this PictureModel picture)
		{
			return new PictureDto()
			{
				Base64Encoded = Convert.ToBase64String(picture.BinaryData),
				FileName = picture.FileName,
				MimeType = picture.MimeType,
				Hash = picture.Hash,
				ToBeDeleted = picture.ToBeDeleted,
				IsNew = picture.IsNew,
			};
		}
		public static PictureModel ToModel(this PictureDto picture)
		{
			return new PictureModel()
			{
				BinaryData = Convert.FromBase64String(picture.Base64Encoded),
				FileName = picture.FileName,
				MimeType = picture.MimeType,
				Hash = picture.Hash,
				ToBeDeleted= picture.ToBeDeleted,
				IsNew= picture.IsNew,
			};
		}
	}
}
