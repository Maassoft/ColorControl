namespace LgTv
{
    public class LgApp
    {
        public string title { get; set; }
        public string appId { get; set; }

        public LgApp(string appId, string title)
        {
            this.appId = appId;
            this.title = title;
        }

        public override string ToString()
        {
            return title;
        }
    }
}
