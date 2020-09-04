// Copyright 2014 Serilog Contributors
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
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Json;

using System.Net.Http.Headers;
using Serilog.Core;
using Serilog.Formatting.Compact;

namespace Serilog.Sinks.CouchDB
{
    /// <summary>
    /// Writes log events as documents to a CouchDB database.
    /// </summary>
    public class CouchDBSink : ILogEventSink
    {
        readonly IFormatProvider _formatProvider;
        readonly HttpClient _httpClient;
        const string BulkUploadResource = "_bulk_docs";

        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 50;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Construct a sink posting to the specified database.
        /// </summary>
        /// <param name="databaseUrl">The URL of a CouchDB database.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="databaseUsername">The username to use in the HTTP Authentication header.</param>
        /// <param name="databasePassword">Password to use in the HTTP Authentication header</param>
        public CouchDBSink(string databaseUrl, int batchPostingLimit, TimeSpan period, IFormatProvider formatProvider, string databaseUsername, string databasePassword)
           
        {
            if (databaseUrl == null) throw new ArgumentNullException("databaseUrl");
            var baseAddress = databaseUrl;
            if (!databaseUrl.EndsWith("/"))
                baseAddress += "/";

            _formatProvider = formatProvider;
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };

          if (databaseUsername != null & databasePassword != null)
          {
            var authByteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", databaseUsername, databasePassword));
            var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authByteArray));
            _httpClient.DefaultRequestHeaders.Authorization = authHeader;
          }
        }


        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="logEvent">The events to emit.</param>
        /// <remarks>Override either <see cref="ILogEventSink.Emit"/>,
        /// not both.</remarks>



       
        public  async void Emit(LogEvent logEvent)
        {
          
            var payload2 = new StringWriter();
        
            payload2.Write("{\"docs\":[");
            JsonValueFormatter jsonValueFormatter = new JsonValueFormatter();
            CoachDBJsonFormatter formatter2 = new CoachDBJsonFormatter(jsonValueFormatter);


            var payload = new StringWriter();
            payload.Write("{\"docs\":[");



            JsonFormatter formatter = new JsonFormatter(
              omitEnclosingObject: true,
               formatProvider: _formatProvider,
                renderMessage: true);

            var delimStart = "{";

            payload.Write(delimStart);


            formatter.Format(logEvent, payload);
            payload.Write(
                ",\"UtcTimestamp\":\"{0:u}\"}}",
                logEvent.Timestamp.ToUniversalTime().DateTime);
            delimStart = ",{";

            payload.Write("]}");

            //   payload.Write(delimStart);


            /*   
                formatter.Format(logEvent, payload);
                    payload.Write(
                        ",\"UtcTimestamp\":\"{0:u}\"}",
                        logEvent.Timestamp.ToUniversalTime().DateTime);
                    delimStart = ",{";
              */
            /*
          //{{"docs":[{"Timestamp":"2020-09-02T17:04:25.4863907+02:00","Level":"Error","MessageTemplate":"Hello {World} {array}!","RenderedMessage":"Hello \"World\" \"0102\"!","Properties":{"World":"World","array":"0102"},"UtcTimestamp":"2020-09-02 15:04:25Z"}}

  */


            formatter2.Format(logEvent, payload2);
            payload2.Write("]}");
            

            var content = new StringContent(payload2.ToString(), Encoding.UTF8, "application/json");
            var result =  await _httpClient.PostAsync(BulkUploadResource, content);
            if (!result.IsSuccessStatusCode)
                throw new LoggingFailedException(string.Format("Received failed result {0} when posting events to CouchDB", result.StatusCode));
        }
    }
}
