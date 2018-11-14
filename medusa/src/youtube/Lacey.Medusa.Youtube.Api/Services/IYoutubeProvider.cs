﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lacey.Medusa.Youtube.Api.Base;

namespace Lacey.Medusa.Youtube.Api.Services
{
    public interface IYoutubeProvider
    {
        Task<Channel> GetChannelInfo(string channelId);

        Task<IReadOnlyList<Base.Video>> GetChannelVideos(string channelId);

        Task<string> DownloadVideo(string videoId);

        Task UploadVideo(string channelId, Base.Video video, string filePath);
    }
}