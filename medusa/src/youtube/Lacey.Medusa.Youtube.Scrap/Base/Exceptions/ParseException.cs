﻿using System;

namespace Lacey.Medusa.Youtube.Scrap.Base.Exceptions
{
    /// <summary>
    /// Thrown when there was an error parsing some data.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary />
        public ParseException(string message)
            : base(message)
        {
        }
    }
}