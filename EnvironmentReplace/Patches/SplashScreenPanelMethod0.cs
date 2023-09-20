using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace EnvironmentReplace
{
    public partial class EnvironmentReplacePlugin
    {
        private static void SplashScreenPanelMethod0(CanvasGroup canvasGroup, CanvasGroup ____imageCanvasGroup, Sprite[] ____sprites, Image ____splashScreen)
        {
            if (canvasGroup == ____imageCanvasGroup && _settingsData.KeyReplaceSplash.Value)
            {
                _splash.Load();

                var splashScreen = ____splashScreen.gameObject;

                DestroyImmediate(____splashScreen);

                var rawImage = splashScreen.AddComponent<RawImage>();

                _splashVideoPlayer = splashScreen.AddComponent<VideoPlayer>();

                _splashVideoPlayer.SetDirectAudioVolume(0, (float)_settingsData.KeyVideoVolume.Value / 100);

                _splashVideoPlayer.isLooping = true;

                var texIsNull = _splash.Texture2D == null;

                var videoIsNull = !(_splash.Video != null && _splash.Video.Exists);

                if (!_settingsData.KeyRandomOriginalSplash.Value && (!texIsNull || !videoIsNull))
                {
                    switch (texIsNull)
                    {
                        case false when !videoIsNull:
                        {
                            var length = _splash.ImagePaths.Length + _splash.VideoPaths.Length;

                            var num = Random.Range(0, length);

                            if (num < _splash.ImagePaths.Length)
                            {
                                _splash.BindImage(rawImage);
                            }
                            else
                            {
                                _splash.BindVideo(_splashVideoPlayer, rawImage);
                            }

                            break;
                        }
                        case true:
                            _splash.BindVideo(_splashVideoPlayer, rawImage);
                            break;
                        default:
                            _splash.BindImage(rawImage);
                            break;
                    }
                }
                else
                {
                    var length = ____sprites.Length + _splash.ImagePaths.Length + _splash.VideoPaths.Length;

                    var num = Random.Range(0, length);

                    if (num < ____sprites.Length)
                    {
                        rawImage.texture = ____sprites[num].texture;
                    }
                    else if (num < ____sprites.Length + _splash.ImagePaths.Length)
                    {
                        _splash.BindImage(rawImage);
                    }
                    else
                    {
                        _splash.BindVideo(_splashVideoPlayer, rawImage);
                    }
                }
            }
        }
    }
}