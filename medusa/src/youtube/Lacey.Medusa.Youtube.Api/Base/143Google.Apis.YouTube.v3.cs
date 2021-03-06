﻿// Decompiled with JetBrains decompiler
// Type: Google.Apis.YouTube.v3.Data.VideoAbuseReportReasonSnippet
// Assembly: Google.Apis.YouTube.v3, Version=1.36.1.1226, Culture=neutral, PublicKeyToken=4b01fa6e34db77ab
// MVID: E56916E5-79D6-4645-883A-B3D57DB2C10C
// Assembly location: C:\Users\Lacey\AppData\Local\Temp\Jylyxot\d3b8721247\lib\netstandard2.0\Google.Apis.YouTube.v3.dll

using System.Collections.Generic;
using Lacey.Medusa.Common.Api.Base.Requests;
using Newtonsoft.Json;

namespace Lacey.Medusa.Youtube.Api.Base
{
  /// <summary>Basic details about a video category, such as its localized title.</summary>
  public class VideoAbuseReportReasonSnippet : IDirectResponseSchema
  {
    /// <summary>The localized label belonging to this abuse report reason.</summary>
    [JsonProperty("label")]
    public virtual string Label { get; set; }

    /// <summary>The secondary reasons associated with this reason, if any are available. (There might be 0 or
    /// more.)</summary>
    [JsonProperty("secondaryReasons")]
    public virtual IList<VideoAbuseReportSecondaryReason> SecondaryReasons { get; set; }

    /// <summary>The ETag of the item.</summary>
    public virtual string ETag { get; set; }
  }
}
