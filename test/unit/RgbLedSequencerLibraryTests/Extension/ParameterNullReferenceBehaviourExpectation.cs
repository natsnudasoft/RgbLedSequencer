// <copyright file="ParameterNullReferenceBehaviourExpectation.cs" company="natsnudasoft">
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
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using RgbLedSequencerLibrary;

    /// <summary>
    /// Encapsulates expectations about the behaviour of a method or constructor when it's invoked
    /// with a null argument. Works around guard clause assertions using Type instead of
    /// ParameterInfo to resolve specimens.
    /// </summary>
    public sealed class ParameterNullReferenceBehaviourExpectation : IBehaviorExpectation
    {
        private readonly IFixture fixture;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ParameterNullReferenceBehaviourExpectation"/> class.
        /// </summary>
        /// <param name="fixture">The anonymous object creation service used to workaround
        /// a problem with guard clause assertions.</param>
        public ParameterNullReferenceBehaviourExpectation(IFixture fixture)
        {
            ParameterValidation.IsNotNull(fixture, nameof(fixture));

            this.fixture = fixture;
        }

        public void Verify(IGuardClauseCommand command)
        {
            if (command != null &&
                (command.RequestedType.IsClass || command.RequestedType.IsInterface))
            {
                var newCommand =
                    GuardClauseExtensions.CreateExtendedCommand(this.fixture, command);
                var expectedExceptionThrown = false;
                try
                {
                    newCommand.Execute(null);
                }
                catch (ArgumentNullException)
                {
                    expectedExceptionThrown = true;
                }
                catch (Exception ex)
                {
                    throw newCommand.CreateException("null", ex);
                }

                if (!expectedExceptionThrown)
                {
                    throw newCommand.CreateException("null");
                }
            }
        }
    }
}
