/*
Copyright 2010 Google Inc

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

namespace Lacey.Medusa.Common.Api.Core.Base.Testing
{
    /// <summary>
    /// Marker Attribute to indicate a Method/Class/Property has been made more visible for purpose of testing.
    /// Mark the member as internal and make the testing assembly a friend using
    /// <code>[assembly: InternalsVisibleTo("Full.Name.Of.Testing.Assembly")]</code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property |
        AttributeTargets.Field)]
    public class VisibleForTestOnly : Attribute { }
}