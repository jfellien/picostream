using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Pico.EventSource.Models;
using Pico.EventSource.Persistence;

namespace Pico.EventSource.Persistence.Azure.BlobStorage;

public class BlobStorageEventStoreClient : IReadAndWriteDomainEvents
{
    private const bool OVERWRITE_BLOB = true;

    private readonly Uri? _storageEndpoint;
    private readonly string? _storageConnectionString;

    public BlobStorageEventStoreClient(IOptions<StorageOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value);
        ArgumentNullException.ThrowIfNull(options.Value.Endpoint);

        if (options.Value.Endpoint.Contains("AccountKey="))
        {
            _storageConnectionString = options.Value.Endpoint;
        }
        else
        {
            _storageEndpoint = new Uri(options.Value.Endpoint);
        }
    }

    public async Task<DomainEventStream> Read(string context, Guid streamId, CancellationToken ct)
    {
        context = context.ToLowerInvariant();
        
        BlobClient contextBlob = await GetBlob(context, streamId, ct);

        if (await contextBlob.ExistsAsync(ct) == false) return [];

        DomainEventStream domainEventStream = await ReadDomainEventStream(contextBlob, ct);

        return domainEventStream;
    }

    public async Task Write(IDomainEvent domainEvent, string context, Guid streamId, CancellationToken ct)
    {
        context = context.ToLowerInvariant();
        
        BlobClient contextBlob = await GetBlob(context, streamId, ct);

        List<DomainEventEnvelope> domainEventStream = await ReadDomainEventEnvelopes(contextBlob, ct);

        DomainEventEnvelope envelope = new()
        {
            TimeStamp = DateTimeOffset.UtcNow,
            Context = context,
            StreamId = streamId,
            EventId = Guid.NewGuid(),
            Event = domainEvent
        };
        
        domainEventStream.Add(envelope);

        BinaryData binaryEventStream = new(domainEventStream, SerializerOptions);

        await contextBlob.UploadAsync(binaryEventStream, OVERWRITE_BLOB, ct);
    }
    
    private async Task<BlobClient> GetBlob(string context, Guid streamId, CancellationToken ct)
    {
        BlobContainerClient client = await GetContextContainer(context, ct);

        return client.GetBlobClient(streamId.ToString());
    }

    private async Task<BlobContainerClient> GetContextContainer(string context, CancellationToken ct)
    {
        try
        {
            BlobServiceClient client = GetBlobServiceClient();

            AsyncPageable<BlobContainerItem> segment = client.GetBlobContainersAsync(cancellationToken: ct);
            IAsyncEnumerable<Page<BlobContainerItem>> segmentPages = segment.AsPages();

            bool containerExists = false;

            await foreach (Page<BlobContainerItem> containerPage in segmentPages)
            {
                if (containerPage.Values.Any(x => x.Name.Equals(context)))
                {
                    containerExists = true;
                }
            }

            if (containerExists)
            {
                return client.GetBlobContainerClient(context);
            }

            return await client.CreateBlobContainerAsync(context, cancellationToken: ct);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    private BlobServiceClient GetBlobServiceClient()
    {
        return _storageConnectionString != null
            ? new BlobServiceClient(_storageConnectionString)
            : new BlobServiceClient(_storageEndpoint, new DefaultAzureCredential());
    }

    private static async Task<DomainEventStream> ReadDomainEventStream(BlobClient blobClient, CancellationToken ct)
    {
        if(await blobClient.ExistsAsync(ct) == false) return [];
        
        Response<BlobDownloadResult>? blob = await blobClient.DownloadContentAsync(ct);

        if (blob.HasValue == false)
        {
            throw new ApplicationException("Stream has no value");
        }

        List<DomainEventEnvelope>? domainEvents = blob.Value.Content.ToObjectFromJson<List<DomainEventEnvelope>>(SerializerOptions);

        if (domainEvents == null) return [];

        DomainEventStream eventStream = [];

        foreach (DomainEventEnvelope eventEnvelope in domainEvents)
        {
            eventStream.Add(eventEnvelope.Event);
        }
        
        return eventStream;
    }
    
    private static async Task<List<DomainEventEnvelope>> ReadDomainEventEnvelopes(BlobClient blobClient, CancellationToken ct)
    {
        if(await blobClient.ExistsAsync(ct) == false) return [];
        
        Response<BlobDownloadResult>? blob = await blobClient.DownloadContentAsync(ct);

        if (blob.HasValue == false)
        {
            throw new ApplicationException("Stream has no value");
        }

        List<DomainEventEnvelope>? domainEventEnvelopes = 
            blob.Value.Content.ToObjectFromJson<List<DomainEventEnvelope>>(SerializerOptions);
        
        return domainEventEnvelopes ?? [];
    }

    private static JsonSerializerOptions SerializerOptions =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
            TypeInfoResolver = new DomainEventTypeResolver()
        };
}