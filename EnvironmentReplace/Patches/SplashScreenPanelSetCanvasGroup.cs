using System;
using System.Collections.Generic;
using EFT.UI;
using EFTApi;
using EnvironmentReplace.Models;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace EnvironmentReplace
{
    public partial class EnvironmentReplacePlugin
    {
        private static void SplashScreenPanelSetCanvasGroup(SplashScreenPanel __instance, CanvasGroup canvasGroup,
            CanvasGroup ____imageCanvasGroup, Sprite[] ____sprites)
        {
            var settingsModel = SettingsModel.Instance;
            var environmentReplaceModel = EnvironmentReplaceModel.Instance;

            if (!settingsModel.KeyReplaceSplash.Value || canvasGroup != ____imageCanvasGroup)
                return;

            var splashMedia = environmentReplaceModel.SplashMedia;

            splashMedia.Load();

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

                videoPlayer.SetDirectAudioVolume(0, (float)settingsModel.KeyVideoVolume.Value / 100);

                videoPlayer.isLooping = true;

                videoPlayerList.Add(videoPlayer);
            }

            var rawImageArray = rawImageList.ToArray();
            var videoPlayerArray = videoPlayerList.ToArray();

            var texIsNull = splashMedia.Texture2D == null;

            var videoIsNull = !(splashMedia.Video != null && splashMedia.Video.Exists);

            if (!settingsModel.KeyRandomOriginalSplash.Value && (!texIsNull || !videoIsNull))
            {
                switch (texIsNull)
                {
                    case false when !videoIsNull:
                    {
                        var length = splashMedia.ImagePaths.Length + splashMedia.VideoPaths.Length;

                        var num = Random.Range(0, length);

                        if (num < splashMedia.ImagePaths.Length)
                        {
                            splashMedia.BindImage(rawImageArray);
                        }
                        else
                        {
                            splashMedia.BindVideo(videoPlayerArray, rawImageArray);
                        }

                        break;
                    }
                    case true:
                        splashMedia.BindVideo(videoPlayerArray, rawImageArray);
                        break;
                    default:
                        splashMedia.BindImage(rawImageArray);
                        break;
                }
            }
            else
            {
                var length = ____sprites.Length + splashMedia.ImagePaths.Length + splashMedia.VideoPaths.Length;

                var num = Random.Range(0, length);

                if (num < ____sprites.Length)
                {
                    var texture2D = ____sprites[num].texture;

                    foreach (var rawImage in rawImageArray)
                    {
                        rawImage.texture = texture2D;
                    }
                }
                else if (num < ____sprites.Length + splashMedia.ImagePaths.Length)
                {
                    splashMedia.BindImage(rawImageArray);
                }
                else
                {
                    splashMedia.BindVideo(videoPlayerArray, rawImageArray);
                }
            }

            environmentReplaceModel.SplashVideoPlayers = videoPlayerArray;
        }
    }
}