using Pico.EventStore.Models;

namespace Pico.EventStore.Tests;

public class SampleValueEvent(string requesterId) : DomainEvent(requesterId)
{
    public required string SampleValue { get; set; }
}