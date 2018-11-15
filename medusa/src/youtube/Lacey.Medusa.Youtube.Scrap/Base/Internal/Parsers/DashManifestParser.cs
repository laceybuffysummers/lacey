﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Lacey.Medusa.Youtube.Scrap.Base.Internal.Parsers
{
    public partial class DashManifestParser
    {
        private readonly XElement _root;

        public DashManifestParser(XElement root)
        {
            _root = root;
        }

        public IEnumerable<StreamInfoParser> GetStreamInfos()
        {
            var streamInfosXml = _root.Descendants("Representation");

            // Filter out partial streams
            streamInfosXml = streamInfosXml.Where(s =>
                s.Descendants("Initialization").FirstOrDefault()?.Attribute("sourceURL")?.Value.Contains("sq/") != true);

            return streamInfosXml.Select(x => new StreamInfoParser(x));
        }
    }

    public partial class DashManifestParser
    {
        public class StreamInfoParser
        {
            private readonly XElement _root;

            public StreamInfoParser(XElement root)
            {
                _root = root;
            }

            public int ParseItag() => (int) _root.Attribute("id");

            public string ParseUrl() => (string) _root.Element("BaseURL");

            public long ParseContentLength() => Regex.Match(ParseUrl(), @"clen[/=](\d+)").Groups[1].Value.ParseLong();

            public long ParseBitrate() => (long) _root.Attribute("bandwidth");

            public bool ParseIsAudioOnly() => _root.Element("AudioChannelConfiguration") != null;

            public int ParseWidth() => (int) _root.Attribute("width");

            public int ParseHeight() => (int) _root.Attribute("height");

            public int ParseFramerate() => (int) _root.Attribute("frameRate");
        }
    }

    public partial class DashManifestParser
    {
        public static DashManifestParser Initialize(string raw)
        {
            var root = XElement.Parse(raw).StripNamespaces();
            return new DashManifestParser(root);
        }
    }
}