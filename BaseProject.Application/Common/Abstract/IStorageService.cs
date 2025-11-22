using BaseProject.Infrastructure.Contract;

namespace BaseProject.Application.Common.Abstract;

public interface IStorageService
{
    Task<bool> DeleteImage(List<string> ids);
    Task<bool> DeleteImage(string id);
    Task<bool> DeleteImage(string[] ids);
    Task<List<StorageModel>> UploadImage(Dictionary<string, byte[]> byteImage);
    Task<StorageModel> UploadImage(string imageURL);
    Task<StorageModel> UploadImage(string? imageName, byte[] byteImage);
    Task<List<StorageModel>> UploadImage(string[] imageURL);

    Task ReplaceObject(string contentName, string contentType, byte[] content, string bucketName);
    Task<byte[]> DownloadObject(string contentName, string bucketName);
    Task<bool> IsObjectExist(string contentName, string bucketName);
    Task DeleteObject(string contentName, string bucketName);
    Task UploadObject(string contentName, string contentType, byte[] content, string bucketName);
}