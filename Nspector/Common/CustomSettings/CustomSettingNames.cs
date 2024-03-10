using nspector.Common.Helper;
using System.Text;

namespace nspector.Common.CustomSettings
{
    [Serializable]
    public class CustomSettingNames
    {
        public List<CustomSetting> Settings = new List<CustomSetting>();

        public void StoreToFile(string filename)
        {
            XMLHelper<CustomSettingNames>.SerializeToXmlFile(this, filename, Encoding.Unicode, true);
        }

        public static CustomSettingNames FactoryLoadFromFile(string filename)
        {
            var settings = XMLHelper<CustomSettingNames>.DeserializeFromXMLFile(filename);

            var interpolatedSettings = settings.Settings.Where(s => s.SettingValues.Count == 2 && s.SettingValues[1].SettingValue <= 1200 && (s.SettingValues[1].SettingValue - s.SettingValues[0].SettingValue) >= 50).ToList();

            foreach (var setting in interpolatedSettings)
            {
                var maxSetting = setting.SettingValues[1].SettingValue;

                setting.SettingValues.RemoveAt(1);

                for (var i = setting.SettingValues[0].SettingValue + 1; i <= maxSetting; i++)
                {
                    setting.SettingValues.Add(new CustomSettingValue
                    {
                        HexValue = i.ToString("X"),
                        UserfriendlyName = i.ToString(),
                    });
                }
            }

            return settings;
        }

        public static CustomSettingNames FactoryLoadFromString(string xml)
        {
            return XMLHelper<CustomSettingNames>.DeserializeFromXmlString(xml);
        }
    }
}
