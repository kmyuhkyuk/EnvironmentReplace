using BepInEx.Configuration;

namespace EnvironmentReplace.Models
{
    internal class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyReplaceSplash;
        public readonly ConfigEntry<bool> KeyRandomOriginalSplash;

        public readonly ConfigEntry<bool> KeyReplaceEnvironment;

        public readonly ConfigEntry<int> KeyVideoVolume;

        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Environment Replace Settings";

            KeyReplaceSplash = configFile.Bind<bool>(mainSettings, "Splash Screen Replace", true);
            KeyRandomOriginalSplash = configFile.Bind<bool>(mainSettings, "Random Original Splash Image", true);

            KeyReplaceEnvironment = configFile.Bind<bool>(mainSettings, "Environment Replace", true);

            KeyVideoVolume = configFile.Bind<int>(mainSettings, "Video Volume", 100,
                new ConfigDescription(string.Empty, new AcceptableValueRange<int>(0, 100)));
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static SettingsModel Create(ConfigFile configFile)
        {
            if (Instance != null)
                return Instance;

            return Instance = new SettingsModel(configFile);
        }
    }
}