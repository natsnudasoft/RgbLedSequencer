// <copyright file="ParameterAttribute.cs" company="natsnudasoft">
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

namespace RgbLedSequencerLibraryTests.Extension
{
    using System;
    using System.Reflection;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;

    /// <summary>
    /// Provides a customization attribute that will apply a known value to a parameter in a
    /// constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public sealed class ParameterAttribute : CustomizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to apply a known value to.</param>
        public ParameterAttribute(string parameterName)
        {
            ParameterValidation.IsNotNull(parameterName, nameof(parameterName));

            this.ParameterName = parameterName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to apply a known value to.</param>
        /// <param name="specimenValue">The value to apply to the parameter.</param>
        public ParameterAttribute(string parameterName, object specimenValue)
        {
            ParameterValidation.IsNotNull(parameterName, nameof(parameterName));
            ParameterValidation.IsNotNull(specimenValue, nameof(specimenValue));

            this.ParameterName = parameterName;
            this.SpecimenValue = specimenValue;
        }

        /// <summary>
        /// Gets the name of the parameter to apply a known value to.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Gets the value to apply to the parameter.
        /// </summary>
        public object SpecimenValue { get; }

        /// <inheritdoc/>
        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            return new ParameterCustomization(
                parameter.ParameterType,
                this.ParameterName,
                this.SpecimenValue);
        }
    }
}
