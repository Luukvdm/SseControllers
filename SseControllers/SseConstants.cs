namespace SseControllers;

public static class SseConstants
{
    public const string SseContentType = "text/event-stream";
    internal const string SseIdField = "id";
    internal const string SseEventField = "event";
    internal const string SseDataField = "data";
    internal const string KeepAliveMessage = "KEEP ALIVE";
}
