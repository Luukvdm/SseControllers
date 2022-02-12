using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SseControllers.Interfaces;

public interface ISseClient
{
    public HttpResponse Response { get; }
    Task SendDataAsync(string msg, CancellationToken cancellationToken);
    Task SendSseEventAsync(IClientEvent msg, CancellationToken cancellationToken);
}