using EFT.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace EnvironmentReplace
{
    public partial class EnvironmentReplacePlugin
    {
        private static void EnvironmentUIRootInit(EnvironmentUIRoot __instance)
        {
            if (_settingsData.KeyReplaceEnvironment.Value)
            {
                _environment.Load();

                var texIsNull = _environment.Texture2D == null;

                var videoIsNull = !(_environment.Video != null && _environment.Video.Exists);

                if (texIsNull && videoIsNull)
                    return;

                var gameObject = new GameObject("RawImage", typeof(Canvas), typeof(CanvasScaler), typeof(RawImage),
                    typeof(VideoPlayer));

                gameObject.transform.SetParent(__instance.transform);

                var canvas = gameObject.GetComponent<Canvas>();

                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = -1;
                canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                                  AdditionalCanvasShaderChannels.Normal |
                                                  AdditionalCanvasShaderChannels.Tangent;

                var rawImage = gameObject.GetComponent<RawImage>();

                _environmentVideoPlayer = gameObject.GetComponent<VideoPlayer>();

                _environmentVideoPlayer.SetDirectAudioVolume(0, (float)_settingsData.KeyVideoVolume.Value / 100);

                _environmentVideoPlayer.isLooping = true;

                var length = _environment.ImagePaths.Length + _environment.VideoPaths.Length;

                var num = Random.Range(0, length);

                if (num < _environment.ImagePaths.Length)
                {
                    _environment.BindImage(rawImage);
                }
                else
                {
                    _environment.BindVideo(_environmentVideoPlayer, rawImage);
                }
            }
        }
    }
}