namespace LgTv
{
    internal class RawRequestMessage
    {
        public RawRequestMessage(RequestMessage rm, int commandCount)
        {
            var prefix = (rm.Prefix ?? "");
            Id = prefix + (prefix.Length>0?"_":"") + commandCount;
            Type = rm.Type??"request";
            Uri = rm.Uri;
            Payload = rm.Payload;
        }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
        public string Payload { get; set; }
    }
}