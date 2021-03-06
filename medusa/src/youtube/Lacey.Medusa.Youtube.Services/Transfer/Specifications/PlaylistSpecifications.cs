﻿using System.Linq;
using Lacey.Medusa.Youtube.Domain.Entities;

namespace Lacey.Medusa.Youtube.Services.Transfer.Specifications
{
    internal static class PlaylistSpecifications
    {
        internal static IQueryable<PlaylistEntity> GetByTransfer(
            this IQueryable<PlaylistEntity> query,
            string originalChannelId,
            string channelId)
        {
            if (string.IsNullOrEmpty(originalChannelId) ||
                string.IsNullOrEmpty(channelId))
            {
                return query;
            }

            query = query.Where(e => e.Channel.OriginalChannelId == originalChannelId &&
                                     e.Channel.ChannelId == channelId);

            return query;
        }

        internal static IQueryable<PlaylistEntity> GetByChannelId(
            this IQueryable<PlaylistEntity> query,
            string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                return query;
            }

            query = query.Where(e => e.Channel.ChannelId == channelId);

            return query;
        }

        internal static IQueryable<PlaylistEntity> GetByPlaylistId(
            this IQueryable<PlaylistEntity> query,
            string playlistId)
        {
            if (string.IsNullOrEmpty(playlistId))
            {
                return query;
            }

            query = query.Where(e => e.PlaylistId == playlistId);

            return query;
        }

        internal static IQueryable<PlaylistVideoEntity> GetVideosByPlaylistId(
            this IQueryable<PlaylistVideoEntity> query,
            int playlistId)
        {
            query = query.Where(e => e.PlaylistId == playlistId);

            return query;
        }
    }
}