namespace BaseProject.Infrastructure.Storage.Cloudflare.Models;
internal sealed class CloudflareResponseResultModel
{
    public string Id { get; set; } = default!;
    public List<string> Variants { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public DateTime Uploaded { get; set; }
    public bool RequireSignedURLs { get; set; }
}
