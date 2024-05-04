using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EFTUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace EnvironmentReplace.Models
{
    internal class ImageAndVideoModel
    {
        public Task<Texture2D> Texture2D;

        public string[] ImagePaths;

        public FileInfo Video;

        public string[] VideoPaths;

        public readonly string Path;

        public ImageAndVideoModel(string path)
        {
            Path = path;
        }

        public void Load()
        {
            var imagePathList = new List<string>();
            var videoPathList = new List<string>();
            foreach (var file in Directory.CreateDirectory(Path).EnumerateFiles())
            {
                if (FileExtensions.Image.Contains(file.Extension, StringComparer.OrdinalIgnoreCase))
                {
                    imagePathList.Add(file.FullName);
                }
                else if (FileExtensions.Video.Contains(file.Extension, StringComparer.OrdinalIgnoreCase))
                {
                    videoPathList.Add(file.FullName);
                }
            }

            ImagePaths = imagePathList.ToArray();
            VideoPaths = videoPathList.ToArray();

            if (ImagePaths.Length > 0)
            {
                Texture2D = UnityWebRequestHelper.GetAsyncTexture(
                    ImagePaths[UnityEngine.Random.Range(0, ImagePaths.Length)]);
            }

            if (VideoPaths.Length > 0)
            {
                Video = new FileInfo(VideoPaths[UnityEngine.Random.Range(0, VideoPaths.Length)]);
            }
        }

        public void BindImage(RawImage rawImage)
        {
            BindImage(new[] { rawImage });
        }

        public async void BindImage(RawImage[] rawImages)
        {
            if (Texture2D == null)
                return;

            var texture2D = await Texture2D;

            foreach (var rawImage in rawImages)
            {
                rawImage.texture = texture2D;
            }
        }

        public void BindVideo(VideoPlayer videoPlayer, RawImage rawImage)
        {
            BindVideo(new[] { videoPlayer }, new[] { rawImage });
        }

        public void BindVideo(VideoPlayer[] videoPlayers, RawImage[] rawImages)
        {
            if (Video == null || !Video.Exists)
                return;

            for (var i = 0; i < videoPlayers.Length; i++)
            {
                var videoPlayer = videoPlayers[i];
                var rawImage = rawImages[i];

                videoPlayer.url = Video.FullName;
                videoPlayer.started += source => rawImage.texture = source.texture;
            }
        }
    }
}