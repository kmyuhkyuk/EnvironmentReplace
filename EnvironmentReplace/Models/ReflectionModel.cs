using System;
using System.Collections;
using EFT.UI;
using EFTApi;
using EFTReflection;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace EnvironmentReplace.Models
{
    internal class ReflectionModel
    {
        private static readonly Lazy<ReflectionModel> Lazy = new Lazy<ReflectionModel>(() => new ReflectionModel());

        public static ReflectionModel Instance => Lazy.Value;

        [CanBeNull] public readonly RefHelper.FieldRef<SplashScreenPanel, Image[]> RefImages;

        [CanBeNull] public readonly RefHelper.FieldRef<SplashScreenPanel, Image> RefSplashScreen;

        public readonly RefHelper.HookRef SplashScreenPanelSetCanvasGroup;

        public ReflectionModel()
        {
            if (EFTVersion.AkiVersion > Version.Parse("3.6.1"))
            {
                RefImages = RefHelper.FieldRef<SplashScreenPanel, Image[]>.Create("_images");
            }
            else
            {
                RefSplashScreen = RefHelper.FieldRef<SplashScreenPanel, Image>.Create("_splashScreen");
            }

            SplashScreenPanelSetCanvasGroup = RefHelper.HookRef.Create(typeof(SplashScreenPanel),
                x => x.ReturnType == typeof(IEnumerator) && x.GetParameters().Length == 1);
        }
    }
}