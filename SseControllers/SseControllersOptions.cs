using System;

namespace SseControllers;

public class SseControllersOptions
{
    internal static readonly TimeSpan DefaultKeepAliveInterval = TimeSpan.FromSeconds(5);
    internal static readonly TimeSpan DefaultReconnectInterval = TimeSpan.FromSeconds(5);
    internal const int DefaultReconnectAttempts = 5;
        
    public TimeSpan KeepAliveInterval { get; set; }
    public TimeSpan ReconnectInterval { get; set; }
    public int ReconnectAttempts { get; set; }
}