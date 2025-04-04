using Microsoft.Identity.Client.Extensions.Msal;
using Pico.EventSource.LinQ.Extensions;
using Pico.EventSource.Models;
using Shouldly;

namespace Pico.EventSource.Tests;

public class EventStoreTests : EventStreamTestSetup
{
    private const string CONTEXT = "TestContext";
    
    [Fact]
    public async Task HappenedEarlierThan_Should_Be_True()
    {
        Guid streamId = Guid.NewGuid();
        
        EventStore sut = new(Storage!);
        
        SampleValueEvent firstSampleEvent = new()
        {
            SampleValue   = "MyFirstValue"
        };
        SampleValueEvent secondSampleEvent = new()
        {
            SampleValue   = "MySecondValue"
        };
        
        await sut.Append(firstSampleEvent, CONTEXT, streamId);
        await sut.Append(secondSampleEvent, CONTEXT, streamId);
        
        DomainEventStream eventStream = await sut.GetStream(CONTEXT, streamId);

        eventStream.Verify<SampleValueEvent>()
            .Where(x => x.SampleValue == firstSampleEvent.SampleValue)
            .HappenedEarlierThan<SampleValueEvent>()
            .Where(x=>x.SampleValue == secondSampleEvent.SampleValue)
            .ShouldBeTrue();
    }
    
    [Fact]
    public async Task HappenedEarlierThan_Should_Be_False()
    {
        Guid streamId = Guid.NewGuid();
        
        EventStore sut = new(Storage!);
        
        SampleValueEvent firstSampleEvent = new()
        {
            SampleValue   = "MyFirstValue"
        };
        SampleValueEvent secondSampleEvent = new()
        {
            SampleValue   = "MySecondValue"
        };
        
        await sut.Append(firstSampleEvent, CONTEXT, streamId);
        await sut.Append(secondSampleEvent, CONTEXT, streamId);
        
        DomainEventStream eventStream = await sut.GetStream(CONTEXT, streamId);

        eventStream.Verify<SampleValueEvent>()
            .Where(x => x.SampleValue == secondSampleEvent.SampleValue)
            .HappenedEarlierThan<SampleValueEvent>()
            .Where(x=>x.SampleValue == firstSampleEvent.SampleValue)
            .ShouldBeFalse();
    }
}