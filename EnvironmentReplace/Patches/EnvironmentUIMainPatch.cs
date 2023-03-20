using Aki.Reflection.Patching;
using System.Reflection;
using EFT.UI;

namespace EnvironmentReplace.Patches
{
    public class EnvironmentUIMainPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EnvironmentUIRoot).GetMethod("RandomRotate", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static bool PatchPrefix(EnvironmentUIRoot __instance)
        {
            if (!EnvironmentReplacePlugin.OpenEnvironmentRotate())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
