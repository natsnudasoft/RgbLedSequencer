// <copyright file="ParameterTypeFavoringConstructorQuery.cs" company="natsnudasoft">
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
    using System.Collections.Generic;
    using Ploeh.AutoFixture.Kernel;
    using RgbLedSequencerLibrary;

    /// <summary>
    /// Provides a constructor query that chooses a constructor that has the specified types in the
    /// specified order.
    /// </summary>
    /// <seealso cref="IMethodQuery" />
    public sealed class ParameterTypeFavoringConstructorQuery : IMethodQuery
    {
        private readonly Type[] parameterTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterTypeFavoringConstructorQuery"/>
        /// class.
        /// </summary>
        /// <param name="parameterTypes">An array of <see cref="Type"/> objects representing the
        /// number, order, and type of the parameters for the desired constructor.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameterTypes"/> is
        /// <see langword="null"/>.</exception>
        public ParameterTypeFavoringConstructorQuery(params Type[] parameterTypes)
        {
            ParameterValidation.IsNotNull(parameterTypes, nameof(parameterTypes));

            this.parameterTypes = parameterTypes;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">A public constructor with the specified parameter
        /// types could not be found on the <see cref="Type"/> specified by <paramref name="type"/>.
        /// </exception>
        public IEnumerable<IMethod> SelectMethods(Type type)
        {
            ParameterValidation.IsNotNull(type, nameof(type));

            var constructor = type.GetConstructor(this.parameterTypes);
            if (constructor == null)
            {
                throw new ArgumentException(
                    "A public constructor with the specified parameter types could not be found.",
                    nameof(type));
            }

            return new[] { new ConstructorMethod(constructor) };
        }
    }
}
