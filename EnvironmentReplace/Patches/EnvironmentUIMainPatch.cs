using Aki.Reflection.Patching;
using System.Reflection;
using EFT.UI;

namespace EnvironmentReplace.Patches
{
    public class EnvironmentUIMainPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EnvironmentUI).GetMethod("SetAsMain", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static void PatchPrefix(ref bool isMain)
        {
            if (!EnvironmentReplacePlugin.KeyRotate.Value)
            {
                isMain = true;
            }  
        }
    }
}
