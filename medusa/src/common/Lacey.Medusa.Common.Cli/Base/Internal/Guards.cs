﻿using System;

namespace Lacey.Medusa.Common.Cli.Base.Internal
{
    internal static class Guards
    {
        [ContractAnnotation("o:null => halt")]
        public static T GuardNotNull<T>([NoEnumeration] this T o, string argName = null) where T : class
        {
            return o ?? throw new ArgumentNullException(argName);
        }
    }
}