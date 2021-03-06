﻿using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;

namespace Lacey.Medusa.Youtube.Scrap.Base.Internal.Parsers
{
    public partial class UserPageParser
    {
        private readonly IHtmlDocument _root;

        public UserPageParser(IHtmlDocument root)
        {
            _root = root;
        }

        public string ParseChannelId() => _root.QuerySelector("link[rel=\"canonical\"]").GetAttribute("href")
            .SubstringAfter("channel/");
    }

    public partial class UserPageParser
    {
        public static UserPageParser Initialize(string raw)
        {
            var root = new HtmlParser().Parse(raw);
            return new UserPageParser(root);
        }
    }
}