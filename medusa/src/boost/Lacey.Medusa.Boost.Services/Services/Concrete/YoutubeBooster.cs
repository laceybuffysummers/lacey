﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lacey.Medusa.Boost.Services.Extensions;
using Lacey.Medusa.Boost.Services.Utils;
using Lacey.Medusa.Youtube.Domain.Entities;
using Lacey.Medusa.Youtube.Services.Transfer.Services;
using Microsoft.Extensions.Logging;

namespace Lacey.Medusa.Boost.Services.Services.Concrete
{
    public sealed class YoutubeBooster : IYoutubeBooster
    {
        private readonly IYoutubeBoostProvider youtubeProvider;

        private readonly ILogger logger;

        private readonly IChannelsService channelsService;

        private readonly IVideosService videosService;

        private readonly string outputFolder;

        public YoutubeBooster(
            IYoutubeBoostProvider youtubeProvider,
            ILogger<YoutubeBooster> logger,
            string outputFolder,
            IChannelsService channelsService,
            IVideosService videosService)
        {
            this.youtubeProvider = youtubeProvider;
            this.logger = logger;
            this.outputFolder = outputFolder;
            this.channelsService = channelsService;
            this.videosService = videosService;
        }

        public async Task Boost(string channelId, int interval)
        {
            ChannelEntity localChannel;
            IReadOnlyList<VideoEntity> localVideos;
            while (true)
            {
                try
                {
                    localChannel = await this.channelsService.GetChannelMetadata(channelId);
                    localVideos = await this.videosService.GetChannelVideos(channelId);
                    break;
                }
                catch (Exception e)
                {
                    this.logger.LogError(e.Message);
                    ConsoleUtils.WaitSec(60);
                }
            }

            while (true)
            {
                try
                {
                    var randomVideo = localVideos.PickRandom();
                    var localVideo = await this.youtubeProvider.GetVideo(randomVideo.VideoId);
                    if (localVideo?.Snippet == null ||
                        localVideo.Snippet.Tags?.Count == 0)
                    {
                        continue;
                    }

                    var similarVideos = (await this.youtubeProvider.FindVideosByTags(
                        localVideo.Snippet.Tags.ToArray(), 20))
                        .Shuffle();

                    foreach (var similarVideo in similarVideos)
                    {
                        if (similarVideo == null
                            || similarVideo.Snippet.ChannelId == localChannel.OriginalChannelId
                            || similarVideo.Snippet.ChannelId == localChannel.ChannelId)
                        {
                            continue;
                        }

                        var video = await this.youtubeProvider.GetVideo(similarVideo.Id.VideoId);
                        if (video.Statistics.ViewCount > 100000 ||
                            video.Statistics.CommentCount > 1000)
                        {
                            continue;
                        }

                        await this.youtubeProvider.AddComment(
                            similarVideo.Snippet.ChannelId,
                            similarVideo.Id.VideoId,
                            localVideo.GetBoostText());
                        this.logger.LogTrace($"{similarVideo.GetYoutubeUrl()}");
                        break;
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogError(e.Message);
                }

                var sec = (interval - 1) * 60 + RandomUtils.GetRandom(0, 60);
                ConsoleUtils.WaitSec(sec);
            }
        }
    }
}