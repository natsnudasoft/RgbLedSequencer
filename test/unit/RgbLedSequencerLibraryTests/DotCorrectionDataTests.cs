// <copyright file="DotCorrectionDataTests.cs" company="natsnudasoft">
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

    public sealed class DotCorrectionDataTests
    {
        private static readonly Type SutType = typeof(DotCorrectionData);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxDotCorrection = byte.MaxValue
            };
            fixture.Customize(customization);
#pragma warning disable SA1118 // Parameter must not span multiple lines
            var behaviorExpectation = new CompositeBehaviorExpectation(
                new ParameterNullReferenceBehaviorExpectation(fixture),
                new ExceptionBehaviorExpectation<ArgumentException>(
                    fixture,
                    "ledDotCorrections",
                    fixture.CreateMany<LedDotCorrection>(customization.RgbLedCount + 1).ToArray(),
                    fixture.CreateMany<LedDotCorrection>(customization.RgbLedCount - 1).ToArray()));
#pragma warning restore SA1118 // Parameter must not span multiple lines
            var assertion = new GuardClauseAssertion(fixture, behaviorExpectation);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            DoesNotThrowAssertion assertion,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxDotCorrection = byte.MaxValue
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void IndexerValidValueReturnsLedDotCorrection(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxDotCorrection = byte.MaxValue
            };
            fixture.Customize(customization);
            var sut = fixture.Create<DotCorrectionData>();

            for (int i = 0; i < customization.RgbLedCount; ++i)
            {
                Assert.IsType<LedDotCorrection>(sut[i]);
                Assert.NotNull(sut[i]);
            }
        }

        [Theory]
        [InlineAutoMoqData(false)]
        [InlineAutoMoqData(true)]
        public void IndexerInvalidValueThrows(
            bool asIReadOnlyList,
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxDotCorrection = byte.MaxValue
            };
            fixture.Customize(customization);
            var sut = fixture.Create<DotCorrectionData>();
            Exception ex;
            if (asIReadOnlyList)
            {
                ex = Record.Exception(() =>
                    ((IReadOnlyList<LedDotCorrection>)sut)[customization.RgbLedCount]);
            }
            else
            {
                ex = Record.Exception(() => sut[customization.RgbLedCount]);
            }

            Assert.IsType<IndexOutOfRangeException>(ex);
        }

        [Theory]
        [AutoMoqData]
        public void DebuggerDisplayDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            DoesNotThrowAssertion assertion,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxDotCorrection = byte.MaxValue
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetProperty(nameof(DotCorrectionData.DebuggerDisplay)));
        }

        [Theory]
        [AutoMoqData]
        public void AsIReadOnlyListHasCorrectCount(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxDotCorrection = byte.MaxValue
            };
            fixture.Customize(customization);
            var sut = (IReadOnlyList<LedDotCorrection>)fixture.Create<DotCorrectionData>();

            Assert.Equal(customization.RgbLedCount, sut.Count);
        }

        [Theory]
        [AutoMoqData]
        public void EnumeratorIsValid(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxDotCorrection = byte.MaxValue
            };
            fixture.Customize(customization);
            var expected = fixture.Create<ICollection<LedDotCorrection>>();
            var sut = new DotCorrectionData(sequencerConfigMock.Object, expected);

            Assert.Equal(expected, sut);
        }
    }
}
