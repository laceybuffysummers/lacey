﻿// Decompiled with JetBrains decompiler
// Type: Google.Apis.YouTube.v3.Data.I18nLanguageListResponse
// Assembly: Google.Apis.YouTube.v3, Version=1.36.1.1226, Culture=neutral, PublicKeyToken=4b01fa6e34db77ab
// MVID: E56916E5-79D6-4645-883A-B3D57DB2C10C
// Assembly location: C:\Users\Lacey\AppData\Local\Temp\Jylyxot\d3b8721247\lib\netstandard2.0\Google.Apis.YouTube.v3.dll

using System.Collections.Generic;
using Lacey.Medusa.Common.Api.Base.Requests;
using Newtonsoft.Json;

namespace Lacey.Medusa.Youtube.Api.Base
{
  public class I18nLanguageListResponse : IDirectResponseSchema
  {
    /// <summary>Etag of this resource.</summary>
    [JsonProperty("etag")]
    public virtual string ETag { get; set; }

    /// <summary>Serialized EventId of the request which produced this response.</summary>
    [JsonProperty("eventId")]
    public virtual string EventId { get; set; }

    /// <summary>A list of supported i18n languages. In this map, the i18n language ID is the map key, and its value
    /// is the corresponding i18nLanguage resource.</summary>
    [JsonProperty("items")]
    public virtual IList<I18nLanguage> Items { get; set; }

    /// <summary>Identifies what kind of resource this is. Value: the fixed string
    /// "youtube#i18nLanguageListResponse".</summary>
    [JsonProperty("kind")]
    public virtual string Kind { get; set; }

    /// <summary>The visitorId identifies the visitor.</summary>
    [JsonProperty("visitorId")]
    public virtual string VisitorId { get; set; }
  }
}
