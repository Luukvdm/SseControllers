using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SseControllers.Interfaces;
using SseControllers.Services;

namespace SseControllers;

public static class DependencyInjection
{
    public static IServiceCollection AddSse(this IServiceCollection services, Action<SseControllersOptions> options)
    {
        services.Configure(options);
        services.AddSingleton<IClientEventService, ClientEventService>();
        services.AddSingleton<IHostedService, KeepAliveService>();
        return services;
    }

    public static IServiceCollection AddSse(this IServiceCollection services)
    {
        services.AddSse(options =>
        {
            options.ReconnectInterval = SseControllersOptions.DefaultReconnectInterval;
            options.KeepAliveInterval = SseControllersOptions.DefaultKeepAliveInterval;
            options.ReconnectAttempts = SseControllersOptions.DefaultReconnectAttempts;
        });
        return services;
    }
}