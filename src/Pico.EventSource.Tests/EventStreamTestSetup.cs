using Pico.EventSource.Persistence;

namespace Pico.EventSource.Tests;

public class EventStreamTestSetup : IDisposable
{
    protected IReadAndWriteDomainEvents? Storage;

    protected EventStreamTestSetup()
    {
        Storage = NSubstitute.Substitute.For<IReadAndWriteDomainEvents>(); 
    }
    
    public void Dispose()
    {
        Storage = null;
    }
}