﻿using System.Threading.Tasks;
using Lacey.Medusa.Youtube.Common.Models.Videos;

namespace Lacey.Medusa.Youtube.Common.Interfaces
{
    public interface IYoutubeDownloadVideoProvider
    {
        Task<YoutubeVideoFile> DownloadVideo(string videoId);
    }
}