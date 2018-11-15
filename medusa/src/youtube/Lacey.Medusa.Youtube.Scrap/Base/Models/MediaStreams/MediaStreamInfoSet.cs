﻿using System.Collections.Generic;
using Lacey.Medusa.Youtube.Scrap.Base.Internal;

namespace Lacey.Medusa.Youtube.Scrap.Base.Models.MediaStreams
{
    /// <summary>
    /// Set of all available media stream infos.
    /// </summary>
    public class MediaStreamInfoSet
    {
        /// <summary>
        /// Muxed streams.
        /// </summary>
        [NotNull, ItemNotNull]
        public IReadOnlyList<MuxedStreamInfo> Muxed { get; }

        /// <summary>
        /// Audio-only streams.
        /// </summary>
        [NotNull, ItemNotNull]
        public IReadOnlyList<AudioStreamInfo> Audio { get; }

        /// <summary>
        /// Video-only streams.
        /// </summary>
        [NotNull, ItemNotNull]
        public IReadOnlyList<VideoStreamInfo> Video { get; }

        /// <summary>
        /// Raw HTTP Live Streaming (HLS) URL to the m3u8 playlist.
        /// Null if not a live stream.
        /// </summary>
        [CanBeNull]
        public string HlsLiveStreamUrl { get; }

        /// <summary />
        public MediaStreamInfoSet(IReadOnlyList<MuxedStreamInfo> muxed,
            IReadOnlyList<AudioStreamInfo> audio,
            IReadOnlyList<VideoStreamInfo> video,
            string hlsLiveStreamUrl)
        {
            Muxed = muxed.GuardNotNull(nameof(muxed));
            Audio = audio.GuardNotNull(nameof(audio));
            Video = video.GuardNotNull(nameof(video));
            HlsLiveStreamUrl = hlsLiveStreamUrl;
        }
    }
}