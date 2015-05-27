﻿// -----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Synchronization.ClientServices
{
    /// <summary>
    /// Custom exception for any CacheController related invalid operations.
    /// </summary>
    public class CacheControllerException : Exception
    {
        /// <summary>
        /// CacheControllerException ctor
        /// </summary>
        /// <param name="message"></param>
        public CacheControllerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// CacheControllerException ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public CacheControllerException(string message, Exception inner)
            : base(message, inner)
        { }

        /// <summary>
        /// CacheControllerException ctor
        /// </summary>
        public CacheControllerException()
            : base()
        { }
    }
}
