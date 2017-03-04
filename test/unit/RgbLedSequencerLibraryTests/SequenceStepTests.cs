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
    using System.Collections.Generic;
    using Extension;
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
        public void ConstructorHasCorrectGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            [Frozen]GrayscaleData grayscaleData,
            SequenceStep sut)
        {
            Assert.Equal(grayscaleData, sut.GrayscaleData);
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void StepDelayChangesPropertyChangedIsRaised(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const string PropertyName = nameof(SequenceStep.StepDelay);
            const byte MaxStepDelay = 100;
            fixture.Customizations.Add(new RandomNumericSequenceGenerator(1, MaxStepDelay));
            var propertyValue = fixture.Create<byte>();
            sequencerConfigMock.Setup(c => c.MaxStepDelay).Returns(MaxStepDelay);
            var sut = fixture.Build<SequenceStep>().OmitAutoProperties().Create();
            var receivedEvents = new List<string>();
            sut.PropertyChanged += (s, e) =>
            {
                receivedEvents.Add(e.PropertyName);
            };

            sut.StepDelay = propertyValue;

            Assert.Equal(1, receivedEvents.Count);
            Assert.Equal(PropertyName, receivedEvents[0]);
        }

        [Theory]
        [InlineAutoMoqData(-50)]
        [InlineAutoMoqData(100)]
        public void StepDelaySetOutOfRangeIsClamped(
            int value,
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const string PropertyName = nameof(SequenceStep.StepDelay);
            const byte MaxStepDelay = 50;
            sequencerConfigMock.Setup(c => c.MaxStepDelay).Returns(MaxStepDelay);
            var sut = fixture.Create<SequenceStep>();

            SutType.GetProperty(PropertyName).SetValue(sut, value);
            var actualValue = (int)SutType.GetProperty(PropertyName).GetValue(sut);

            Assert.InRange(actualValue, 0, MaxStepDelay);
        }

        [Theory]
        [AutoMoqData]
        public void DebuggerDisplayDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            DoesNotThrowAssertion assertion)
        {
            const int RgbLedCount = 5;
            sequencerConfigMock.Setup(c => c.RgbLedCount).Returns(RgbLedCount);

            assertion.Verify(SutType.GetProperty(nameof(SequenceStep.DebuggerDisplay)));
        }
    }
}
