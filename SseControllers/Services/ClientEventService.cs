using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SseControllers.Interfaces;

namespace SseControllers.Services
{
    public class ClientEventService : IClientEventService
    {
        private readonly ConcurrentDictionary<Guid, ISseClient> _clients;
        private readonly ILogger<ClientEventService> _logger;
        private readonly SseControllersOptions _options;

        public ClientEventService(IOptions<SseControllersOptions> options, ILogger<ClientEventService> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _clients = new ConcurrentDictionary<Guid, ISseClient>();
        }

        public Guid AddClient(ISseClient sseClient)
        {
            var clientId = Guid.NewGuid();
            _clients.TryAdd(clientId, sseClient);
            return clientId;
        }

        public ISseClient RemoveClient(Guid clientId)
        {
            _clients.TryRemove(clientId, out var client);
            return client;
        }

        // TODO https://javascript.info/server-sent-events#message-id
        public async Task AttemptReconnect(Guid clientId, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _options.ReconnectAttempts; i++)
            {
                _logger.LogInformation("Reconnect attempt " + i + "/" + _options.ReconnectAttempts);
                // TODO how can we tell if this succeeds
                string msg = i + "\n";
                await _clients[clientId].SendDataAsync(msg, cancellationToken);

                await Task.Delay(_options.ReconnectInterval, cancellationToken);
            }
        }

        public Task SendDataAsync(string msg, CancellationToken cancellationToken)
        {
            var clientTasks = _clients.Select(e => e.Value.SendDataAsync(msg, cancellationToken)).ToList();
            return Task.WhenAll(clientTasks);
        }

        public Task SendEventAsync(IClientEvent msg, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SSE event: {MessageType} {ClientCount}", msg.Type, _clients.Count);
            var clientTasks = _clients.Select(client => client.Value.SendSseEventAsync(msg, cancellationToken))
                .ToList();
            return Task.WhenAll(clientTasks);
        }

        public Task SendEventAsync(IClientEvent msg, Guid clientId, CancellationToken cancellationToken)
        {
            if (clientId == null)
            {
                throw new ArgumentException("Client Id cant be null", nameof(clientId));
            }

            _logger.LogInformation($"Sending SSE to clientEventDispatcher id {clientId}");
            return _clients[clientId].SendSseEventAsync(msg, cancellationToken);
        }
    }
}
