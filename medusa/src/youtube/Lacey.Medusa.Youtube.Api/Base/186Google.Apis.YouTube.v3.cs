﻿// Decompiled with JetBrains decompiler
// Type: Google.Apis.YouTube.v3.Data.LiveChatPollEditedDetails
// Assembly: Google.Apis.YouTube.v3, Version=1.36.1.1226, Culture=neutral, PublicKeyToken=4b01fa6e34db77ab
// MVID: E56916E5-79D6-4645-883A-B3D57DB2C10C
// Assembly location: C:\Users\Lacey\AppData\Local\Temp\Jylyxot\d3b8721247\lib\netstandard2.0\Google.Apis.YouTube.v3.dll

using System.Collections.Generic;
using Lacey.Medusa.Common.Api.Base.Requests;
using Newtonsoft.Json;

namespace Lacey.Medusa.Youtube.Api.Base
{
  public class LiveChatPollEditedDetails : IDirectResponseSchema
  {
    [JsonProperty("id")]
    public virtual string Id { get; set; }

    [JsonProperty("items")]
    public virtual IList<LiveChatPollItem> Items { get; set; }

    [JsonProperty("prompt")]
    public virtual string Prompt { get; set; }

    /// <summary>The ETag of the item.</summary>
    public virtual string ETag { get; set; }
  }
}
