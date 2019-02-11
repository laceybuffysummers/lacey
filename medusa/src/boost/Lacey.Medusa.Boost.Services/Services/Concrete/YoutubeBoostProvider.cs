﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lacey.Medusa.Boost.Services.Extensions;
using Lacey.Medusa.Youtube.Api.Base;
using Lacey.Medusa.Youtube.Api.Extensions;
using Lacey.Medusa.Youtube.Api.Models.Enums;
using Lacey.Medusa.Youtube.Api.Services;
using Lacey.Medusa.Youtube.Api.Services.Concrete;
using Microsoft.Extensions.Logging;

namespace Lacey.Medusa.Boost.Services.Services.Concrete
{
    public sealed class YoutubeBoostProvider : YoutubeProvider, IYoutubeBoostProvider
    {
        private readonly ILogger logger;

        public YoutubeBoostProvider(
            IYoutubeAuthProvider youtubeAuthProvider,
            ILogger<YoutubeBoostProvider> logger) 
            : base(youtubeAuthProvider, logger)
        {
            this.logger = logger;
        }

        public async Task<IReadOnlyList<SearchResult>> FindVideosByTags(string[] tags)
        {
            var request = this.Youtube.Search.List(VideoParts.Snippet);
            request.Q = tags.ToQuery();
            request.MaxResults = 3;

            var response = await request.ExecuteAsync();
            var videos = response.Items.Where(i => i.Id.Kind == ResourceKind.Video).ToList();

            return videos;
        }

        public async Task<Video> GetVideo(string videoId)
        {
            var request = this.Youtube.Videos.List(VideoParts.AllAnonymous.AsListParam());
            request.Id = videoId;
            request.MaxResults = 1;

            var response = await request.ExecuteAsync();

            return response.Items.First();
        }
    }
}