﻿using System.Linq;
using System.Threading.Tasks;
using Lacey.Medusa.Boost.Services.Extensions;
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

        public async Task Boost(string channelId)
        {
            var channel = await this.channelsService.GetChannelMetadata(channelId);
            var localVideos = await this.videosService.GetChannelVideos(channelId);

            var randomVideos = localVideos.PickRandom(3);
            foreach (var localVideo in randomVideos)
            {
                var video = await this.youtubeProvider.GetVideo(localVideo.VideoId);
                if (video.Snippet == null || 
                    video.Snippet.Tags?.Count == 0)
                {
                    continue;
                }

                var similarVideos = await this.youtubeProvider.FindVideosByTags(video.Snippet.Tags.ToArray());
                foreach (var similarVideo in similarVideos)
                {
                    if (similarVideo.Snippet.ChannelId == channel.OriginalChannelId)
                    {
                        continue;
                    }
                }
            }
        }
    }
}