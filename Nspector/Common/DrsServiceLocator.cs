using nspector.Common.CustomSettings;
using System.Reflection;

namespace nspector.Common
{
    public class DrsServiceLocator
    {
        private static readonly CustomSettingNames CustomSettings;
        public static readonly CustomSettingNames ReferenceSettings;
        public static readonly DrsSettingsMetaService MetaService;
        public static readonly DrsImportService ImportService;
        public static readonly DrsSettingsService SettingService;
        public static readonly DrsScannerService ScannerService;
        public static readonly DrsDecrypterService DecrypterService;

        static DrsServiceLocator()
        {
            CustomSettings = LoadCustomSettings();
            ReferenceSettings = LoadReferenceSettings();

            MetaService = new DrsSettingsMetaService(CustomSettings, ReferenceSettings);
            DecrypterService = new DrsDecrypterService(MetaService);
            ScannerService = new DrsScannerService(MetaService, DecrypterService);
            SettingService = new DrsSettingsService(MetaService, DecrypterService);
            ImportService = new DrsImportService(MetaService, SettingService, ScannerService, DecrypterService);
        }

        private static CustomSettingNames LoadCustomSettings()
        {
            string csnDefaultPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\nspector\\CustomSettingNames.xml";

            if (File.Exists(csnDefaultPath))
                return CustomSettingNames.FactoryLoadFromFile(csnDefaultPath);
            else
                return new CustomSettingNames(); //return CustomSettingNames.FactoryLoadFromString(Properties.Resources.CustomSettingNames);
        }

        private static CustomSettingNames LoadReferenceSettings()
        {
            string csnDefaultPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\nspector\\Reference.xml";

            if (File.Exists(csnDefaultPath))
                return CustomSettingNames.FactoryLoadFromFile(csnDefaultPath);

            return null;
        }

    }
}
