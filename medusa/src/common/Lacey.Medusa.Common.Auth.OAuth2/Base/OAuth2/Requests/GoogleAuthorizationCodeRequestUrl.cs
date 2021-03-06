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

namespace Lacey.Medusa.Common.Auth.OAuth2.Base.OAuth2.Requests
{
    /// <summary>
    /// Google-specific implementation of the OAuth 2.0 URL for an authorization web page to allow the end user to 
    /// authorize the application to access their protected resources and that returns an authorization code, as 
    /// specified in https://developers.google.com/accounts/docs/OAuth2WebServer.
    /// </summary>
    public class GoogleAuthorizationCodeRequestUrl : AuthorizationCodeRequestUrl
    {
        /// <summary>
        /// Gets or sets the access type. Set <c>online</c> to request on-line access or <c>offline</c> to request 
        /// off-line access or <c>null</c> for the default behavior. The default value is <c>offline</c>.
        /// </summary>
        [Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterAttribute("access_type", Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterType.Query)]
        public string AccessType { get; set; }

        /// <summary>
        /// Gets of sets prompt for consent behaviour.
        /// Value can be <c>null</c>, <c>"none"</c>, <c>"consent"</c>, or <c>"select_account"</c>.
        /// See <a href="https://developers.google.com/identity/protocols/OpenIDConnect#prompt">OpenIDConnect documentation</a>
        /// for details.
        /// </summary>
        [Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterAttribute("prompt", Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterType.Query)]
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets prompt for consent behavior <c>auto</c> to request auto-approval or<c>force</c> to force the 
        /// approval UI to show, or <c>null</c> for the default behavior.
        /// </summary>
        [Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterAttribute("approval_prompt", Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterType.Query)]
        [Obsolete("Unused for Google OpenID; use the 'Prompt' property instead.")]
        public string ApprovalPrompt { get; set; }

        /// <summary>
        /// Gets or sets the login hint. Sets <c>email address</c> or sub <c>identifier</c>.
        /// When your application knows which user it is trying to authenticate, it may provide this parameter as a
        /// hint to the Authentication Server. Passing this hint will either pre-fill the email box on the sign-in form
        /// or select the proper multi-login session, thereby simplifying the login flow.
        /// </summary>
        [Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterAttribute("login_hint", Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterType.Query)]
        public string LoginHint { get; set; }

        /// <summary>
        /// Gets or sets the include granted scopes to determine if this authorization request should use
        /// incremental authorization (https://developers.google.com/+/web/api/rest/oauth#incremental-auth).
        /// If true and the authorization request is granted, the authorization will include any previous 
        /// authorizations granted to this user/application combination for other scopes.
        /// </summary>
        /// <remarks>Currently unsupported for installed apps.</remarks>
        [Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterAttribute("include_granted_scopes",
            Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterType.Query)]
        public string IncludeGrantedScopes { get; set; }

        /// <summary>
        /// Gets or sets a collection of user defined query parameters to facilitate any not explicitly supported
        /// by the library which will be included in the resultant authentication URL.
        /// </summary>
        /// <remarks>
        /// The name of this parameter is used only for the constructor and will not end up in the resultant query
        /// string.
        /// </remarks>
        [Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterAttribute("user_defined_query_params",
            Lacey.Medusa.Common.Api.Core.Base.Util.RequestParameterType.UserDefinedQueries)]
        public IEnumerable<KeyValuePair<string, string>> UserDefinedQueryParams { get; set; }

        /// <summary>
        /// Constructs a new authorization code request with the given authorization server URL. This constructor sets
        /// the <see cref="AccessType"/> to <c>offline</c>.
        /// </summary>
        public GoogleAuthorizationCodeRequestUrl(Uri authorizationServerUrl)
            : base(authorizationServerUrl)
        {
            AccessType = "offline";
        }
    }
}