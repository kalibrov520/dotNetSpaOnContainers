using System;
using Newtonsoft.Json;

namespace EventBus.Abstractions
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        [JsonProperty] public Guid Id { get; set; }
        [JsonProperty] public DateTime CreationDate { get; set; }
    }
}