﻿namespace Lacey.Medusa.MakeMoney.Run.Configuration
{
    internal sealed class AppConfiguration
    {
        public MmConfiguration Mm { get; set; }

        public string TempFolder { get; set; }

        public EmailConfiguration Email { get; set; }

        public LogsConfiguration Logs { get; set; }
    }
}