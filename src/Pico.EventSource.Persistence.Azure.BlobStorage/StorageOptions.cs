namespace Pico.EventSource.Persistence.Azure.BlobStorage;

public class StorageOptions
{
    public const string NAME = nameof(StorageOptions);
    
    public string? Endpoint { get; set; }
}
