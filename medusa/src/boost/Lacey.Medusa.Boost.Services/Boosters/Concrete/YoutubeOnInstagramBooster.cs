﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstagramApiSharp.Classes.Models;
using Lacey.Medusa.Boost.Services.Extensions;
using Lacey.Medusa.Boost.Services.Providers;
using Lacey.Medusa.Common.Generators.Generators;
using Lacey.Medusa.Instagram.Domain.Entities;
using Lacey.Medusa.Youtube.Api.Base;
using Microsoft.Extensions.Logging;

namespace Lacey.Medusa.Boost.Services.Boosters.Concrete
{
    public sealed class YoutubeOnInstagramBooster
    {
        private readonly IInstagramBoostProvider instagramProvider;

        private readonly ILogger logger;

        private readonly IReadOnlyList<string> names;

        public YoutubeOnInstagramBooster(
            IInstagramBoostProvider instagramProvider, 
            ILogger<YoutubeOnInstagramBooster> logger, 
            INamesGenerator generator)
        {
            this.instagramProvider = instagramProvider;
            this.logger = logger;

            this.names = generator.GenerateFirstNames();

            this.instagramProvider.Login().Wait();
        }

        public async Task<bool> Boost(
            ChannelEntity channel,
            Youtube.Domain.Entities.ChannelEntity youtubeChannel,
            Video video)
        {
            var query = this.names.GetRandomName();

            var usersCount = 10;
            var users = new List<InstaUser>();
            while (usersCount <= 100)
            {
                var result = await this.instagramProvider.SearchPeopleAsync(query, usersCount);
                if (!result.Succeeded)
                {
                    this.logger.LogError($"{result.Info?.Message}");
                    return false;
                }

                users = result.Value.Users
                    .Where(u => 
                        !u.IsPrivate &&
                        u.FollowersCount < 1000)
                    .ToList();

                if (users.Any())
                {
                    break;
                }

                usersCount += 10;
            }

            if (!users.Any()) 
            {
                return false;
            }

            foreach (var user in users.Shuffle())
            {
                if (user.UserName == channel.OriginalChannelId ||
                    user.UserName == channel.ChannelId ||
                    user.UserName == youtubeChannel.Name)
                {
                    continue;
                }

                var mediaList = await this.instagramProvider.GetLastMedia(user.UserName);
                if (mediaList == null || !mediaList.Any())
                {
                    continue;
                }

                foreach (var media in mediaList)
                {
                    if (media?.Caption == null ||
                        string.IsNullOrEmpty(media.Caption.MediaId) ||
                        media.IsCommentsDisabled)
                    {
                        continue;
                    }

                    if (!int.TryParse(media.CommentsCount, out var commentsCount))
                    {
                        continue;
                    }

                    if (commentsCount > 3)
                    {
                        continue;
                    }

                    var res = await this.instagramProvider.CommentMediaAsync(
                        media.Caption.MediaId,
                        video.GetInstagramBoostText());
                    if (!res.Succeeded)
                    {
                        this.logger.LogError($"{res.Info?.Message}");
                        continue;
                    }

                    this.logger.LogTrace($"[{media.GetInstagramUrl()}] [{user.UserName}] [{user.FullName}]");
                    return true;
                }
            }

            return false;
        }
    }
}