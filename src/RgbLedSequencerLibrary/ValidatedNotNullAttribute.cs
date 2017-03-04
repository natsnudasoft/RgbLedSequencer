// <copyright file="ValidatedNotNullAttribute.cs" company="natsnudasoft">
// Copyright (c) Adrian John Dunstan. All rights reserved.
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
// </copyright>

namespace RgbLedSequencerLibrary
{
    using System;

    /// <summary>
    /// Provides a workaround attribute to CA1062 being triggered when parameter validation is
    /// performed by an external method.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}
