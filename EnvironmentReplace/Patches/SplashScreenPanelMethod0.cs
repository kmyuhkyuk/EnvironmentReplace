using System;
using System.Collections.Generic;
using EFT.UI;
using EFTApi;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace EnvironmentReplace
{
    public partial class EnvironmentReplacePlugin
    {
        private static void SplashScreenPanelMethod0(SplashScreenPanel __instance, CanvasGroup canvasGroup,
            CanvasGroup ____imageCanvasGroup, Sprite[] ____sprites)
        {
            if (canvasGroup == ____imageCanvasGroup && _settingsData.KeyReplaceSplash.Value)
            {
                _splash.Load();

                var splashScreenImages = EFTVersion.AkiVersion > Version.Parse("3.6.1")
                    ? Traverse.Create(__instance).Field("_images").GetValue<Image[]>()
                    : new[] { Traverse.Create(__instance).Field("_splashScreen").GetValue<Image>() };

                var rawImageList = new List<RawImage>();
                var videoPlayerList = new List<VideoPlayer>();
                foreach (var splashScreenImage in splashScreenImages)
                {
                    var splashScreen = splashScreenImage.gameObject;

                    DestroyImmediate(splashScreenImage);

                    var rawImage = splashScreen.AddComponent<RawImage>();

                    rawImageList.Add(rawImage);

                    var videoPlayer = splashScreen.AddComponent<VideoPlayer>();

                    videoPlayer.SetDirectAudioVolume(0, (float)_settingsData.KeyVideoVolume.Value / 100);

                    videoPlayer.isLooping = true;

                    videoPlayerList.Add(videoPlayer);
                }

                var rawImageArray = rawImageList.ToArray();
                var videoPlayerArray = videoPlayerList.ToArray();

                _splashVideoPlayers = videoPlayerArray;

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
                                _splash.BindImage(rawImageArray);
                            }
                            else
                            {
                                _splash.BindVideo(videoPlayerArray, rawImageArray);
                            }

                            break;
                        }
                        case true:
                            _splash.BindVideo(videoPlayerArray, rawImageArray);
                            break;
                        default:
                            _splash.BindImage(rawImageArray);
                            break;
                    }
                }
                else
                {
                    var length = ____sprites.Length + _splash.ImagePaths.Length + _splash.VideoPaths.Length;

                    var num = Random.Range(0, length);

                    if (num < ____sprites.Length)
                    {
                        var texture2D = ____sprites[num].texture;

                        foreach (var rawImage in rawImageArray)
                        {
                            rawImage.texture = texture2D;
                        }
                    }
                    else if (num < ____sprites.Length + _splash.ImagePaths.Length)
                    {
                        _splash.BindImage(rawImageArray);
                    }
                    else
                    {
                        _splash.BindVideo(videoPlayerArray, rawImageArray);
                    }
                }
            }
        }
    }
}