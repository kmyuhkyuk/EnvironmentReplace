using System.IO;
using UnityEngine.Video;

namespace EnvironmentReplace.Models
{
    internal class EnvironmentReplaceModel
    {
        public static EnvironmentReplaceModel Instance { get; private set; }

        public readonly string ModPath = Path.Combine(BepInEx.Paths.PluginPath, "kmyuhkyuk-EnvironmentReplace");

        public readonly ImageAndVideoModel SplashMedia;

        public readonly ImageAndVideoModel EnvironmentMedia;

        public VideoPlayer[] SplashVideoPlayers;

        public VideoPlayer EnvironmentVideoPlayer;

        private EnvironmentReplaceModel()
        {
            SplashMedia = new ImageAndVideoModel(Path.Combine(ModPath, "splash"));
            EnvironmentMedia = new ImageAndVideoModel(Path.Combine(ModPath, "environment"));
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static EnvironmentReplaceModel Create()
        {
            if (Instance != null)
                return Instance;

            return Instance = new EnvironmentReplaceModel();
        }
    }
}