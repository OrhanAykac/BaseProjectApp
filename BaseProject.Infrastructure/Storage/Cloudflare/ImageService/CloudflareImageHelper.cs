using BaseProject.Infrastructure.Contract;
using BaseProject.Infrastructure.Storage.Cloudflare.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BaseProject.Infrastructure.Storage.Cloudflare.ImageService;

public class CloudflareImageHelper(IConfiguration configuration, IHttpClientFactory httpClientFactory)
{
    private readonly string accountKey = configuration["CloudFlare:ImageStorage:AccountKey"]!;
    private readonly HttpClient httpClient = httpClientFactory.CreateClient("CFImage");
    internal async Task<StorageModel> UploadImage(string imageName, byte[] byteImage)
    {
        using MultipartFormDataContent content = [];
        var fileContent = new ByteArrayContent(byteImage);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse($"image/{imageName.Split(".")?.Last()}");

        content.Add(fileContent, "\"file\"", "\"" + imageName + "\"");

        string url = $"client/v4/accounts/{accountKey}/images/v1";

        var httpResponse = await httpClient.PostAsync(url, content);

        if (!httpResponse.IsSuccessStatusCode)
            return new StorageModel() { Success = false, Message = httpResponse.StatusCode.ToString() };

        var jsonResult = await httpResponse.Content.ReadFromJsonAsync<CloudflareResponseModel>();

        return new StorageModel()
        {
            Success = jsonResult!.Success,
            Message = jsonResult.Errors.FirstOrDefault(),
            FileId = jsonResult.Result.Id,
            FileName = imageName,
        };
    }

    internal async Task<List<StorageModel>> UploadImage(Dictionary<string, byte[]> byteImages)
    {
        List<Task<StorageModel>> cloudImages = [];

        foreach (var dic in byteImages)
        {
            cloudImages.Add(UploadImage(dic.Key, dic.Value));
        }

        var result = await Task.WhenAll(cloudImages);
        return [.. result];
    }

    internal async Task<StorageModel> UploadImage(string imageURL)
    {
        using MultipartFormDataContent content = [];
        var fileContent = new StringContent(imageURL);
        //fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse($"image/{imgName.Split(".")?.Last()}");

        content.Add(fileContent, "\"url\"", "\"" + imageURL + "\"");

        string url = $"client/v4/accounts/{accountKey}/images/v1";

        var httpResponse = await httpClient.PostAsync(url, content);

        if (httpResponse.IsSuccessStatusCode == false)
            return new StorageModel() { Success = false, Message = httpResponse.StatusCode.ToString() };


        var jsonResult = await httpResponse.Content.ReadFromJsonAsync<CloudflareResponseModel>();

        return new StorageModel()
        {
            Success = jsonResult!.Success,
            Message = jsonResult.Errors.First(),
            FileId = jsonResult.Result.Id,
            FileName = jsonResult.Result.FileName,
        };
    }
    internal async Task<List<StorageModel>> UploadImage(string[] imageURLs)
    {
        List<Task<StorageModel>> cloudImages = [];

        foreach (var imgUrl in imageURLs)
        {
            cloudImages.Add(UploadImage(imgUrl));
        }
        var result = await Task.WhenAll(cloudImages);

        return [.. result];
    }

    internal async Task<bool> DeleteImage(string id)
    {

        string requestUrl = $"/client/v4/accounts/{accountKey}/images/v1/{id}";
        var httpResult = await httpClient.DeleteAsync(requestUrl);

        if (httpResult.IsSuccessStatusCode == false)
            return false;

        var resultContent = await httpResult.Content.ReadFromJsonAsync<CloudflareResponseModel>();

        return resultContent!.Success;
    }
    internal async Task<bool> DeleteImage(string[] ids)
    {

        List<Task<bool>> cloudImages = [];

        foreach (var imgId in ids)
        {
            cloudImages.Add(DeleteImage(imgId));
        }
        var result = await Task.WhenAll(cloudImages);

        if (result.Any(a => a == false) == false)
            return false;

        return true;
    }
}