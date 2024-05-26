using BepInEx;
using EnvironmentReplace.Attributes;
using EnvironmentReplace.Models;
using HarmonyLib;
using static EFTApi.EFTHelpers;

namespace EnvironmentReplace
{
    [BepInPlugin("com.kmyuhkyuk.EnvironmentReplace", "EnvironmentReplace", "1.4.3")]
    [BepInDependency("com.kmyuhkyuk.EFTApi", "1.2.1")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/759-environment-replace")]
    public partial class EnvironmentReplacePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            var settingsModel = SettingsModel.Create(Config);

            EnvironmentReplaceModel.Create();

            settingsModel.KeyVideoVolume.SettingChanged += (sender, args) =>
            {
                var environmentReplaceModel = EnvironmentReplaceModel.Instance;

                var volume = (float)settingsModel.KeyVideoVolume.Value / 100;

                if (environmentReplaceModel.SplashVideoPlayers != null)
                {
                    foreach (var splashVideoPlayer in environmentReplaceModel.SplashVideoPlayers)
                    {
                        splashVideoPlayer.SetDirectAudioVolume(0, volume);
                    }
                }

                if (environmentReplaceModel.EnvironmentVideoPlayer == null)
                    return;

                environmentReplaceModel.EnvironmentVideoPlayer.SetDirectAudioVolume(0, volume);
            };
        }

        private void Start()
        {
            _EnvironmentUIRootHelper.Init.Add(this, nameof(EnvironmentUIRootInit), HarmonyPatchType.Prefix);

            ReflectionModel.Instance.SplashScreenPanelSetCanvasGroup.Add(this, nameof(SplashScreenPanelSetCanvasGroup),
                HarmonyPatchType.Prefix);
        }
    }
}