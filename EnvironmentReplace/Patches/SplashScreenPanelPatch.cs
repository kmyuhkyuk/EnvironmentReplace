using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;
using EFT.UI;
using UnityEngine;
using UnityEngine.UI;

namespace EnvironmentReplace.Patches
{
    public class SplashScreenPanelPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(SplashScreenPanel).GetMethod("method_0", PatchConstants.PrivateFlags);
        }

        [PatchPrefix]
        private static void PatchPrefix(Sprite[] ____sprites, ref Image ____splashScreen)
        {
            EnvironmentReplacePlugin.SplashScreenPanelReplace(____sprites, ref ____splashScreen);
        }
    }
}
