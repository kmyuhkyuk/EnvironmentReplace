using BepInEx;
using BepInEx.Configuration;
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
    [BepInPlugin("com.kmyuhkyuk.EnvironmentReplace", "kmyuhkyuk-EnvironmentReplace", "1.3.1")]
    public class EnvironmentReplacePlugin : BaseUnityPlugin
    {
        private readonly string ModPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BepInEx/plugins/kmyuhkyuk-EnvironmentReplace") ;

        private GameObject EnvironmentPrefab;

        private Sprite SplashSprite;

        private string[] SplashImages;

        private readonly SettingsData SettingsDatas = new SettingsData();

        internal delegate void RefEnvironment(Transform envui, Camera _alignmentCamera, EEventType[] Events, bool bool_0, EnvironmentShading _environmentShading, ref EnvironmentUIRoot envuiroot);

        internal delegate void RefSplashScreePanel(Sprite[] _sprites, ref Image _splashScreen);

        internal static RefEnvironment EnvironmentReplace;

        internal static RefSplashScreePanel SplashScreenPanelReplace;

        internal static Func<bool> EnvironmentRotate;

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-EnvironmentReplace");

            const string mainSettings = "Environment Replace Settings";

            SettingsDatas.KeySplash = Config.Bind<bool>(mainSettings, "启动屏幕替换 Splash Screen Replace", true);
            SettingsDatas.KeyOriginalSplash = Config.Bind<bool>(mainSettings, "使用原始启动图片 Use Original Splash Image", true);

            SettingsDatas.KeyEnvironment = Config.Bind<bool>(mainSettings, "替换环境 Environment Replace", true);
            SettingsDatas.KeyRotate = Config.Bind<bool>(mainSettings, "环境旋转 Environment Rotate", true);
            SettingsDatas.KeyBundleName = Config.Bind<string>(mainSettings, "Environment Bundle Name", "newenvironmentuiroot.bundle");

            LoadImage(Path.Combine(ModPath, "images"));
            LoadBundle();

            new SplashScreenPanelPatch().Enable();
            new EnvironmentUIPatch().Enable();
            new EnvironmentUIMainPatch().Enable();

            SplashScreenPanelReplace = SSP;
            EnvironmentReplace = Env;
            EnvironmentRotate = EnvRotate;
        }

        async void LoadBundle()
        {
            //Load AssetBundle
            var www = AssetBundle.LoadFromFileAsync(Path.Combine(ModPath, "bundles", SettingsDatas.KeyBundleName.Value));

            while (!www.isDone)
                await Task.Yield();

            if (www.assetBundle == null)
            {
                Logger.LogError("Failed to load AssetBundle!");
            }
            else
            {
                EnvironmentPrefab = www.assetBundle.LoadAllAssets<GameObject>()[0];

                www.assetBundle.Unload(false);
            }
        }

        async void LoadImage(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(@path);

            string[] extensions = new string[] { ".png", ".jpg", ".tga", ".bmp", ".psd" };

            SplashImages = directory.EnumerateFiles().Where(x => extensions.Contains(x.Extension.ToLower())).Select(x => x.FullName).ToArray();

            if (SplashImages.Length > 0)
            {
                SplashSprite = await LoadAsyncSprite(SplashImages[UnityEngine.Random.Range(0, SplashImages.Length - 1)]);
            }
        }

        async Task<Sprite> LoadAsyncSprite(string path)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);

            var sendWeb = www.SendWebRequest();

            while (!sendWeb.isDone)
                await Task.Yield();

            if (www.isNetworkError || www.isHttpError)
            {
                return null;
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }

        void Env(Transform envui, Camera _alignmentCamera, EEventType[] Events, bool bool_0, EnvironmentShading _environmentShading, ref EnvironmentUIRoot envuiroot)
        {
            if (SettingsDatas.KeyEnvironment.Value)
            {
                //Remove Orgin Environment GameObject
                if (envuiroot != null)
                {
                    Destroy(envuiroot.gameObject);
                }

                //Create New Environment
                GameObject newEnv = Instantiate(EnvironmentPrefab, envui);

                //Video Local Paths Url Replace
                foreach (VideoPlayer vp in newEnv.GetComponentsInChildren<VideoPlayer>())
                {
                    vp.url = string.Concat("file://", Path.Combine(ModPath, "videos", vp.url));
                }

                //EnvironmentUI Set New EnvironmentUIRoot
                envuiroot = newEnv.GetComponent<EnvironmentUIRoot>();

                //Init New EnvironmentUIRoot

                envuiroot.Init(_alignmentCamera, Events, bool_0);
                _environmentShading.SetDefaultShading(envuiroot.Shading);
            }
        }

        bool EnvRotate()
        {
            return SettingsDatas.KeyRotate.Value;
        }

        void SSP(Sprite[] _sprites, ref Image _splashScreen)
        {
            if (SettingsDatas.KeySplash.Value)
            {
                Sprite sprites;

                if (!SettingsDatas.KeyOriginalSplash.Value && SplashSprite != null)
                {
                    sprites = SplashSprite;
                }
                else
                {
                    int length = SplashImages.Length + _sprites.Length;

                    int num = UnityEngine.Random.Range(0, length - 1);

                    if (num >= _sprites.Length)
                    {
                        sprites = SplashSprite;
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

            public ConfigEntry<string> KeyBundleName;
        }
    }
}
