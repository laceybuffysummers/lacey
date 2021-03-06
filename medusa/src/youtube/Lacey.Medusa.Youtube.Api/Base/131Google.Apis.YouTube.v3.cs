﻿// Decompiled with JetBrains decompiler
// Type: Google.Apis.YouTube.v3.Data.SubscriptionContentDetails
// Assembly: Google.Apis.YouTube.v3, Version=1.36.1.1226, Culture=neutral, PublicKeyToken=4b01fa6e34db77ab
// MVID: E56916E5-79D6-4645-883A-B3D57DB2C10C
// Assembly location: C:\Users\Lacey\AppData\Local\Temp\Jylyxot\d3b8721247\lib\netstandard2.0\Google.Apis.YouTube.v3.dll

using Lacey.Medusa.Common.Api.Base.Requests;
using Newtonsoft.Json;

namespace Lacey.Medusa.Youtube.Api.Base
{
  /// <summary>Details about the content to witch a subscription refers.</summary>
  public class SubscriptionContentDetails : IDirectResponseSchema
  {
    /// <summary>The type of activity this subscription is for (only uploads, everything).</summary>
    [JsonProperty("activityType")]
    public virtual string ActivityType { get; set; }

    /// <summary>The number of new items in the subscription since its content was last read.</summary>
    [JsonProperty("newItemCount")]
    public virtual long? NewItemCount { get; set; }

    /// <summary>The approximate number of items that the subscription points to.</summary>
    [JsonProperty("totalItemCount")]
    public virtual long? TotalItemCount { get; set; }

    /// <summary>The ETag of the item.</summary>
    public virtual string ETag { get; set; }
  }
}
