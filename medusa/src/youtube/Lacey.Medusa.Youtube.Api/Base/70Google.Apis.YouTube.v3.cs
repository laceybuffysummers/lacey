﻿// Decompiled with JetBrains decompiler
// Type: Google.Apis.YouTube.v3.Data.CommentListResponse
// Assembly: Google.Apis.YouTube.v3, Version=1.36.1.1226, Culture=neutral, PublicKeyToken=4b01fa6e34db77ab
// MVID: E56916E5-79D6-4645-883A-B3D57DB2C10C
// Assembly location: C:\Users\Lacey\AppData\Local\Temp\Jylyxot\d3b8721247\lib\netstandard2.0\Google.Apis.YouTube.v3.dll

using System.Collections.Generic;
using Lacey.Medusa.Common.Api.Base.Requests;
using Newtonsoft.Json;

namespace Lacey.Medusa.Youtube.Api.Base
{
  public class CommentListResponse : IDirectResponseSchema
  {
    /// <summary>Etag of this resource.</summary>
    [JsonProperty("etag")]
    public virtual string ETag { get; set; }

    /// <summary>Serialized EventId of the request which produced this response.</summary>
    [JsonProperty("eventId")]
    public virtual string EventId { get; set; }

    /// <summary>A list of comments that match the request criteria.</summary>
    [JsonProperty("items")]
    public virtual IList<Comment> Items { get; set; }

    /// <summary>Identifies what kind of resource this is. Value: the fixed string
    /// "youtube#commentListResponse".</summary>
    [JsonProperty("kind")]
    public virtual string Kind { get; set; }

    /// <summary>The token that can be used as the value of the pageToken parameter to retrieve the next page in the
    /// result set.</summary>
    [JsonProperty("nextPageToken")]
    public virtual string NextPageToken { get; set; }

    [JsonProperty("pageInfo")]
    public virtual PageInfo PageInfo { get; set; }

    [JsonProperty("tokenPagination")]
    public virtual TokenPagination TokenPagination { get; set; }

    /// <summary>The visitorId identifies the visitor.</summary>
    [JsonProperty("visitorId")]
    public virtual string VisitorId { get; set; }
  }
}
