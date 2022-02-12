using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SseControllers.Interfaces;

namespace SseControllers.Extensions
{
    internal static class HttpResponseExtension
    {
        internal static async Task WriteSseEventAsync(this HttpResponse response, IClientEvent clientEvent,
            CancellationToken cancellationToken)
        {
            if (clientEvent.Data == null || clientEvent.Data.Count == 0)
            {
                throw new ArgumentException("Cant send an event without data", nameof(clientEvent));
            }

            if (!string.IsNullOrWhiteSpace(clientEvent.Id))
            {
                await response.WriteSseEventFieldAsync(Constants.SseIdField, clientEvent.Id, cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(clientEvent.Type))
            {
                await response.WriteSseEventFieldAsync(Constants.SseEventField, clientEvent.Type, cancellationToken);
            }

            foreach (string line in clientEvent.Data)
            {
                await response.WriteSseEventFieldAsync(Constants.SseDataField, line, cancellationToken);
            }

            await response.WriteSseEventBoundaryAsync(cancellationToken);
            await response.Body.FlushAsync(cancellationToken);
        }

        private static Task WriteSseEventFieldAsync(this HttpResponse response, string field, string data,
            CancellationToken cancellationToken)
        {
            return response.WriteAsync($"{field}: {data}\n", cancellationToken);
        }

        private static Task WriteSseEventBoundaryAsync(this HttpResponse response, CancellationToken cancellationToken)
        {
            return response.WriteAsync("\n", cancellationToken);
        }
    }
}
