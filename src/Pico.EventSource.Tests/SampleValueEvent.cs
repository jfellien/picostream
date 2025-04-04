using Pico.EventSource.Models;

namespace Pico.EventSource.Tests;

public class SampleValueEvent() : DomainEvent()
{
    public required string SampleValue { get; set; }
}