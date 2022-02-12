using System;
using System.Threading;
using System.Threading.Tasks;

namespace SseControllers.Interfaces
{
    public interface IClientEventService
    {
        Guid AddClient(ISseClient sseClient);
        ISseClient RemoveClient(Guid clientId);
        Task AttemptReconnect(Guid clientId, CancellationToken cancellationToken = default);
        Task SendDataAsync(string msg, CancellationToken cancellationToken = default);
        Task SendEventAsync(IClientEvent msg, Guid clientId, CancellationToken cancellationToken = default);
        Task SendEventAsync(IClientEvent msg, CancellationToken cancellationToken = default);
    }
}
