using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using EFT;
using EFT.UI;

namespace EnvironmentReplace.Patches
{
    public class EnvironmentUIPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EnvironmentUI).GetMethod("method_0", PatchConstants.PrivateFlags);
        }

        [PatchPrefix]
        private static bool PatchPrefix(EnvironmentUI __instance, Camera ____alignmentCamera, bool ___bool_0, EnvironmentShading ____environmentShading, ref EnvironmentUIRoot ___environmentUIRoot_0)
        {
            EnvironmentReplacePlugin.EnvironmentReplace(__instance.transform, ____alignmentCamera, Traverse.Create(__instance).Property("Events").GetValue<EEventType[]>(), ___bool_0, ____environmentShading, ref ___environmentUIRoot_0);

            return false;
        }
    }
}
