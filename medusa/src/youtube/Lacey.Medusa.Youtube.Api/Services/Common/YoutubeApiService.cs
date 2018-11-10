﻿using AutoMapper;
using Lacey.Medusa.Common.Api.Base.Services;
using Lacey.Medusa.Youtube.Api.Base;
using Lacey.Medusa.Youtube.Api.Services.Auth;

namespace Lacey.Medusa.Youtube.Api.Services.Common
{
    public abstract class YoutubeApiService
    {
        internal YouTubeService Youtube { get; }

        protected readonly IMapper Mapper;

        protected YoutubeApiService(
            IYoutubeAuthProvider youtubeAuthProvider, 
            IMapper mapper)
        {
            this.Mapper = mapper;


            this.Youtube = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = youtubeAuthProvider.GetApiKey(),
                HttpClientInitializer = youtubeAuthProvider.GetUserCredentials().Result,
                ApplicationName = GetType().ToString()
            });
        }
    }
}