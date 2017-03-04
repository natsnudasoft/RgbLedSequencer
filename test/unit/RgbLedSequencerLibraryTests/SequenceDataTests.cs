// <copyright file="SequenceDataTests.cs" company="natsnudasoft">
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

namespace RgbLedSequencerLibraryTests
{
    using System;
    using Extension;
    using Moq;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using Xunit;

    public sealed class SequenceDataTests
    {
        private static readonly Type SutType = typeof(SequenceData);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const int MaxStepCount = 770;
            sequencerConfigMock.Setup(c => c.MaxStepCount).Returns(MaxStepCount);
#pragma warning disable SA1118 // Parameter must not span multiple lines
            var behaviourExpectation = new CompositeBehaviorExpectation(
                new ParameterNullReferenceBehaviourExpectation(fixture),
                new ExceptionBehaviourExpectation<ArgumentOutOfRangeException>(
                    fixture,
                    "stepCount",
                    771,
                    -1));
#pragma warning restore SA1118 // Parameter must not span multiple lines
            var assertion = new GuardClauseAssertion(fixture, behaviourExpectation);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            ConstructorInitializedMemberAssertion assertion)
        {
            const int MaxStepCount = int.MaxValue;
            sequencerConfigMock.Setup(c => c.MaxStepCount).Returns(MaxStepCount);

            assertion.Verify(SutType.GetProperty(nameof(SequenceData.StepCount)));
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            DoesNotThrowAssertion assertion)
        {
            const int MaxStepCount = int.MaxValue;
            sequencerConfigMock.Setup(c => c.MaxStepCount).Returns(MaxStepCount);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void IndexerInvalidValueThrows(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const int MaxStepCount = 770;
            const int StepCount = 5;
            sequencerConfigMock.Setup(c => c.MaxStepCount).Returns(MaxStepCount);
            ApplyStepCountSpecimen(fixture, StepCount);
            var sut = fixture.Create<SequenceData>();

            var ex = Record.Exception(() => sut[StepCount]);

            Assert.IsType<IndexOutOfRangeException>(ex);
        }

        [Theory]
        [AutoMoqData]
        public void IndexerValidValueReturnsSequenceStep(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const int MaxStepCount = 770;
            const int StepCount = 8;
            sequencerConfigMock.Setup(c => c.MaxStepCount).Returns(MaxStepCount);
            ApplyStepCountSpecimen(fixture, StepCount);
            var sut = fixture.Create<SequenceData>();

            for (int i = 0; i < StepCount; ++i)
            {
                Assert.IsType<SequenceStep>(sut[i]);
                Assert.NotNull(sut[i]);
            }
        }

        [Theory]
        [AutoMoqData]
        public void DebuggerDisplayDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            DoesNotThrowAssertion assertion,
            Fixture fixture)
        {
            const int MaxStepCount = 770;
            const int StepCount = 5;
            sequencerConfigMock.Setup(c => c.MaxStepCount).Returns(MaxStepCount);
            ApplyStepCountSpecimen(fixture, StepCount);

            assertion.Verify(SutType.GetProperty(nameof(SequenceData.DebuggerDisplay)));
        }

        private static void ApplyStepCountSpecimen(
            IFixture fixture,
            int stepCount)
        {
            fixture.Customizations.Add(new ParameterSpecimenBuilder(
                SutType,
                nameof(stepCount),
                stepCount));
        }
    }
}
