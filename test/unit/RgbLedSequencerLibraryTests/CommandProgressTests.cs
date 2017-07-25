// <copyright file="CommandProgressTests.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerLibraryTests
{
    using System;
    using NatsnudaLibrary.TestExtensions;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Xunit;
    using SutAlias = RgbLedSequencerLibrary.CommandProgress;

    public sealed class CommandProgressTests
    {
        private static readonly Type SutType = typeof(SutAlias);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(Fixture fixture)
        {
            const double ProgressPercentage = 50d;
            ApplyProgressPercentageSpecimen(fixture, ProgressPercentage);
#pragma warning disable SA1118 // Parameter must not span multiple lines
            var behaviorExpectation = new CompositeBehaviorExpectation(
                new ParameterNullReferenceBehaviorExpectation(fixture),
                new ExceptionBehaviorExpectation<ArgumentOutOfRangeException>(
                    fixture,
                    "progressPercentage",
                    200d,
                    -50d),
                new ExceptionBehaviorExpectation<ArgumentException>(
                    fixture,
                    "currentAction",
                    string.Empty));
#pragma warning restore SA1118 // Parameter must not span multiple lines
            var assertion = new GuardClauseAssertion(fixture, behaviorExpectation);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            ConstructorInitializedMemberAssertion assertion,
            Fixture fixture)
        {
            const double ProgressPercentage = 50d;
            ApplyProgressPercentageSpecimen(fixture, ProgressPercentage);

            assertion.Verify(
                SutType.GetProperty(nameof(SutAlias.ProgressPercentage)),
                SutType.GetProperty(nameof(SutAlias.CurrentAction)));
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion, Fixture fixture)
        {
            const double ProgressPercentage = 50d;
            ApplyProgressPercentageSpecimen(fixture, ProgressPercentage);

            assertion.Verify(SutType.GetConstructors());
        }

        private static void ApplyProgressPercentageSpecimen(
            IFixture fixture,
            double progressPercentage)
        {
            fixture.Customizations.Add(new ParameterSpecimenBuilder(
                SutType,
                nameof(progressPercentage),
                progressPercentage));
        }
    }
}
