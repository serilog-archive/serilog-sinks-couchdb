﻿// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.CouchDB;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.CouchDB() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationCouchDBExtensions
    {
      /// <summary>
      /// Adds a sink that writes log events as documents to a CouchDB database.
      /// </summary>
      /// <param name="loggerConfiguration">The logger configuration.</param>
      /// <param name="databaseUrl">The URL of a created CouchDB database that log events will be written to.</param>
      /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
      /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
      /// <param name="period">The time to wait between checking for event batches.</param>
      /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
      /// <param name="databaseUsername">The username to authenticate with the database (if needed).</param>
      /// <param name="databasePassword">The password to use to authenticate (if needed)</param>
      /// <returns>Logger configuration, allowing configuration to continue.</returns>
      /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
      public static LoggerConfiguration CouchDB(
            this LoggerSinkConfiguration loggerConfiguration,
            string databaseUrl,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = CouchDBSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            string databaseUsername = null,
            string databasePassword = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (databaseUrl == null) throw new ArgumentNullException("databaseUrl");
            if (databaseUsername != null && databasePassword == null) throw new ArgumentNullException("databasePassword");
            if (databaseUsername == null && databasePassword != null) throw new ArgumentNullException("databaseUsername");

            var defaultedPeriod = period ?? CouchDBSink.DefaultPeriod;
            var sink = new CouchDBSink(databaseUrl, batchPostingLimit, defaultedPeriod, formatProvider, databaseUsername, databasePassword);
            return loggerConfiguration.Sink(sink,restrictedToMinimumLevel);
        }
    }
}
