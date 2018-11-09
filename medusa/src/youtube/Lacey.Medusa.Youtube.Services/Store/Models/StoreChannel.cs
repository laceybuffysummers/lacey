﻿using System.Collections.Generic;

namespace Lacey.Medusa.Youtube.Services.Store.Models
{
    public sealed class StoreChannel
    {
        public StoreChannel(
            StoreChannelInfo channel, 
            IEnumerable<StoreVideo> videos)
        {
            Channel = channel;
            Videos = videos;
        }

        public StoreChannelInfo Channel { get; }

        public IEnumerable<StoreVideo> Videos { get; set; }

        public IEnumerable<StoreVideo> InvalidVideos { get; set; }
    }
}