// <copyright file="ExceptionBehaviourExpectation.cs" company="natsnudasoft">
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
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using RgbLedSequencerLibrary;
    using static System.FormattableString;

    /// <summary>
    /// Provides a behaviour expectation that will apply a known value to a parameter in a
    /// constructor and check for a specified exception when calling the constructor with that
    /// value.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the expected exception.</typeparam>
    /// <seealso cref="GuardClauseExtensions" />
    /// <seealso cref="IBehaviorExpectation" />
    public class ExceptionBehaviourExpectation<T> : IBehaviorExpectation
        where T : Exception
    {
        private readonly IFixture fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionBehaviourExpectation{T}"/> class.
        /// </summary>
        /// <param name="fixture">The anonymous object creation service used to workaround
        /// a problem with guard clause assertions.</param>
        /// <param name="parameterName">The name of the parameter to apply a known value to.</param>
        /// <param name="values">The values to apply to the parameter to assert the guard clause.
        /// </param>
        public ExceptionBehaviourExpectation(
            IFixture fixture,
            string parameterName,
            params object[] values)
        {
            ParameterValidation.IsNotNull(fixture, nameof(fixture));
            ParameterValidation.IsNotNull(parameterName, nameof(parameterName));
            ParameterValidation.IsNotNull(values, nameof(values));
            if (values.Length == 0)
            {
                throw new ArgumentException("Collection must not be empty.", nameof(values));
            }

            this.fixture = fixture;
            this.ParameterName = parameterName;
            this.Values = values;
        }

        /// <summary>
        /// Gets the name of the parameter to apply a known value to.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Gets the values to apply to the parameter to assert the guard clause.
        /// </summary>
        public IEnumerable<object> Values { get; }

        /// <inheritdoc/>
        public void Verify(IGuardClauseCommand command)
        {
            ParameterValidation.IsNotNull(command, nameof(command));

            var methodInvokeCommand = GuardClauseExtensions.GetMethodInvokeCommand(command);

            if (methodInvokeCommand != null &&
                methodInvokeCommand.ParameterInfo.Name == this.ParameterName)
            {
                var newCommand =
                    GuardClauseExtensions.CreateExtendedCommand(this.fixture, command);
                foreach (var value in this.Values)
                {
                    var expectedExceptionThrown = false;
                    try
                    {
                        newCommand.Execute(value);
                    }
                    catch (Exception ex) when (ex.GetType() == typeof(T))
                    {
                        expectedExceptionThrown = true;
                    }
                    catch (Exception ex)
                    {
                        throw newCommand.CreateException(Invariant($"\"{value.ToString()}\""), ex);
                    }

                    if (!expectedExceptionThrown)
                    {
                        throw newCommand.CreateException(Invariant($"\"{value.ToString()}\""));
                    }
                }
            }
        }
    }
}
