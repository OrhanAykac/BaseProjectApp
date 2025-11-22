using BaseProject.Application.Common.Abstract;
using BaseProject.Infrastructure.Contract;
using BaseProject.Infrastructure.Storage.Cloudflare.ImageService;
using BaseProject.Infrastructure.Storage.Cloudflare.StorageService;

namespace BaseProject.Infrastructure.Storage;

public class StorageService(CloudflareImageHelper cloudflareHelper, CloudflareObjectStorageHelper cloudflareObjectStorageHelper)
    : IStorageService
{
    public Task<bool> DeleteImage(string id)
    {
        return cloudflareHelper.DeleteImage(id);
    }

    public async Task<bool> DeleteImage(string[] ids)
    {
        await Task.WhenAll(
            ids.Select(cloudflareHelper.DeleteImage));

        return true;
    }
    public async Task<bool> DeleteImage(List<string> ids)
    {
        await Task.WhenAll(
            ids.Select(cloudflareHelper.DeleteImage));

        return true;
    }

    public async Task<StorageModel> UploadImage(string imageURL)
    {
        var result = await cloudflareHelper.UploadImage(imageURL);
        return result;
    }

    public async Task<StorageModel> UploadImage(string? imageName, byte[] byteImage)
    {

        var result = await cloudflareHelper.UploadImage(imageName ?? "default-image-name.jpg", byteImage);
        return result;
    }
    public async Task<List<StorageModel>> UploadImage(string[] imageURL)
    {
        var result = await cloudflareHelper.UploadImage(imageURL);
        return result;
    }

    public async Task<List<StorageModel>> UploadImage(Dictionary<string, byte[]> byteImage)
    {
        var result = await cloudflareHelper.UploadImage(byteImage);
        return result;
    }

    public Task UploadObject(string contentName, string contentType, byte[] content, string bucketName)
    {
        return cloudflareObjectStorageHelper.UploadObject(contentName, contentType, content, bucketName);
    }
    public Task DeleteObject(string contentName, string bucketName)
    {
        return cloudflareObjectStorageHelper.DeleteObject(contentName, bucketName);
    }
    public Task<byte[]> DownloadObject(string contentName, string bucketName)
    {
        return cloudflareObjectStorageHelper.DownloadObject(contentName, bucketName);
    }
    public Task<bool> IsObjectExist(string contentName, string bucketName)
    {
        return cloudflareObjectStorageHelper.IsExists(contentName, bucketName);
    }

    /// <summary>
    /// Yolladığınız documentId'li dosyayı siler, güncellenmiş haliyle tekrar ekler.
    /// </summary>
    public Task ReplaceObject(string contentName, string contentType, byte[] content, string bucketName)
    {
        return Task.WhenAll(
            cloudflareObjectStorageHelper.DeleteObject(contentName, bucketName),
            cloudflareObjectStorageHelper.UploadObject(contentName, contentType, content, bucketName));
    }
}

