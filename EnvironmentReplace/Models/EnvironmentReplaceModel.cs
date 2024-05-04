using System;
using System.IO;
using UnityEngine.Video;

namespace EnvironmentReplace.Models
{
    internal class EnvironmentReplaceModel
    {
        private static readonly Lazy<EnvironmentReplaceModel> Lazy =
            new Lazy<EnvironmentReplaceModel>(() => new EnvironmentReplaceModel());

        public static EnvironmentReplaceModel Instance => Lazy.Value;

        public readonly string ModPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "BepInEx/plugins/kmyuhkyuk-EnvironmentReplace");

        public readonly ImageAndVideoModel SplashMedia;

        public readonly ImageAndVideoModel EnvironmentMedia;

        public VideoPlayer[] SplashVideoPlayers;

        public VideoPlayer EnvironmentVideoPlayer;

        private EnvironmentReplaceModel()
        {
            SplashMedia = new ImageAndVideoModel(Path.Combine(ModPath, "splash"));
            EnvironmentMedia = new ImageAndVideoModel(Path.Combine(ModPath, "environment"));
        }
    }
}