using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SseControllers.Interfaces;

namespace SseControllers;

public static class StreamCreator
{
    public static async Task CreateStream(HttpResponse response, IClientEventService sseService)
    {
        response.StatusCode = 200;
        response.Headers.Add("Content-Type", "text/event-stream");

        await response.Body.FlushAsync();

        var client = new SseClient(response);
        var clientId = sseService.AddClient(client);

        response.HttpContext.RequestAborted.WaitHandle.WaitOne();

        await sseService.AttemptReconnect(clientId);

        sseService.RemoveClient(clientId);
    }
}
