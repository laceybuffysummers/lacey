﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lacey.Medusa.Youtube.Api.Base;
using Lacey.Medusa.Youtube.Domain.Entities;

namespace Lacey.Medusa.Youtube.Services.Transfer.Services
{
    public interface IPlaylistsService
    {
        Task<IReadOnlyList<PlaylistEntity>> GetTransferPlaylists(string originalChannelId, string channelId);

        Task<IReadOnlyList<PlaylistEntity>> GetChannelPlaylists(string channelId);

        Task<int> Add(string originalChannelId, string channelId, Playlist playlist);

        Task DeleteTransferPlaylists(string originalChannelId, string channelId);

        Task DeleteChannelPlaylists(string channelId);

        Task DeletePlaylist(string playlistId);
    }
}