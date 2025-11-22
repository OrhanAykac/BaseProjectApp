using BlobHelper;
using Microsoft.Extensions.Configuration;

namespace BaseProject.Infrastructure.Storage.Cloudflare.StorageService;

public class CloudflareObjectStorageHelper(IConfiguration configuration)
{
    public async Task UploadObject(string contentFileName, string contentType, byte[] content, string bucket)
    {
        var client = GetClient(bucket);
        await client.Write(contentFileName, contentType, content);
    }
    public async Task<byte[]> DownloadObject(string contentKeyName, string bucket)
    {
        var client = GetClient(bucket);
        var result = await client.Get(contentKeyName);

        return result;
    }
    public async Task DeleteObject(string contentKeyName, string bucket)
    {
        var client = GetClient(bucket);
        await client.Delete(contentKeyName);
    }
    public async Task<bool> IsExists(string contentKeyName, string bucket)
    {
        var client = GetClient(bucket);

        return await client.Exists(contentKeyName);
    }


    private BlobClient GetClient(string? bucket = null) => new(new AwsSettings(
        endpoint: configuration.GetValue<string>("Cloudflare:ObjectStorage:BaseUrl")!,
        ssl: true,
        accessKey: configuration.GetValue<string>("Cloudflare:ObjectStorage:AccessKey")!,
        secretKey: configuration.GetValue<string>("Cloudflare:ObjectStorage:SecretKey")!,
        region: AwsRegion.EUCentral1,
        bucket: bucket ?? configuration.GetValue<string>("Cloudflare:ObjectStorage:DefaultBucket")!,
        baseUrl: configuration.GetValue<string>("Cloudflare:ObjectStorage:BaseUrl")!));
}
