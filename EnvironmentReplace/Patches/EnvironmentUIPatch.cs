using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;
using System.Threading.Tasks;
using EFT.UI;

namespace EnvironmentReplace.Patches
{
    public class EnvironmentUIPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EnvironmentUI).GetMethod("method_0", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static async void PatchPostfix(Task __result, EnvironmentUI __instance)
        {
            await __result;

            EnvironmentReplacePlugin.EnvironmentReplace(__instance);
        }
    }
}
