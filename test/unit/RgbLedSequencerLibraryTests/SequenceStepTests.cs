// <copyright file="SequenceStepTests.cs" company="natsnudasoft">
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
    using Helper;
    using Moq;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using Xunit;

    public sealed class SequenceStepTests
    {
        private static readonly Type SutType = typeof(SequenceStep);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new SequenceStepCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepDelay = 1000,
                StepDelay = 500
            };
            fixture.Customize(customization);
            var behaviourExpectation = new ParameterNullReferenceBehaviourExpectation(fixture);
            var assertion = new GuardClauseAssertion(fixture, behaviourExpectation);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new GrayscaleDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5
            };
            fixture.Customize(customization);
            var grayscaleData = fixture.Create<GrayscaleData>();
            Func<GrayscaleData> grayscaleDataFactory = () => grayscaleData;
            var sut = new SequenceStep(sequencerConfigMock.Object, grayscaleDataFactory);

            Assert.Equal(grayscaleData, sut.GrayscaleData);
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            DoesNotThrowAssertion assertion,
            Fixture fixture)
        {
            var customization = new SequenceStepCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepDelay = 1000,
                StepDelay = 500
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void WritablePropertiesAreCorrectlyImplemented(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            WritablePropertyAssertion assertion,
            Fixture fixture)
        {
            var customization = new SequenceStepCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepDelay = int.MaxValue,
                StepDelay = 500
            };
            fixture.Customize(customization);

            assertion.Verify(
                SutType.GetProperty(nameof(SequenceStep.StepDelay)));
        }

        [Theory]
        [AutoMoqData]
        public void StepDelayChangesPropertyChangedIsRaised(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            PropertyChangedRaisedAssertion assertion,
            Fixture fixture)
        {
            var customization = new SequenceStepCustomization(sequencerConfigMock)
            {
                MaxStepDelay = int.MaxValue
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetProperty(nameof(SequenceStep.StepDelay)));
        }

        [Theory]
        [InlineAutoMoqData(-50)]
        [InlineAutoMoqData(100)]
        public void StepDelaySetOutOfRangeIsClamped(
            int value,
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new SequenceStepCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepDelay = 50,
                StepDelay = 0
            };
            fixture.Customize(customization);
            var sut = fixture.Create<SequenceStep>();
            const string PropertyName = nameof(SequenceStep.StepDelay);

            SutType.GetProperty(PropertyName).SetValue(sut, value);
            var actualValue = (int)SutType.GetProperty(PropertyName).GetValue(sut);

            Assert.InRange(actualValue, 0, customization.MaxStepDelay);
        }

        [Theory]
        [AutoMoqData]
        public void DebuggerDisplayDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            DoesNotThrowAssertion assertion,
            Fixture fixture)
        {
            var customization = new SequenceStepCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepDelay = 50,
                StepDelay = 0
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetProperty(nameof(SequenceStep.DebuggerDisplay)));
        }
    }
}
