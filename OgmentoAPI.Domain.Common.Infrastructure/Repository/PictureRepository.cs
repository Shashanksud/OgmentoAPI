﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;
using OgmentoAPI.Domain.Common.Abstractions.DataContext;
using OgmentoAPI.Domain.Common.Abstractions.Models;
using OgmentoAPI.Domain.Common.Abstractions.Repository;
using System.Security.Cryptography;


namespace OgmentoAPI.Domain.Common.Infrastructure.Repository
{
	public class PictureRepository : IPictureRepository
	{
		private readonly CommonDBContext _dbContext;
		public PictureRepository(CommonDBContext dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task<int> GetPictureId(string hash)
		{
			PictureBinary pictureBinary = await _dbContext.PictureBinary.FirstOrDefaultAsync(x => x.Hash == hash);
			if (pictureBinary == null) {
				throw new EntityNotFoundException("Picture not found.");
			}
			return pictureBinary.PictureId;
		}
		public async Task<List<PictureModel>> GetPictures(List<int> pictureIds)
		{
			List<Picture> pictures = _dbContext.Picture.Where(x=>pictureIds.Contains(x.PictureID)).ToList();
			return pictures.Select(x => new PictureModel
			{
				PictureId = x.PictureID,
				FileName = x.FileName,
				MimeType = x.MimeType,
				Hash = _dbContext.PictureBinary.Single(b => b.PictureId==x.PictureID).Hash,
				BinaryData = _dbContext.PictureBinary.Single(b => b.PictureId == x.PictureID).BinaryData,
			}).ToList();

		}
		public async Task<PictureModel> AddPicture(PictureModel pictureModel)
		{
			Picture picture = new Picture()
			{
				FileName = pictureModel.FileName,
				MimeType = pictureModel.MimeType,
				AltAttribute = pictureModel.FileName,
				TitleAttribute = pictureModel.FileName
			};
			EntityEntry<Picture> pictureEntity = _dbContext.Picture.Add(picture);
			await _dbContext.SaveChangesAsync();
			PictureBinary pictureBinary = new PictureBinary()
			{
				PictureId = pictureEntity.Entity.PictureID,
				BinaryData =pictureModel.BinaryData,
			};
			using (MD5 md5 = MD5.Create())
			{
				byte[] hashBytes = md5.ComputeHash(pictureModel.BinaryData);
				pictureBinary.Hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
			}
			EntityEntry<PictureBinary> pictureBinaryEntity = _dbContext.PictureBinary.Add(pictureBinary);
			int rowsAdded = await _dbContext.SaveChangesAsync();
			if (rowsAdded == 0)
			{
				throw new DatabaseOperationException("Unable to add pictures.");
			}
			pictureModel.Hash = pictureBinaryEntity.Entity.Hash;
			pictureModel.PictureId = pictureEntity.Entity.PictureID;
			return pictureModel;
		}
		public async Task DeletePicture(string? hash)
		{
			if(hash == null)
			{
				throw new InvalidDataException("hash cannot be null");
			}
			int pictureId = await GetPictureId(hash);
			Picture picture = _dbContext.Picture.Single(x => x.PictureID == pictureId);
			PictureBinary pictureBinary = _dbContext.PictureBinary.Single(x => x.PictureId == pictureId);
			_dbContext.PictureBinary.Remove(pictureBinary);
			await _dbContext.SaveChangesAsync();
			_dbContext.Picture.Remove(picture);
			int rowsDeleted = await _dbContext.SaveChangesAsync();
			if (rowsDeleted == 0) {
				throw new DatabaseOperationException("Unable to delete the pictures.");
			}
		}

		public async Task DeletePictures(List<int> pictureIds)
		{
			int rowsDeleted = await _dbContext.PictureBinary.Where(x => pictureIds.Contains(x.PictureId)).ExecuteDeleteAsync();
			if (rowsDeleted == 0)
			{
				throw new DatabaseOperationException("Unable to delete the Picture Binary.");
			}
			rowsDeleted = await _dbContext.Picture.Where(x => pictureIds.Contains(x.PictureID)).ExecuteDeleteAsync();
			if (rowsDeleted == 0)
			{
				throw new DatabaseOperationException("Unable to delete the pictures.");
			}
		}
	}
}
