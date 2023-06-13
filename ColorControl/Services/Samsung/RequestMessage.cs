namespace ColorControl.Services.Samsung
{
    public class RequestMessage
    {
        public RequestMessage(string method, object payload)
        {
            Method = method;
            Params = payload;
        }

        public string Method { get; set; }
        public object Params { get; set; }
    }
}