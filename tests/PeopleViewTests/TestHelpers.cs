using AppMvp.Presentation.Abstractions;
using AppMvp.Domain.Repositories;
using AppMvp.Domain.Entities;
using AppMvp.ApplicationCore.EventBus;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal class FakeBusyIndicator : IBusyIndicator
{
    public bool IsBusy { get; private set; }
    public string? Message { get; private set; }
    private string? _activeId;

    public event EventHandler<BusyStateChangedEventArgs>? BusyStateChanged;

    public IBusyScope Begin(string? message = null, string? scopeId = null)
    {
        Message = message;
        IsBusy = true;
        _activeId = scopeId;
        OnBusyStateChanged(new BusyStateChangedEventArgs(true, message, new[] { scopeId ?? string.Empty }));
        return new DummyScope(this, scopeId);
    }

    private void End(string? scopeId)
    {
        IsBusy = false;
        Message = null;
        _activeId = null;
        OnBusyStateChanged(new BusyStateChangedEventArgs(false, null, Array.Empty<string>()));
    }

    public void RequestCancel(string? scopeId = null) { }
    public void SetSynchronizationContext(SynchronizationContext? context) { }

    public System.Collections.Generic.IReadOnlyCollection<string> GetActiveScopeIds()
    {
        if (IsBusy && !string.IsNullOrEmpty(_activeId))
            return new[] { _activeId! };
        return Array.Empty<string>();
    }

    private void OnBusyStateChanged(BusyStateChangedEventArgs e) => BusyStateChanged?.Invoke(this, e);

    private sealed class DummyScope : IBusyScope
    {
        private readonly FakeBusyIndicator _owner;
        private readonly string? _id;
        public DummyScope(FakeBusyIndicator owner, string? id) { _owner = owner; _id = id; }
        public CancellationToken Token => CancellationToken.None;
        public string? Id => _id;
        public void Dispose() => _owner.End(_id);
    }
}

internal class FakePersonRepository : IPersonRepository
{
    public Task AddAsync(Person person, CancellationToken token) => Task.CompletedTask;
    public Task DeleteAsync(Person person) { return Task.CompletedTask; }
    public Task<Person?> GetByIdAsync(int id, CancellationToken token) => Task.FromResult<Person?>(null);
    public Task<List<Person>> GetAllAsync(CancellationToken token) => Task.FromResult(new List<Person>());
    public Task UpdateAsync(Person person, CancellationToken token) => Task.CompletedTask;
}

internal class FakeEventBus : AppMvp.ApplicationCore.EventBus.IApplicationEventBus
{
    public Task PublishAsync<TEvent>(TEvent evt) where TEvent : INotification
    {
        return Task.CompletedTask;
    }
}

internal static class StaTestHelper
{
    public static void RunInSta(Action action, int timeoutMs = 5000)
    {
        Exception? ex = null;
        var evt = new ManualResetEvent(false);
        var thread = new Thread(() =>
        {
            try
            {
                // Ensure a WindowsFormsSynchronizationContext is available on the STA thread so
                // BusyIndicatorService can capture and post UI updates during tests.
                System.Threading.SynchronizationContext.SetSynchronizationContext(new System.Windows.Forms.WindowsFormsSynchronizationContext());
                action();
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                evt.Set();
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();
        if (!evt.WaitOne(timeoutMs))
            throw new TimeoutException("STA action timed out");
        if (ex != null) throw new AggregateException("Exception in STA action", ex);
    }
}
