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

namespace Natsnudasoft.RgbLedSequencerLibraryTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Helper;
    using Moq;
    using NatsnudaLibrary.TestExtensions;
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
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepCount = 770,
                StepCount = 10,
                MaxStepDelay = 1000,
                StepDelay = 500
            };
            fixture.Customize(customization);
#pragma warning disable SA1118 // Parameter must not span multiple lines
            var behaviorExpectation = new CompositeBehaviorExpectation(
                new ParameterNullReferenceBehaviorExpectation(fixture),
                new ExceptionBehaviorExpectation<ArgumentOutOfRangeException>(
                    fixture,
                    "stepCount",
                    customization.MaxStepCount + 1,
                    -1),
                new ExceptionBehaviorExpectation<ArgumentException>(
                    fixture,
                    "sequenceSteps",
                    new[]
                    {
                        fixture.CreateMany<SequenceStep>(customization.MaxStepCount + 1).ToArray()
                    }));
#pragma warning restore SA1118 // Parameter must not span multiple lines
            var assertion = new GuardClauseAssertion(fixture, behaviorExpectation);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorWithSequenceStepsSetsCorrectInitializedMembers(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepDelay = 500,
                StepDelay = 50,
                MaxStepCount = 10,
                StepCount = 8
            };
            fixture.Customize(customization);
            var sequenceSteps =
                fixture.CreateMany<SequenceStep>(customization.StepCount.Value).ToArray();
            var sut = new SequenceData(sequencerConfigMock.Object, sequenceSteps);

            Assert.Equal(customization.StepCount.Value, sut.StepCount);
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorWithFactorySetsCorrectInitializedMembers(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepDelay = 500,
                StepDelay = 50,
                MaxStepCount = 10,
                StepCount = 8
            };
            fixture.Customize(customization);
            var sequenceStepFactory = fixture.Create<Func<SequenceStep>>();
            var sut = new SequenceData(
                sequencerConfigMock.Object,
                sequenceStepFactory,
                customization.StepCount.Value);

            Assert.Equal(customization.StepCount.Value, sut.StepCount);
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            DoesNotThrowAssertion assertion,
            Fixture fixture)
        {
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                MaxStepCount = int.MaxValue,
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [InlineAutoMoqData(false)]
        [InlineAutoMoqData(true)]
        public void IndexerInvalidValueThrows(
            bool asIReadOnlyList,
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                MaxStepCount = 770,
                StepCount = 5
            };
            fixture.Customize(customization);
            var sut = fixture.Create<SequenceData>();
            Exception ex;
            if (asIReadOnlyList)
            {
                ex = Record.Exception(() =>
                    ((IReadOnlyList<SequenceStep>)sut)[customization.StepCount.Value]);
            }
            else
            {
                ex = Record.Exception(() => sut[customization.StepCount.Value]);
            }

            Assert.IsType<IndexOutOfRangeException>(ex);
        }

        [Theory]
        [AutoMoqData]
        public void IndexerValidValueReturnsSequenceStep(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                MaxStepCount = 770,
                StepCount = 8
            };
            fixture.Customize(customization);
            var sut = fixture.Create<SequenceData>();

            for (int i = 0; i < customization.StepCount; ++i)
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
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                MaxStepCount = 770,
                StepCount = 9
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetProperty(nameof(SequenceData.DebuggerDisplay)));
        }

        [Theory]
        [AutoMoqData]
        public void AsIReadOnlyListHasCorrectCount(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                MaxStepCount = 770,
                StepCount = 9
            };
            fixture.Customize(customization);
            var sut = (IReadOnlyList<SequenceStep>)fixture.Create<SequenceData>();

            Assert.Equal(customization.StepCount.Value, sut.Count);
        }

        [Theory]
        [AutoMoqData]
        public void EnumeratorIsValid(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                MaxStepCount = 770,
                StepCount = 9
            };
            fixture.Customize(customization);
            var expected = fixture.Create<SequenceStep[]>();
            var sut = new SequenceData(sequencerConfigMock.Object, expected);

            Assert.Equal(expected, sut);
        }
    }
}
