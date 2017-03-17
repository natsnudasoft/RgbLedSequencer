// <copyright file="LedGrayscaleTests.cs" company="natsnudasoft">
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
    using Moq;
    using NatsnudaLibrary.TestExtensions;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using Xunit;

    public sealed class LedGrayscaleTests
    {
        private static readonly Type SutType = typeof(LedGrayscale);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void WritablePropertiesAreCorrectlyImplemented(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfig,
            WritablePropertyAssertion assertion)
        {
            const byte MaxGrayscale = byte.MaxValue;
            sequencerConfig.Setup(c => c.MaxGrayscale).Returns(MaxGrayscale);
            assertion.Verify(
                SutType.GetProperty(nameof(LedGrayscale.Red)),
                SutType.GetProperty(nameof(LedGrayscale.Green)),
                SutType.GetProperty(nameof(LedGrayscale.Blue)));
        }

        [Theory]
        [AutoMoqData]
        public void ColorChannelChangesPropertyChangedIsRaised(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            PropertyChangedRaisedAssertion assertion)
        {
            const byte MaxGrayscale = byte.MaxValue;
            sequencerConfigMock.Setup(c => c.MaxGrayscale).Returns(MaxGrayscale);

            assertion.Verify(
                SutType.GetProperty(nameof(LedGrayscale.Red)),
                SutType.GetProperty(nameof(LedGrayscale.Green)),
                SutType.GetProperty(nameof(LedGrayscale.Blue)));
        }

        [Theory]
        [InlineAutoMoqData(nameof(LedGrayscale.Red))]
        [InlineAutoMoqData(nameof(LedGrayscale.Green))]
        [InlineAutoMoqData(nameof(LedGrayscale.Blue))]
        public void ColorChannelSetTooHighIsClamped(
            string propertyName,
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const byte MaxGrayscale = 50;
            sequencerConfigMock.Setup(c => c.MaxGrayscale).Returns(MaxGrayscale);
            var sut = fixture.Build<LedGrayscale>().OmitAutoProperties().Create();

            SutType.GetProperty(propertyName).SetValue(sut, byte.MaxValue);
            var actualValue = SutType.GetProperty(propertyName).GetValue(sut);

            Assert.Equal(MaxGrayscale, actualValue);
        }

        [Theory]
        [AutoMoqData]
        public void DebuggerDisplayDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetProperty(nameof(LedGrayscale.DebuggerDisplay)));
        }
    }
}
