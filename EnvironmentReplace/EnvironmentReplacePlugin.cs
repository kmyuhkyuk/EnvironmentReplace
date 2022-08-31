﻿using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;
using EFT;
using EFT.UI;
using EnvironmentReplace.Patches;

namespace EnvironmentReplace
{
    [BepInPlugin("com.kmyuhkyuk.EnvironmentReplace", "kmyuhkyuk-EnvironmentReplace", "1.3.0")]
    public class EnvironmentReplacePlugin : BaseUnityPlugin
    {
        public static GameObject prefab;

        public readonly static string modpath = AppDomain.CurrentDomain.BaseDirectory + "/BepInEx/plugins/kmyuhkyuk-EnvironmentReplace";

        private Sprite sprite;

        private string[] images;

        private SettingsData settingsdata = new SettingsData();

        public static Action<SplashScreenPanel> SplashScreenPanelReplace;

        public static Action<EnvironmentUI> EnvironmentReplace;

        public static Func<bool> EnvironmentRotate;

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-EnvironmentReplace");

            string MainSettings = "Environment Replace Settings";

            settingsdata.KeySplash = Config.Bind<bool>(MainSettings, "启动屏幕替换 Splash Screen Replace", true);
            settingsdata.KeyOriginalSplash = Config.Bind<bool>(MainSettings, "使用原始启动图片 Use Original Splash Image", true);

            settingsdata.KeyEnvironment = Config.Bind<bool>(MainSettings, "替换环境 Environment Replace", true);
            settingsdata.KeyRotate = Config.Bind<bool>(MainSettings, "环境旋转 Environment Rotate", true);

            LoadImage(modpath + "/images");
            LoadBundle();

            new SplashScreenPanelPatch().Enable();
            new EnvironmentUIPatch().Enable();
            new EnvironmentUIMainPatch().Enable();

            SplashScreenPanelReplace += SSP;
            EnvironmentReplace += Env;
            EnvironmentRotate += EnvRotate;
        }

        async void LoadBundle()
        {
            //Load AssetBundle
            var www = AssetBundle.LoadFromFileAsync(modpath + "/bundles/newenvironmentuiroot.bundle");

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

        async void LoadImage(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path.Replace("/", "\\"));

            string[] extension = new string[] { ".png", ".jpg", ".tga", ".bmp", ".psd" };

            images = directory.EnumerateFiles().Where(x => extension.Contains(x.Extension.ToLower())).Select(x => x.FullName).ToArray();

            sprite = await LoadAsyncSprite(images[UnityEngine.Random.Range(0, images.Length - 1)]);
        }

        async Task<Sprite> LoadAsyncSprite(string path)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);

            var SendWeb = www.SendWebRequest();

            while (!SendWeb.isDone)
                await Task.Yield();

            if (www.isNetworkError || www.isHttpError)
            {
                return null;
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                return sprite;
            }
        }

        void Env(EnvironmentUI envui)
        {
            if (settingsdata.KeyEnvironment.Value)
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
                    vp.url = "file://" + modpath + "/videos/" + vp.url;
                }

                //EnvironmentUI Set New EnvironmentUIRoot
                root.SetValue(newenv.GetComponent<EnvironmentUIRoot>());

                //Init New EnvironmentUIRoot
                EnvironmentUIRoot envuiroot = root.GetValue<EnvironmentUIRoot>();

                envuiroot.Init(_alignmentCamera, Events, bool_0);
                Traverse.Create(envui).Field("_environmentShading").GetValue<EnvironmentShading>().SetDefaultShading(envuiroot.Shading);
            }
        }

        bool EnvRotate()
        {
            return settingsdata.KeyRotate.Value;
        }

        void SSP(SplashScreenPanel splash)
        {
            if (settingsdata.KeySplash.Value)
            {
                Image _splashScreen = Traverse.Create(splash).Field("_splashScreen").GetValue<Image>();

                Sprite sprites;

                Sprite[] _sprites = Traverse.Create(splash).Field("_sprites").GetValue<Sprite[]>();

                if (!settingsdata.KeyOriginalSplash.Value)
                {
                    sprites = sprite;
                }
                else
                {
                    int length = images.Length + _sprites.Length;

                    int num = UnityEngine.Random.Range(0, length - 1);

                    if (num >= _sprites.Length)
                    {
                        sprites = sprite;
                    }
                    else
                    {
                        sprites = _sprites[num];
                    }
                }

                _splashScreen.sprite = sprites;
            }
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeySplash;
            public ConfigEntry<bool> KeyOriginalSplash;

            public ConfigEntry<bool> KeyEnvironment;
            public ConfigEntry<bool> KeyRotate;
        }
    }
}
