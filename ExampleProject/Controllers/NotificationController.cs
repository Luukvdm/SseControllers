using System;
using System.Threading.Tasks;
using ExampleProject.Dtos;
using Microsoft.AspNetCore.Mvc;
using SseControllers;
using SseControllers.Interfaces;
using SseControllers.Models;

namespace ExampleProject.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IClientEventService _eventService;

    public NotificationController(IClientEventService eventService)
    {
        _eventService = eventService;
    }

    // [HttpSse("stream")]
    [HttpGet("stream")]
    [Consumes(SseConstants.SseContentType)]
    public async Task NotificationStream()
    {
        await StreamCreator.CreateStream(Response, _eventService);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateNotification([FromBody] NotificationDto notification)
    {
        Console.WriteLine("Received notification: " + notification.Notification);
        await _eventService.SendEventAsync(new ClientEvent(notification.Notification));
        return Ok();
    }
}
