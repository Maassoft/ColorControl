namespace ColorControl.Services.Samsung
{
    public class SamsungApp
    {
        public string Title { get; set; }
        public string AppId { get; set; }

        public SamsungApp(string appId, string title)
        {
            AppId = appId;
            Title = title;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
