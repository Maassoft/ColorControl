
using Newtonsoft.Json;

namespace ColorControl.Services.Samsung
{
    internal class RawRequestMessage
    {
        public RawRequestMessage(RequestMessage rm, int commandCount)
        {
            Id = commandCount.ToString();
            Method = rm.Method;
            Params = rm.Params;
        }

        [JsonIgnore]
        public string Id { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("params")]
        public object Params { get; set; }
    }
}