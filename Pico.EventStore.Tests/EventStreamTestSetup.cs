using Pico.EventStore.Persistence;

namespace Pico.EventStore.Tests;

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