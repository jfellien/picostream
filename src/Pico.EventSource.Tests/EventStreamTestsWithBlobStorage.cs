using Microsoft.Extensions.Options;
using NSubstitute;
using Pico.EventSource.Models;
using Pico.EventSource.Persistence;
using Pico.EventSource.Persistence.Azure.BlobStorage;
using Shouldly;

namespace Pico.EventSource.Tests;

public class EventStreamTestsWithBlobStorage
{
    private const string CONTEXT = "TestContext";
    private readonly IReadAndWriteDomainEvents _repository;
    private readonly Guid _streamId = Guid.Parse("56a59926-5300-441d-802c-cf8e4872ef5e");
    
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

        await sut.Append(new SampleValueEvent()
        {
            SampleValue   = "MyFirstValue"
        }, CONTEXT, _streamId);
    }
    
    [Fact]
    public async Task Read_From_Blob_Storage_Should_Fail()
    {
        EventStore sut = new(_repository);

        await sut.Append(new SampleValueEvent()
        {
            SampleValue   = "MyFirstValue"
        }, CONTEXT, _streamId);
        
        DomainEventStream eventStream = await sut.GetStream(CONTEXT, _streamId);
        
        eventStream.Count.ShouldBe(1);
    }
}