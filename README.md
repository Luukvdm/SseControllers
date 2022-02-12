## SseControllers

### What is SseControllers?

SseControllers is a library that helps with adding [server sent events](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events) to your Asp.Net Controllers.
Server sent events can be used to push data to a web page at any time like websockets.
Unlike websockets, server sent events flow only one way: from the server to the client.
But because the events only flow one way the protocol is much simpler then websockets.
And server sent events take advantage of HTTP's feature set because its protocol is build on top of HTTP.

### Getting started

There is an [example project](./ExampleProject) inside the repository that you can check out.
Or you can follow these basic steps.

Start by injecting the SseControllers services with dependency injection.
In most projects you would do this in the `Startup.cs` like this:
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // If you don't care about these options you can also just do services.AddSse();
        services.AddSse(options =>
        {
            options.ReconnectInterval = SseControllersOptions.DefaultReconnectInterval;
            options.KeepAliveInterval = SseControllersOptions.DefaultKeepAliveInterval;
            options.ReconnectAttempts = SseControllersOptions.DefaultReconnectAttempts;
        });
    }
}
```


Then add an endpoint with the `text/event-stream` content type to your controller.
And create a new stream inside this endpoints.
```csharp
[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    [HttpGet("stream")]
    [Consumes(SseConstants.SseContentType)]
    public async Task NotificationStream()
    {
        await StreamCreator.CreateStream(Response, _eventService);
    }
}
```

Now you can send messages to your clients by getting `IClientEventService` from dependency injection and use it to send `ClientEvent` to your clients.

```csharp
await _eventService.SendEventAsync(new ClientEvent(notification.Notification));
```
