using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SseControllers.Interfaces;

namespace SseControllers.Services;

internal class KeepAliveService : IHostedService, IDisposable
{
    private readonly IClientEventService _clientEventService;
    private readonly CancellationTokenSource _stoppingCts;
    private Task _executingTask;
    private readonly TimeSpan _interval;

    public KeepAliveService(IOptions<SseControllersOptions> options, IClientEventService clientEventService)
    {
        var theOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _clientEventService = clientEventService;
        _stoppingCts = new CancellationTokenSource();
        _interval = theOptions.KeepAliveInterval;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _executingTask = SendKeepAliveMessages(_interval, cancellationToken);
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
        {
            return;
        }

        try
        {
            _stoppingCts.Cancel();
        }
        finally
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    public void Dispose()
    {
        _stoppingCts.Cancel();
    }

    private async Task SendKeepAliveMessages(TimeSpan interval, CancellationToken cancellationToken)
    {
        while (!_stoppingCts.Token.IsCancellationRequested)
        {
            await _clientEventService.SendDataAsync(SseConstants.KeepAliveMessage, cancellationToken);
            await Task.Delay(interval, cancellationToken);
        }
    }
}