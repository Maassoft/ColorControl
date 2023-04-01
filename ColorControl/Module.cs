using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace ColorControl
{
    public class Module
    {
        public bool IsActive { get; set; }
        public string DisplayName { get; set; }
        [JsonIgnore]
        public Func<UserControl> InitAction { get; set; }
    }
}
