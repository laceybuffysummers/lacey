﻿// Decompiled with JetBrains decompiler
// Type: Google.Apis.YouTube.v3.Data.WatchSettings
// Assembly: Google.Apis.YouTube.v3, Version=1.36.1.1226, Culture=neutral, PublicKeyToken=4b01fa6e34db77ab
// MVID: E56916E5-79D6-4645-883A-B3D57DB2C10C
// Assembly location: C:\Users\Lacey\AppData\Local\Temp\Jylyxot\d3b8721247\lib\netstandard2.0\Google.Apis.YouTube.v3.dll

using Lacey.Medusa.Common.Api.Base.Requests;
using Newtonsoft.Json;

namespace Lacey.Medusa.Youtube.Api.Base
{
  /// <summary>Branding properties for the watch. All deprecated.</summary>
  public class WatchSettings : IDirectResponseSchema
  {
    /// <summary>The text color for the video watch page's branded area.</summary>
    [JsonProperty("backgroundColor")]
    public virtual string BackgroundColor { get; set; }

    /// <summary>An ID that uniquely identifies a playlist that displays next to the video player.</summary>
    [JsonProperty("featuredPlaylistId")]
    public virtual string FeaturedPlaylistId { get; set; }

    /// <summary>The background color for the video watch page's branded area.</summary>
    [JsonProperty("textColor")]
    public virtual string TextColor { get; set; }

    /// <summary>The ETag of the item.</summary>
    public virtual string ETag { get; set; }
  }
}
