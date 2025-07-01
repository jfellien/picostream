using Microsoft.Identity.Client.Extensions.Msal;
using Pico.EventSource.LinQ.Extensions;
using Pico.EventSource.Models;
using Shouldly;

namespace Pico.EventSource.Tests;

public class EventStoreTests : EventStreamTestSetup
{
    private readonly ContextId _context = new ContextId("TestContext");
    private readonly StreamId _streamId = new StreamId("TestStream");
    
    [Fact]
    public async Task HappenedEarlierThan_Should_Be_True()
    {
        EventStore sut = new(Storage!);
        
        SampleValueEvent firstSampleEvent = new()
        {
            SampleValue   = "MyFirstValue"
        };
        SampleValueEvent secondSampleEvent = new()
        {
            SampleValue   = "MySecondValue"
        };
        
        await sut.Append(_context, _streamId, firstSampleEvent);
        await sut.Append(_context, _streamId, secondSampleEvent);
        
        DomainEventStream eventStream = await sut.GetStream(_context, _streamId);

        eventStream.VerifyIf<SampleValueEvent>()
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
        
        await sut.Append(_context, _streamId,firstSampleEvent);
        await sut.Append(_context, _streamId, secondSampleEvent);
        
        DomainEventStream eventStream = await sut.GetStream(_context, _streamId);

        eventStream.VerifyIf<SampleValueEvent>()
            .Where(x => x.SampleValue == secondSampleEvent.SampleValue)
            .HappenedEarlierThan<SampleValueEvent>()
            .Where(x=>x.SampleValue == firstSampleEvent.SampleValue)
            .ShouldBeFalse();
    }
}