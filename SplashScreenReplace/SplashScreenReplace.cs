using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using EFT;
using EFT.UI;

namespace SplashScreenReplace
{
    [BepInPlugin("com.kmyuhkyuk.SplashScreenReplace", "kmyuhkyuk-SplashScreenReplace", "1.2.1")]
    public class SplashScreenReplaceePlugin : BaseUnityPlugin
    {
        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-EnvironmentReplace");
        }
    }
}
