﻿using CloudinaryDotNet.Actions;
using RunGroops.Interfaces;
using Microsoft.Extensions.Options;
using RunGroops.Helpers;
using Microsoft.AspNetCore.Builder;
using CloudinaryDotNet;

namespace RunGroops.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicUrl)
        {
			var publicId = publicUrl.Split('/').Last().Split('.')[0];
			var deleteParams = new DeletionParams(publicId);
			return await _cloudinary.DestroyAsync(deleteParams);
		}
    }
}
