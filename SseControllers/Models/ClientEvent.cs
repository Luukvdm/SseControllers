using System.Collections.Generic;
using SseControllers.Interfaces;

namespace SseControllers.Models;

public class ClientEvent : IClientEvent
{
    public ClientEvent(string id, string type, IList<string> data)
    {
        Id = id;
        Type = type;
        Data = data;
    }

    public ClientEvent(string type, string data) : this(type, new List<string> {data}) { }

    public ClientEvent(string type, IList<string> data)
    {
        Type = type;
        Data = data;
    }

    public ClientEvent(string data)
    {
        Data = new List<string> {data};
    }

    public string Id { get; set; }
    public string Type { get; set; }
    public IList<string> Data { get; }
}