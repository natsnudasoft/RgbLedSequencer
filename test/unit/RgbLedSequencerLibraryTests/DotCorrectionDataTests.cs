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

    public sealed class DotCorrectionDataTests
    {
        private static readonly Type SutType = typeof(DotCorrectionData);

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
        public void IndexerValidValueReturnsLedDotCorrection(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const int RgbLedCount = 5;
            sequencerConfigMock.Setup(c => c.RgbLedCount).Returns(RgbLedCount);
            var sut = fixture.Create<DotCorrectionData>();

            for (int i = 0; i < RgbLedCount; ++i)
            {
                Assert.IsType<LedDotCorrection>(sut[i]);
                Assert.NotNull(sut[i]);
            }
        }

        [Theory]
        [AutoMoqData]
        public void IndexerInvalidValueThrows(DotCorrectionData sut)
        {
            var ex = Record.Exception(() => sut[0]);

            Assert.IsType<IndexOutOfRangeException>(ex);
        }

        [Theory]
        [AutoMoqData]
        public void DebuggerDisplayDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            DoesNotThrowAssertion assertion)
        {
            const int RgbLedCount = 5;
            sequencerConfigMock.Setup(c => c.RgbLedCount).Returns(RgbLedCount);

            assertion.Verify(SutType.GetProperty(nameof(DotCorrectionData.DebuggerDisplay)));
        }
    }
}
