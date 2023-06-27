using Newtonsoft.Json;

namespace ColorControl.Shared.Contracts
{
    public class Module
    {
        public bool IsActive { get; set; }
        public string DisplayName { get; set; }
        [JsonIgnore]
        public Func<UserControl> InitAction { get; set; }
    }
}
