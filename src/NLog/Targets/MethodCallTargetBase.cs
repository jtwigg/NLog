// 
// Copyright (c) 2004,2005 Jaroslaw Kowalski <jkowalski@users.sourceforge.net>
// 
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of the Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;

using NLog.Config;

namespace NLog.Targets
{
    /// <summary>
    /// The base class for all targets which call methods (local or remote). 
    /// Manages parameters and type coercion.
    /// </summary>
    public abstract class MethodCallTargetBase: Target
    {
        private MethodCallParameterCollection _parameters = new MethodCallParameterCollection();

        /// <summary>
        /// Prepares an array of parameters to be passed based on the logging event and calls DoInvoke()
        /// </summary>
        /// <param name="ev">The logging event.</param>
        protected internal override void Append(LogEventInfo ev)
        {
            object[]parameters = new object[Parameters.Count];
            for (int i = 0; i < parameters.Length; ++i)
            {
                parameters[i] = Parameters[i].GetValue(ev);
            }

            DoInvoke(parameters);
        }

        /// <summary>
        /// Calls the target method. Must be implemented in concrete classes.
        /// </summary>
        /// <param name="parameters">Method call parameters</param>
        protected abstract void DoInvoke(object[]parameters);

        /// <summary>
        /// Determines whether stack trace information should be gathered
        /// during log event processing. It calls <see cref="NLog.Layout.NeedsStackTrace" /> on
        /// all parameters.
        /// </summary>
        /// <returns>0 - don't include stack trace<br/>1 - include stack trace without source file information<br/>2 - include full stack trace</returns>
        protected internal override int NeedsStackTrace()
        {
            int max = base.NeedsStackTrace();
            for (int i = 0; i < Parameters.Count; ++i)
            {
                max = Math.Max(max, Parameters[i].NeedsStackTrace());
                if (max == 2)
                    break;
            }

            return max;
        }

        /// <summary>
        /// Array of parameters to be passed.
        /// </summary>
        [ArrayParameter(typeof(MethodCallParameter), "parameter")]
        public MethodCallParameterCollection Parameters
        {
            get
            {
                return _parameters;
            }
        }
    }
}
