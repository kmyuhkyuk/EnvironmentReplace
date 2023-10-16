using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using EFT.UI;
using EFTReflection;
using EFTUtils;
using EnvironmentReplace.Attributes;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static EFTApi.EFTHelpers;

namespace EnvironmentReplace
{
    [BepInPlugin("com.kmyuhkyuk.EnvironmentReplace", "kmyuhkyuk-EnvironmentReplace", "1.4.2")]
    [BepInDependency("com.kmyuhkyuk.EFTApi", "1.1.7")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/759-environment-replace")]
    public partial class EnvironmentReplacePlugin : BaseUnityPlugin
    {
        private static readonly string ModPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "BepInEx/plugins/kmyuhkyuk-EnvironmentReplace");

        private static ImageAndVideo _splash;

        private static ImageAndVideo _environment;

        private static VideoPlayer[] _splashVideoPlayers;

        private static VideoPlayer _environmentVideoPlayer;

        private static SettingsData _settingsData;

        private readonly ReflectionData _reflectionData = new ReflectionData();

        public EnvironmentReplacePlugin()
        {
            _settingsData = new SettingsData(Config);

            _splash = new ImageAndVideo(Path.Combine(ModPath, "splash"));
            _environment = new ImageAndVideo(Path.Combine(ModPath, "environment"));

            _settingsData.KeyVideoVolume.SettingChanged += (sender, args) =>
            {
                var volume = (float)_settingsData.KeyVideoVolume.Value / 100;

                if (_splashVideoPlayers != null)
                {
                    foreach (var splashVideoPlayer in _splashVideoPlayers)
                    {
                        splashVideoPlayer.SetDirectAudioVolume(0, volume);
                    }
                }

                if (_environmentVideoPlayer != null)
                {
                    _environmentVideoPlayer.SetDirectAudioVolume(0, volume);
                }
            };
        }

        private void Start()
        {
            _EnvironmentUIRootHelper.Init.Add(this, nameof(EnvironmentUIRootInit), HarmonyPatchType.Prefix);
            _reflectionData.SplashScreenPanelMethod0.Add(this, nameof(SplashScreenPanelMethod0),
                HarmonyPatchType.Prefix);
        }

        public class ImageAndVideo
        {
            public Task<Texture2D> Texture2D;

            public string[] ImagePaths;

            public FileInfo Video;

            public string[] VideoPaths;

            public readonly string Path;

            public ImageAndVideo(string path)
            {
                Path = path;
            }

            public void Load()
            {
                var directory = new DirectoryInfo(Path);

                if (!directory.Exists)
                {
                    directory.Create();
                }
                else
                {
                    var files = directory.GetFiles();

                    ImagePaths = files.Where(x =>
                            FileExtensions.Image.Contains(x.Extension, StringComparer.OrdinalIgnoreCase))
                        .Select(x => x.FullName).ToArray();

                    VideoPaths = files.Where(x =>
                            FileExtensions.Video.Contains(x.Extension, StringComparer.OrdinalIgnoreCase))
                        .Select(x => x.FullName).ToArray();

                    if (ImagePaths.Length > 0)
                    {
                        Texture2D = UnityWebRequestHelper.GetAsyncTexture(
                            ImagePaths[UnityEngine.Random.Range(0, ImagePaths.Length)]);
                    }

                    if (VideoPaths.Length > 0)
                    {
                        Video = new FileInfo(VideoPaths[UnityEngine.Random.Range(0, VideoPaths.Length)]);
                    }
                }
            }

            public void BindImage(RawImage rawImage)
            {
                BindImage(new[] { rawImage });
            }

            public async void BindImage(RawImage[] rawImages)
            {
                if (Texture2D == null)
                    return;

                var texture2D = await Texture2D;

                foreach (var rawImage in rawImages)
                {
                    rawImage.texture = texture2D;
                }
            }

            public void BindVideo(VideoPlayer videoPlayer, RawImage rawImage)
            {
                BindVideo(new[] { videoPlayer }, new[] { rawImage });
            }

            public void BindVideo(VideoPlayer[] videoPlayers, RawImage[] rawImages)
            {
                if (Video == null || !Video.Exists)
                    return;

                for (var i = 0; i < videoPlayers.Length; i++)
                {
                    var videoPlayer = videoPlayers[i];
                    var rawImage = rawImages[i];

                    videoPlayer.url = Video.FullName;
                    videoPlayer.started += source => rawImage.texture = source.texture;
                }
            }
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyReplaceSplash;
            public ConfigEntry<bool> KeyRandomOriginalSplash;

            public ConfigEntry<bool> KeyReplaceEnvironment;

            public ConfigEntry<int> KeyVideoVolume;

            public SettingsData(ConfigFile configFile)
            {
                const string mainSettings = "Environment Replace Settings";

                KeyReplaceSplash = configFile.Bind<bool>(mainSettings, "Splash Screen Replace", true);
                KeyRandomOriginalSplash = configFile.Bind<bool>(mainSettings, "Random Original Splash Image", true);

                KeyReplaceEnvironment = configFile.Bind<bool>(mainSettings, "Environment Replace", true);

                KeyVideoVolume = configFile.Bind<int>(mainSettings, "Video Volume", 100,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<int>(0, 100)));
            }
        }

        private class ReflectionData
        {
            public readonly RefHelper.HookRef SplashScreenPanelMethod0;

            public ReflectionData()
            {
                SplashScreenPanelMethod0 = RefHelper.HookRef.Create(typeof(SplashScreenPanel),
                    x => x.ReturnType == typeof(IEnumerator) && x.GetParameters().Length == 1);
            }
        }
    }
}