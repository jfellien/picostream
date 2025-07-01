using Microsoft.Extensions.Options;
using NSubstitute;
using Pico.EventSource.Models;
using Pico.EventSource.Persistence;
using Pico.EventSource.Persistence.Azure.BlobStorage;
using Shouldly;

namespace Pico.EventSource.Tests;

public class EventStreamTestsWithBlobStorage
{
    private readonly ContextId _context = new ContextId("TestContext");
    private readonly StreamId _streamId = new StreamId("TestStream");
    
    private readonly IReadAndWriteDomainEvents _repository;
    
    public EventStreamTestsWithBlobStorage()
    {
        IOptions<StorageOptions> options = Substitute.For<IOptions<StorageOptions>>();
        options.Value.Returns(new StorageOptions()
        {
            Endpoint =
                "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;"
        });
        _repository = new BlobStorageEventStoreClient(options);
    }
    
    [Fact]
    public async Task Write_To_Blob_Storage_Should_Succeed()
    {
        EventStore sut = new(_repository);

        await sut.Append(_context, _streamId, new SampleValueEvent()
        {
            SampleValue   = "MyFirstValue"
        });
    }
    
    [Fact]
    public async Task Read_From_Blob_Storage_Should_Fail()
    {
        EventStore sut = new(_repository);

        await sut.Append(_context, _streamId, new SampleValueEvent()
        {
            SampleValue   = "MyFirstValue"
        });
        
        DomainEventStream eventStream = await sut.GetStream(_context, _streamId);
        
        eventStream.Events.Count.ShouldBe(1);
    }
}