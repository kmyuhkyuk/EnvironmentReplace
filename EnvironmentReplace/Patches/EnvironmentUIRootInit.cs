using EFT.UI;
using EnvironmentReplace.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace EnvironmentReplace
{
    public partial class EnvironmentReplacePlugin
    {
        private static void EnvironmentUIRootInit(EnvironmentUIRoot __instance)
        {
            var settingsModel = SettingsModel.Instance;
            var environmentReplaceModel = EnvironmentReplaceModel.Instance;

            if (!settingsModel.KeyReplaceEnvironment.Value)
                return;

            var environmentMedia = environmentReplaceModel.EnvironmentMedia;

            environmentMedia.Load();

            var texIsNull = environmentMedia.Texture2D == null;

            var videoIsNull = !(environmentMedia.Video != null && environmentMedia.Video.Exists);

            if (texIsNull && videoIsNull)
                return;

            var rawImageGameObject = new GameObject("RawImage", typeof(Canvas), typeof(CanvasScaler), typeof(RawImage),
                typeof(VideoPlayer))
            {
                layer = 25
            };

            rawImageGameObject.transform.SetParent(__instance.transform);

            var camera = __instance.GetComponentInChildren<Camera>(true);

            foreach (var behaviour in camera.GetComponents<Behaviour>())
            {
                if (behaviour.GetType() != typeof(Camera))
                {
                    behaviour.enabled = false;
                }
            }

            var layout = __instance.GetComponentInChildren<MeshRenderer>().transform;
            while (layout.parent != __instance.transform)
            {
                layout = layout.parent;
            }

            layout.gameObject.SetActive(false);

            var canvas = rawImageGameObject.GetComponent<Canvas>();

            canvas.worldCamera = camera;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.sortingOrder = -1000;

            var rawImage = rawImageGameObject.GetComponent<RawImage>();

            var environmentVideoPlayer = rawImageGameObject.GetComponent<VideoPlayer>();

            environmentVideoPlayer.SetDirectAudioVolume(0, (float)settingsModel.KeyVideoVolume.Value / 100);

            environmentVideoPlayer.isLooping = true;

            var length = environmentMedia.ImagePaths.Length + environmentMedia.VideoPaths.Length;

            var num = Random.Range(0, length);

            if (num < environmentMedia.ImagePaths.Length)
            {
                environmentMedia.BindImage(rawImage);
            }
            else
            {
                environmentMedia.BindVideo(environmentVideoPlayer, rawImage);
            }

            environmentReplaceModel.EnvironmentVideoPlayer = environmentVideoPlayer;
        }
    }
}