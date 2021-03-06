﻿using System.Linq;
using System.Threading.Tasks;
using Lacey.Medusa.Youtube.Domain.Entities;
using Lacey.Medusa.Youtube.Services.Transfer.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Lacey.Medusa.Youtube.Services.Transfer.Mappers
{
    internal static class ChannelsMapper
    {
        internal static async Task<ChannelEntity> MapToTransferMetadata(
            IQueryable<ChannelEntity> channels,
            string originalChannelId,
            string channelId)
        {
            var query = from c in channels
                    .GetByOriginalChannelId(originalChannelId)
                    .GetByChannelId(channelId)
                select c;

            return await query.FirstOrDefaultAsync();
        }

        internal static async Task<ChannelEntity> MapToChannelMetadata(
            IQueryable<ChannelEntity> channels,
            string channelId)
        {
            var query = from c in channels.GetByChannelId(channelId)
                select c;

            return await query.FirstOrDefaultAsync();
        }
    }
}