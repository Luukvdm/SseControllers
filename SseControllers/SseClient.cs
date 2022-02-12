using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SseControllers.Extensions;
using SseControllers.Interfaces;

namespace SseControllers;

public class SseClient : ISseClient
{
    internal SseClient(HttpResponse response)
    {
        Response = response;
        IsConnected = true;
    }

    public HttpResponse Response { get; }
    public bool IsConnected { get; private set; }

    public async Task SendDataAsync(string msg, CancellationToken cancellationToken)
    {
        await Response.WriteAsync(msg + "\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }

    public Task SendSseEventAsync(IClientEvent msg, CancellationToken cancellationToken)
    {
        return Response.WriteSseEventAsync(msg, cancellationToken);
    }
}
