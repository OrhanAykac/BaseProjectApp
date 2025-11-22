namespace BaseProject.Infrastructure.Contract;

public record StorageModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string FileId { get; set; } = default!;
    public string FileName { get; set; } = default!;
}
