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
using System.Reflection;
using Lacey.Medusa.Common.Api.Core.Base.Logging;
using Lacey.Medusa.Common.Api.Core.Base.Util;

namespace Lacey.Medusa.Common.Api.Core.Base.Requests.Parameters
{
    /// <summary>
    /// Utility class for iterating on <see cref="RequestParameterAttribute"/> properties in a request object.
    /// </summary>
    public static class ParameterUtils
    {
        private static readonly ILogger Logger = ApplicationContext.Logger.ForType(typeof(ParameterUtils));

        /// <summary>
        /// Creates a <see cref="FormUrlEncodedContent"/> with all the specified parameters in 
        /// the input request. It uses reflection to iterate over all properties with
        /// <see cref="RequestParameterAttribute"/> attribute.
        /// </summary>
        /// <param name="request">
        /// A request object which contains properties with 
        /// <see cref="RequestParameterAttribute"/> attribute. Those properties will be serialized
        /// to the returned <see cref="FormUrlEncodedContent"/>.
        /// </param>
        /// <returns>
        /// A <see cref="FormUrlEncodedContent"/> which contains the all the given object required 
        /// values.
        /// </returns>
        public static FormUrlEncodedContent CreateFormUrlEncodedContent(object request)
        {
            IList<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            IterateParameters(request, (type, name, value) =>
                {
                    list.Add(new KeyValuePair<string, string>(name, value.ToString()));
                });
            return new FormUrlEncodedContent(list);
        }

        /// <summary>
        /// Creates a parameter dictionary by using reflection to iterate over all properties with
        /// <see cref="RequestParameterAttribute"/> attribute.
        /// </summary>
        /// <param name="request">
        /// A request object which contains properties with 
        /// <see cref="RequestParameterAttribute"/> attribute. Those properties will be set
        /// in the output dictionary.
        /// </param>
        public static IDictionary<string, object> CreateParameterDictionary(object request)
        {
            var dict = new Dictionary<string, object>();
            IterateParameters(request, (type, name, value) =>
                {
                    dict.Add(name, value);
                });
            return dict;
        }

        /// <summary>
        /// Sets query parameters in the given builder with all all properties with the
        /// <see cref="RequestParameterAttribute"/> attribute.
        /// </summary>
        /// <param name="builder">The request builder</param>
        /// <param name="request">
        /// A request object which contains properties with 
        /// <see cref="RequestParameterAttribute"/> attribute. Those properties will be set in the 
        /// given request builder object
        /// </param>
        public static void InitParameters(RequestBuilder builder, object request)
        {
            IterateParameters(request, (type, name, value) =>
                {
                    builder.AddParameter(type, name, value.ToString());
                });
        }

        /// <summary>
        /// Iterates over all <see cref="RequestParameterAttribute"/> properties in the request
        /// object and invokes the specified action for each of them.
        /// </summary>
        /// <param name="request">A request object</param>
        /// <param name="action">An action to invoke which gets the parameter type, name and its value</param>
        private static void IterateParameters(object request, Action<RequestParameterType, string, object> action)
        {
            // Use reflection to build the parameter dictionary.
            foreach (PropertyInfo property in request.GetType().GetProperties(BindingFlags.Instance |
                BindingFlags.Public))
            {
                // Retrieve the RequestParameterAttribute.
                RequestParameterAttribute attribute =
                    property.GetCustomAttributes(typeof(RequestParameterAttribute), false).FirstOrDefault() as
                    RequestParameterAttribute;
                if (attribute == null)
                {
                    continue;
                }

                // Get the name of this parameter from the attribute, if it doesn't exist take a lower-case variant of
                // property name.
                string name = attribute.Name ?? property.Name.ToLower();

                var propertyType = property.PropertyType;
                var value = property.GetValue(request, null);

                // Call action with the type name and value.
                if (propertyType.GetTypeInfo().IsValueType || value != null)
                {
                    if (attribute.Type == RequestParameterType.UserDefinedQueries)
                    {
                        if (typeof(IEnumerable<KeyValuePair<string, string>>).IsAssignableFrom(value.GetType()))
                        {
                            foreach (var pair in (IEnumerable<KeyValuePair<string, string>>)value)
                            {
                                action(RequestParameterType.Query, pair.Key, pair.Value);
                            }
                        }
                        else
                        {
                            Logger.Warning("Parameter marked with RequestParameterType.UserDefinedQueries attribute " +
                                "was not of type IEnumerable<KeyValuePair<string, string>> and will be skipped.");
                        }
                    }
                    else
                    {
                        action(attribute.Type, name, value);
                    }
                }
            }
        }
    }
}
