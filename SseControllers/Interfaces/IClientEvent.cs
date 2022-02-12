using System.Collections.Generic;

namespace SseControllers.Interfaces
{
    public interface IClientEvent
    {
        string Id { get; set; }
        string Type { get; set; }
        IList<string> Data { get; }
    }
}
