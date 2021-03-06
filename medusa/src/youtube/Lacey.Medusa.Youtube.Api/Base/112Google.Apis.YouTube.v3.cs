﻿// Decompiled with JetBrains decompiler
// Type: Google.Apis.YouTube.v3.Data.PlaylistContentDetails
// Assembly: Google.Apis.YouTube.v3, Version=1.36.1.1226, Culture=neutral, PublicKeyToken=4b01fa6e34db77ab
// MVID: E56916E5-79D6-4645-883A-B3D57DB2C10C
// Assembly location: C:\Users\Lacey\AppData\Local\Temp\Jylyxot\d3b8721247\lib\netstandard2.0\Google.Apis.YouTube.v3.dll

using Lacey.Medusa.Common.Api.Base.Requests;
using Newtonsoft.Json;

namespace Lacey.Medusa.Youtube.Api.Base
{
  public class PlaylistContentDetails : IDirectResponseSchema
  {
    /// <summary>The number of videos in the playlist.</summary>
    [JsonProperty("itemCount")]
    public virtual long? ItemCount { get; set; }

    /// <summary>The ETag of the item.</summary>
    public virtual string ETag { get; set; }
  }
}
