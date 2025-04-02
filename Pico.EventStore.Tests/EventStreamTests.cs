using Pico.EventStore.LinQ.Extensions;
using Pico.EventStore.Models;
using Shouldly;

namespace Pico.EventStore.Tests;

public class EventStreamTests : EventStreamTestSetup
{
    private const string Context = "TestContext";
    private const string Entity = "TestEntity";
    private const string EntityId = "1234";
    private const string RequesterId = "requesterId";
    
    [Fact]
    public async Task HappenedEarlierThan_Should_Be_True()
    {
        EventStream sut = new(Context, Entity, EntityId, Storage!);
        
        SampleValueEvent firstSampleEvent = new(RequesterId)
        {
            SampleValue   = "MyFirstValue"
        };
        SampleValueEvent secondSampleEvent = new(RequesterId)
        {
            SampleValue   = "MySecondValue"
        };
        
        await sut.Append(firstSampleEvent);
        await sut.Append(secondSampleEvent);
        
        DomainEvents events = await sut.Events();

        events.Verify<SampleValueEvent>()
            .Where(x => x.SampleValue == firstSampleEvent.SampleValue)
            .HappenedEarlierThan<SampleValueEvent>()
            .Where(x=>x.SampleValue == secondSampleEvent.SampleValue)
            .ShouldBeTrue();
    }
    
    [Fact]
    public async Task HappenedEarlierThan_Should_Be_False()
    {
        EventStream sut = new(Context, Entity, EntityId, Storage!);
        
        SampleValueEvent firstSampleEvent = new(RequesterId)
        {
            SampleValue   = "MyFirstValue"
        };
        SampleValueEvent secondSampleEvent = new(RequesterId)
        {
            SampleValue   = "MySecondValue"
        };
        
        await sut.Append(firstSampleEvent);
        await sut.Append(secondSampleEvent);
        
        DomainEvents events = await sut.Events();

        events.Verify<SampleValueEvent>()
            .Where(x => x.SampleValue == secondSampleEvent.SampleValue)
            .HappenedEarlierThan<SampleValueEvent>()
            .Where(x=>x.SampleValue == firstSampleEvent.SampleValue)
            .ShouldBeFalse();
    }
}