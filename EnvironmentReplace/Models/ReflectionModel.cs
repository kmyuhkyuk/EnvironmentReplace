using System;
using System.Collections;
using EFT.UI;
using EFTReflection;

namespace EnvironmentReplace.Models
{
    internal class ReflectionModel
    {
        private static readonly Lazy<ReflectionModel> Lazy = new Lazy<ReflectionModel>(() => new ReflectionModel());

        public static ReflectionModel Instance => Lazy.Value;

        public readonly RefHelper.HookRef SplashScreenPanelSetCanvasGroup;

        public ReflectionModel()
        {
            SplashScreenPanelSetCanvasGroup = RefHelper.HookRef.Create(typeof(SplashScreenPanel),
                x => x.ReturnType == typeof(IEnumerator) && x.GetParameters().Length == 1);
        }
    }
}