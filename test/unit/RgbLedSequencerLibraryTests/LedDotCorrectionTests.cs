// <copyright file="LedDotCorrectionTests.cs" company="natsnudasoft">
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

    public sealed class LedDotCorrectionTests
    {
        private static readonly Type SutType = typeof(LedDotCorrection);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            byte maxDotCorrection,
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            sequencerConfigMock.Setup(c => c.MaxDotCorrection).Returns(maxDotCorrection);
            var sut = fixture.Build<LedDotCorrection>().OmitAutoProperties().Create();

            Assert.Equal(maxDotCorrection, sut.Red);
            Assert.Equal(maxDotCorrection, sut.Green);
            Assert.Equal(maxDotCorrection, sut.Blue);
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
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            WritablePropertyAssertion assertion)
        {
            const byte MaxDotCorrection = byte.MaxValue;
            sequencerConfigMock.Setup(c => c.MaxDotCorrection).Returns(MaxDotCorrection);
            assertion.Verify(
                SutType.GetProperty(nameof(LedDotCorrection.Red)),
                SutType.GetProperty(nameof(LedDotCorrection.Green)),
                SutType.GetProperty(nameof(LedDotCorrection.Blue)));
        }

        [Theory]
        [AutoMoqData]
        public void ColorChannelChangesPropertyChangedIsRaised(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            PropertyChangedRaisedAssertion assertion)
        {
            const byte MaxDotCorrection = byte.MaxValue;
            sequencerConfigMock.Setup(c => c.MaxDotCorrection).Returns(MaxDotCorrection);

            assertion.Verify(
                SutType.GetProperty(nameof(LedDotCorrection.Red)),
                SutType.GetProperty(nameof(LedDotCorrection.Green)),
                SutType.GetProperty(nameof(LedDotCorrection.Blue)));
        }

        [Theory]
        [InlineAutoMoqData(nameof(LedDotCorrection.Red))]
        [InlineAutoMoqData(nameof(LedDotCorrection.Green))]
        [InlineAutoMoqData(nameof(LedDotCorrection.Blue))]
        public void ColorChannelSetTooHighIsClamped(
            string propertyName,
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const byte MaxDotCorrection = 50;
            sequencerConfigMock.Setup(c => c.MaxDotCorrection).Returns(MaxDotCorrection);
            var sut = fixture.Build<LedDotCorrection>().OmitAutoProperties().Create();

            SutType.GetProperty(propertyName).SetValue(sut, (byte)0);
            SutType.GetProperty(propertyName).SetValue(sut, byte.MaxValue);
            var actualValue = SutType.GetProperty(propertyName).GetValue(sut);

            Assert.Equal(MaxDotCorrection, actualValue);
        }

        [Theory]
        [AutoMoqData]
        public void DebuggerDisplayDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetProperty(nameof(LedDotCorrection.DebuggerDisplay)));
        }
    }
}
