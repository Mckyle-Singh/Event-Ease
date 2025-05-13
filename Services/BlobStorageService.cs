using Azure.Storage.Blobs;
using Event_Ease.Infra;
using Microsoft.Extensions.Options;

namespace Event_Ease.Services
{
    public class BlobStorageService:IBlobStorageService
    {
        private readonly AzureBlobStorageSettings _blobSettings;


        public BlobStorageService(IOptions<AzureBlobStorageSettings> options)
        {
            _blobSettings = options.Value;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_blobSettings.ConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await blobContainerClient.CreateIfNotExistsAsync();
            await blobContainerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }
    }
}
