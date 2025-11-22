namespace BaseProject.Infrastructure.Storage.Cloudflare.Models;

internal class CloudflareResponseModel
{
    public CloudflareResponseResultModel Result { get; set; } = default!;
    public object Result_Info { get; set; } = default!;
    public bool Success { get; set; } = default!;
    public string[] Errors { get; set; } = default!;
    public string[] Messages { get; set; } = default!;
}