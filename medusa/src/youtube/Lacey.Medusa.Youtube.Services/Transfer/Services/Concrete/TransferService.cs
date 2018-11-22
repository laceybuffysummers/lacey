﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lacey.Medusa.Youtube.Api.Base;
using Lacey.Medusa.Youtube.Api.Services;
using Lacey.Medusa.Youtube.Services.Common.Services;
using Microsoft.Extensions.Logging;

namespace Lacey.Medusa.Youtube.Services.Transfer.Services.Concrete
{
    public sealed class TransferService : YoutubeService, ITransferService
    {
        private readonly string outputFolder;

        public TransferService(
            IYoutubeProvider youtubeProvider, 
            ILogger<TransferService> logger, 
            string outputFolder) : 
            base(youtubeProvider, logger)
        {
            this.outputFolder = outputFolder;
        }

        public async Task TransferChannel(string sourceChannelId, string destChannelId)
        {
            var videosResult = await this.TransferVideos(sourceChannelId, destChannelId);

            await this.TransferPlaylists(sourceChannelId, destChannelId, videosResult.Links);

            await this.TransferSubscriptions(sourceChannelId, destChannelId);

            await this.TransferComments(sourceChannelId, destChannelId);

            await this.TransferMetadata(sourceChannelId, destChannelId);
        }

        #region videos

        private async Task<TransferVideosResult> TransferVideos(string sourceChannelId, string destChannelId)
        {
            var source = await this.YoutubeProvider.GetVideos(sourceChannelId);
            var dest = await this.YoutubeProvider.GetVideos(destChannelId);

            var videos = new List<Video>();
            var links = new List<VideoLink>();
            foreach (var item in source
                .OrderBy(v => v.Snippet.PublishedAt))
            {
                // skip existing items
                if (dest.Any(d =>
                    item.Snippet.Title == d.Snippet.Title &&
                    item.ContentDetails.Duration == d.ContentDetails.Duration))
                {
                    this.Logger.LogTrace($"Video [{item.Snippet.Title}] skipped. Video already exists.");
                    continue;
                }

                this.Logger.LogTrace($"Downloading video [{item.Id}]...");
                var filePath = await this.YoutubeProvider.DownloadVideo(item.Id, this.outputFolder);
                this.Logger.LogTrace($"Video [{item.Id}] downloaded to [{filePath}]");
                try
                {
                    var video = await this.YoutubeProvider.UploadVideo(
                        destChannelId,
                        item,
                        filePath);

                    videos.Add(video);
                    links.Add(new VideoLink(item.Id, video.Id));
                }
                catch (Exception exc)
                {
                    this.Logger.LogError(exc.Message);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            return new TransferVideosResult(videos, links);
        }

        #endregion

        #region playlists

        private async Task<IList<Playlist>> TransferPlaylists(
            string sourceChannelId, 
            string destChannelId,
            IList<VideoLink> links)
        {
            IList<Playlist> list = new List<Playlist>();
            try
            {
                var source = await this.YoutubeProvider.GetPlaylists(sourceChannelId);
                var dest = await this.YoutubeProvider.GetPlaylists(destChannelId);

                // skip existing items
                foreach (var playlist in source
                    .OrderBy(s => s.Snippet.PublishedAt))
                {
                    if (dest.Any(d =>
                        playlist.Snippet.Title == d.Snippet.Title &&
                        playlist.Snippet.Description == d.Snippet.Description))
                    {
                        continue;
                    }

                    var uploaded = await this.YoutubeProvider.UploadPlaylist(destChannelId, playlist);

                    // insert playlist items
                    var playlistItems = await this.YoutubeProvider.GetPlaylistItems(playlist.Id);
                    foreach (var item in playlistItems)
                    {
                        var link = links.FirstOrDefault(
                            l => l.SourceVideoId == item.Snippet.ResourceId.VideoId);
                        item.Snippet.PlaylistId = uploaded.Id;
                        if (link != null)
                        {
                            item.Snippet.ResourceId.VideoId = link.DestVideoId;
                        }
                        await this.YoutubeProvider.UploadPlaylistItem(destChannelId, item);
                    }

                    list.Add(uploaded);
                }
            }
            catch (Exception exc)
            {
                this.Logger.LogError(exc.Message);
            }

            return list;
        }

        #endregion

        #region subscriptions

        private async Task<IList<Subscription>> TransferSubscriptions(string sourceChannelId, string destChannelId)
        {
            IList<Subscription> uploaded = new List<Subscription>();
            try
            {
                var source = await this.YoutubeProvider.GetSubscriptions(sourceChannelId);
                var dest = await this.YoutubeProvider.GetSubscriptions(destChannelId);

                // skip existing items
                var toUpload = new List<Subscription>();
                foreach (var item in source)
                {
                    if (dest.Any(d =>
                        item.Snippet.ResourceId.ChannelId == d.Snippet.ResourceId.ChannelId))
                    {
                        continue;
                    }

                    toUpload.Add(item);
                }

                uploaded = await this.YoutubeProvider.UploadSubscriptions(
                    destChannelId,
                    toUpload
                        .OrderBy(c => c.Snippet.PublishedAt).ToList());
            }
            catch (Exception exc)
            {
                this.Logger.LogError(exc.Message);
            }

            return uploaded;
        }

        #endregion

        #region comments

        private async Task<IList<CommentThread>> TransferComments(string sourceChannelId, string destChannelId)
        {
            IList<CommentThread> uploaded = new List<CommentThread>();
            try
            {
                var source = await this.YoutubeProvider.GetComments(sourceChannelId);
                var dest = await this.YoutubeProvider.GetComments(destChannelId);

                var toUpload = new List<CommentThread>();
                foreach (var item in source)
                {
                    // skip existing items
                    if (dest.Any(d =>
                        item.Snippet.TopLevelComment.Snippet.TextDisplay == d.Snippet.TopLevelComment.Snippet.TextDisplay))
                    {
                        continue;
                    }

                    toUpload.Add(item);
                }

                uploaded = await this.YoutubeProvider.UploadComments(
                    destChannelId,
                    toUpload
                        .OrderBy(c => c.Snippet.TopLevelComment.Snippet.PublishedAt).ToList());
            }
            catch (Exception exc)
            {
                this.Logger.LogError(exc.Message);
            }

            return uploaded;
        }

        #endregion

        #region metadata

        private async Task<Channel> TransferMetadata(string sourceChannelId, string destChannelId)
        {
            Channel uploaded = null;
            try
            {
                var source = await this.YoutubeProvider.GetChannel(sourceChannelId);

                var iconFilePath = await this.YoutubeProvider.DownloadIcon(source, this.outputFolder);
                this.Logger.LogTrace($"Icon downloaded to [{iconFilePath}].");

                uploaded = await this.YoutubeProvider.UpdateMetadata(destChannelId, source);
            }
            catch (Exception exc)
            {
                this.Logger.LogError(exc.Message);
            }

            return uploaded;
        }

        #endregion

        #region private classes

        private sealed class VideoLink
        {
            public VideoLink(
                string sourceVideoId,
                string destVideoId)
            {
                SourceVideoId = sourceVideoId;
                DestVideoId = destVideoId;
            }

            public string SourceVideoId { get; private set; }

            public string DestVideoId { get; private set; }
        }

        private sealed class TransferVideosResult
        {
            public TransferVideosResult(
                IList<Video> videos,
                IList<VideoLink> links)
            {
                this.Videos = videos;
                this.Links = links;
            }

            public IList<Video> Videos { get; private set; }

            public IList<VideoLink> Links { get; private set; }
        }

        #endregion
    }
}