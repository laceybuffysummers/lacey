﻿/*
Copyright 2013 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lacey.Medusa.Common.Api.Base.Requests;
using Lacey.Medusa.Common.Api.Core.Base;
using Lacey.Medusa.Common.Api.Core.Base.Discovery;
using Lacey.Medusa.Common.Api.Core.Base.Http;
using Lacey.Medusa.Common.Api.Core.Base.Json;
using Lacey.Medusa.Common.Api.Core.Base.Logging;
using Lacey.Medusa.Common.Api.Core.Base.Requests;
using Lacey.Medusa.Common.Api.Core.Base.Testing;
using Lacey.Medusa.Common.Api.Core.Base.Util;
using Newtonsoft.Json;

namespace Lacey.Medusa.Common.Api.Base.Services
{
    /// <summary>
    /// A base class for a client service which provides common mechanism for all services, like 
    /// serialization and GZip support.  It should be safe to use a single service instance to make server requests
    /// concurrently from multiple threads. 
    /// This class adds a special <see cref="IHttpExecuteInterceptor"/> to the 
    /// <see cref="ConfigurableMessageHandler"/> execute interceptor list, which uses the given 
    /// Authenticator. It calls to its applying authentication method, and injects the "Authorization" header in the 
    /// request.
    /// If the given Authenticator implements <see cref="IHttpUnsuccessfulResponseHandler"/>, this 
    /// class adds the Authenticator to the <see cref="ConfigurableMessageHandler"/>'s unsuccessful 
    /// response handler list.
    /// </summary>
    public abstract class BaseClientService : IClientService
    {
        /// <summary>The class logger.</summary>
        private static readonly ILogger Logger = ApplicationContext.Logger.ForType<BaseClientService>();

        /// <summary>The default maximum allowed length of a URL string for GET requests.</summary>
        [VisibleForTestOnly]
        public const uint DefaultMaxUrlLength = 2048;

        #region Initializer

        /// <summary>An initializer class for the client service.</summary>
        public class Initializer
        {
            /// <summary>
            /// Gets or sets the factory for creating <see cref="System.Net.Http.HttpClient"/> instance. If this 
            /// property is not set the service uses a new <see cref="Core.Base.Http.HttpClientFactory"/> instance.
            /// </summary>
            public IHttpClientFactory HttpClientFactory { get; set; }

            /// <summary>
            /// Gets or sets a HTTP client initializer which is able to customize properties on 
            /// <see cref="ConfigurableHttpClient"/> and 
            /// <see cref="ConfigurableMessageHandler"/>.
            /// </summary>
            public IConfigurableHttpClientInitializer HttpClientInitializer { get; set; }

            /// <summary>
            /// Get or sets the exponential back-off policy used by the service. Default value is 
            /// <c>UnsuccessfulResponse503</c>, which means that exponential back-off is used on 503 abnormal HTTP
            /// response.
            /// If the value is set to <c>None</c>, no exponential back-off policy is used, and it's up to the user to
            /// configure the <see cref="ConfigurableMessageHandler"/> in an
            /// <see cref="IConfigurableHttpClientInitializer"/> to set a specific back-off
            /// implementation (using <see cref="BackOffHandler"/>).
            /// </summary>
            public ExponentialBackOffPolicy DefaultExponentialBackOffPolicy { get; set; }

            /// <summary>Gets or sets whether this service supports GZip. Default value is <c>true</c>.</summary>
            public bool GZipEnabled { get; set; }

            /// <summary>
            /// Gets or sets the serializer. Default value is <see cref="NewtonsoftJsonSerializer"/>.
            /// </summary>
            public ISerializer Serializer { get; set; }

            /// <summary>Gets or sets the API Key. Default value is <c>null</c>.</summary>
            public string ApiKey { get; set; }

            /// <summary>
            /// Gets or sets Application name to be used in the User-Agent header. Default value is <c>null</c>. 
            /// </summary>
            public string ApplicationName { get; set; }

            /// <summary>
            /// Maximum allowed length of a URL string for GET requests. Default value is <c>2048</c>. If the value is
            /// set to <c>0</c>, requests will never be modified due to URL string length.
            /// </summary>
            public uint MaxUrlLength { get; set; }

            /// <summary>Constructs a new initializer with default values.</summary>
            public Initializer()
            {
                GZipEnabled = true;
                Serializer = new NewtonsoftJsonSerializer();
                DefaultExponentialBackOffPolicy = ExponentialBackOffPolicy.UnsuccessfulResponse503;
                MaxUrlLength = DefaultMaxUrlLength;
            }

            // HttpRequestMessage.Headers fails if any of these characters are included in a User-Agent header.
            private const string InvalidApplicationNameCharacters = "\"(),:;<=>?@[\\]{}";

            internal void Validate()
            {
                if (ApplicationName != null && ApplicationName.Any(c => InvalidApplicationNameCharacters.Contains(c)))
                {
                    throw new ArgumentException("Invalid Application name", nameof(ApplicationName));
                }
            }
        }

        #endregion

        /// <summary>Constructs a new base client with the specified initializer.</summary>
        protected BaseClientService(Initializer initializer)
        {
            initializer.Validate();
            // Set the right properties by the initializer's properties.
            GZipEnabled = initializer.GZipEnabled;
            Serializer = initializer.Serializer;
            ApiKey = initializer.ApiKey;
            ApplicationName = initializer.ApplicationName;
            if (ApplicationName == null)
            {
                Logger.Warning("Application name is not set. Please set Initializer.ApplicationName property");
            }
            HttpClientInitializer = initializer.HttpClientInitializer;

            // Create a HTTP client for this service.
            HttpClient = CreateHttpClient(initializer);
        }

        /// <summary>Returns <c>true</c> if this service contains the specified feature.</summary>
        private bool HasFeature(Features feature)
        {
            return Features.Contains(Utilities.GetEnumStringValue(feature));
        }

        private ConfigurableHttpClient CreateHttpClient(Initializer initializer)
        {
            // If factory wasn't set use the default HTTP client factory.
            var factory = initializer.HttpClientFactory ?? new HttpClientFactory();
            var args = new CreateHttpClientArgs
                {
                    GZipEnabled = GZipEnabled,
                    ApplicationName = ApplicationName,
                };

            // Add the user's input initializer.
            if (HttpClientInitializer != null)
            {
                args.Initializers.Add(HttpClientInitializer);
            }

            // Add exponential back-off initializer if necessary.
            if (initializer.DefaultExponentialBackOffPolicy != ExponentialBackOffPolicy.None)
            {
                args.Initializers.Add(new ExponentialBackOffInitializer(initializer.DefaultExponentialBackOffPolicy,
                    CreateBackOffHandler));
            }

            var httpClient = factory.CreateHttpClient(args);
            if (initializer.MaxUrlLength > 0)
            {
                httpClient.MessageHandler.AddExecuteInterceptor(new MaxUrlLengthInterceptor(initializer.MaxUrlLength));
            }
            return httpClient;
        }

        /// <summary>
        /// Creates the back-off handler with <see cref="ExponentialBackOff"/>. 
        /// Overrides this method to change the default behavior of back-off handler (e.g. you can change the maximum
        /// waited request's time span, or create a back-off handler with you own implementation of 
        /// <see cref="IBackOff"/>).
        /// </summary>
        protected virtual BackOffHandler CreateBackOffHandler()
        {
            // TODO(peleyal): consider return here interface and not the concrete class
            return new BackOffHandler(new ExponentialBackOff());
        }

        #region IClientService Members

        /// <inheritdoc/>
        public ConfigurableHttpClient HttpClient { get; private set; }

        /// <inheritdoc/>
        public IConfigurableHttpClientInitializer HttpClientInitializer { get; private set; }

        /// <inheritdoc/>
        public bool GZipEnabled { get; private set; }

        /// <inheritdoc/>
        public string ApiKey { get; private set; }

        /// <inheritdoc/>
        public string ApplicationName { get; private set; }

        /// <inheritdoc/>
        public void SetRequestSerailizedContent(HttpRequestMessage request, object body)
        {
            request.SetRequestSerailizedContent(this, body, GZipEnabled);
        }

        #region Serialization

        /// <inheritdoc/>
        public ISerializer Serializer { get; set; }

        /// <inheritdoc/>
        public virtual string SerializeObject(object obj) => Serializer.Serialize(obj);

        /// <inheritdoc/>
        public virtual async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // If a string is request, don't parse the response.
			if (Type.Equals(typeof(T), typeof(string)))
            {
                return (T)(object)text;
            }

            // Check if there was an error returned. The error node is returned in both paths
            // Deserialize the stream based upon the format of the stream.
            if (HasFeature(Core.Base.Discovery.Features.LegacyDataResponse))
            {
                // Legacy path (deprecated!)
                StandardResponse<T> sr = null;
                try
                {
                    sr = Serializer.Deserialize<StandardResponse<T>>(text);
                }
                catch (JsonReaderException ex)
                {
                    throw new GoogleApiException(Name,
                        "Failed to parse response from server as json [" + text + "]", ex);
                }

                if (sr.Error != null)
                {
                    throw new GoogleApiException(Name, "Server error - " + sr.Error)
                    {
                        Error = sr.Error
                    };
                }

                if (sr.Data == null)
                {
                    throw new GoogleApiException(Name, "The response could not be deserialized.");
                }
                return sr.Data;
            }

            // New path: Deserialize the object directly.
            T result = default(T);
            try
            {
                result = Serializer.Deserialize<T>(text);
            }
            catch (JsonReaderException ex)
            {
                throw new GoogleApiException(Name, "Failed to parse response from server as json [" + text + "]", ex);
            }

            // TODO(peleyal): is this the right place to check ETag? it isn't part of deserialization!
            // If this schema/object provides an error container, check it.
            var eTag = response.Headers.ETag != null ? response.Headers.ETag.Tag : null;
            if (result is IDirectResponseSchema && eTag != null)
            {
                (result as IDirectResponseSchema).ETag = eTag;
            }
            return result;
        }

        /// <inheritdoc/>
        public virtual async Task<RequestError> DeserializeError(HttpResponseMessage response)
        {
            if (response?.Content == null)
            {
                throw new GoogleApiException(Name, "response or response content unexpectedly null.");
            }
            // If we can't even read the response, let that exception bubble up.
            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            RequestError error;
            try
            {
                error = Serializer.Deserialize<StandardResponse<object>>(responseText)?.Error;
            }
            catch (JsonException ex)
            {
                throw new GoogleApiException(Name, responseText, ex)
                {
                    HttpStatusCode = response.StatusCode
                };
            }
            if (error == null)
            {
                throw new GoogleApiException(Name, "Error response is null")
                {
                    HttpStatusCode = response.StatusCode
                };
            }
            return error;
        }

        #endregion

        #region Abstract Members

        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public abstract string BaseUri { get; }

        /// <inheritdoc/>
        public abstract string BasePath { get; }

        /// <summary>The URI used for batch operations.</summary>
        public virtual string BatchUri { get { return null; } }

        /// <summary>The path used for batch operations.</summary>
        public virtual string BatchPath { get { return null; } }

        /// <inheritdoc/>
        public abstract IList<string> Features { get; }

        #endregion

        #endregion

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            if (HttpClient != null)
            {
                HttpClient.Dispose();
            }
        }
    }
}
