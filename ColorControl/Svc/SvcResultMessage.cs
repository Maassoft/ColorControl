namespace ColorControl.Svc
{
    public class SvcResultMessage
    {
        public bool Result { get; set; }

        public string Data { get; set; }

        public string ErrorMessage { get; set; }


        public static SvcResultMessage FromResult(bool result, string errorMessage = null) => new SvcResultMessage { Result = result, ErrorMessage = errorMessage };
        public static SvcResultMessage FromResult(string data) => new SvcResultMessage { Result = true, Data = data };
    }
}
