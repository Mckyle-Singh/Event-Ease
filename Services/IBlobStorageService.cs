namespace Event_Ease.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string containerName);
        Task DeleteFileAsync(string fileUrl, string containerName); 
    }
}
