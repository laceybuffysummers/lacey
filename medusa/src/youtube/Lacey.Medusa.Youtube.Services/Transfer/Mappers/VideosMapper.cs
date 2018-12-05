﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lacey.Medusa.Youtube.Domain.Entities;
using Lacey.Medusa.Youtube.Services.Transfer.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Lacey.Medusa.Youtube.Services.Transfer.Mappers
{
    internal static class VideosMapper
    {
        internal static async Task<IEnumerable<VideoEntity>> MapToVideos(
            IQueryable<ChannelEntity> channels,
            IQueryable<VideoEntity> videos,
            string originalChannelId,
            string channelId)
        {
            var query = from v in videos.GetByTransfer(originalChannelId, channelId)
                        join c in channels on v.ChannelId equals c.Id
                        select v;

            return await query.ToArrayAsync();
        }
    }
}