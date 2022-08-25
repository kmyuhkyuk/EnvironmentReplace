using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using EFT;
using EFT.UI;
using EnvironmentReplace.Patches;

namespace EnvironmentReplace
{
    [BepInPlugin("com.kmyuhkyuk.EnvironmentReplace", "kmyuhkyuk-EnvironmentReplace", "1.2.0")]
    public class EnvironmentReplacePlugin : BaseUnityPlugin
    {
        public static GameObject prefab;

        public readonly static string videospath = "file://" + AppDomain.CurrentDomain.BaseDirectory + "/BepInEx/plugins/kmyuhkyuk-EnvironmentReplace/videos/";

        public static ConfigEntry<bool> KeyEnvironment;
        public static ConfigEntry<bool> KeyRotate;

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-EnvironmentReplace");

            string MainSettings = "Environment Replace Settings";

            KeyEnvironment = Config.Bind<bool>(MainSettings, "替换环境 Environment Replace", true);
            KeyRotate = Config.Bind<bool>(MainSettings, "环境旋转 Environment Rotate", true);

            LoadBundle();

            new EnvironmentUIPatch().Enable();
            new EnvironmentUIMainPatch().Enable();
        }
        async void LoadBundle()
        {
            //Load AssetBundle
            var www = AssetBundle.LoadFromFileAsync(AppDomain.CurrentDomain.BaseDirectory + "/BepInEx/plugins/kmyuhkyuk-EnvironmentReplace/bundles/newenvironmentuiroot.bundle");

            while (!www.isDone)
                await Task.Yield();

            if (www.assetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
            }
            else
            {
                prefab = www.assetBundle.LoadAsset<GameObject>("newenvironmentuiroot");

                www.assetBundle.Unload(false);
            }
        }

        public static void Env(EnvironmentUI envui, EnvironmentShading envshading)
        {
            Traverse root = Traverse.Create(envui).Field("environmentUIRoot_0");

            //Remove Orgin Environment GameObject
            Destroy(root.GetValue<EnvironmentUIRoot>().gameObject);

            //From EnvironmentUI Get alignmentCamera, Events, bool_0
            Camera _alignmentCamera = Traverse.Create(envui).Field("_alignmentCamera").GetValue<Camera>();

            EEventType[] Events = Traverse.Create(envui).Property("Events").GetValue<EEventType[]>();

            bool bool_0 = Traverse.Create(envui).Field("bool_0").GetValue<bool>();

            //Create New Environment
            GameObject newenv = Instantiate(prefab, envui.transform);

            //Video Local Paths Url Replace
            foreach (VideoPlayer vp in newenv.GetComponentsInChildren<VideoPlayer>())
            {
                vp.url = videospath + vp.url;
            }

            //EnvironmentUI Set New EnvironmentUIRoot
            root.SetValue(newenv.GetComponent<EnvironmentUIRoot>());

            //Init New EnvironmentUIRoot
            EnvironmentUIRoot envuiroot = root.GetValue<EnvironmentUIRoot>();

            envuiroot.Init(_alignmentCamera, Events, bool_0);
            envshading.SetDefaultShading(envuiroot.Shading);
        }
    }
}
