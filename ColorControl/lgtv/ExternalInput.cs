namespace LgTv
{
    public class ExternalInput
    {
        public ExternalInput(string id,string label)
        {
            Id = id;
            Label = label;
        }
        public string Id { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }

    }
}