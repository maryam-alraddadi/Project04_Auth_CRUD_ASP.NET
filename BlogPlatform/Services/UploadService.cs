using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BlogPlatform.Services
{
    public class UploadService : IUploadService
    {
        private readonly string _azureConnectionString;

        public UploadService(IConfiguration configuration)
        {
            _azureConnectionString =  configuration.GetConnectionString("AzureConnectionString");
        }

        public async Task<string> UploadImage(IFormFile image)
        {
            try
            {
                if (image.Length <= 0) return null;
                var container = new BlobContainerClient(_azureConnectionString, "upload-container");
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);

                if (!image.ContentType.Contains("image")) return null;
                var fileName = DateTime.Now.Ticks + "." + System.IO.Path.GetExtension(image.FileName).Substring(1);
                var blob = container.GetBlobClient(fileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await using (var fileStream = image.OpenReadStream())
                {
                    await blob.UploadAsync(fileStream, new BlobHttpHeaders {ContentType = image.ContentType});
                }

                return blob.Uri.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}