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
    using Ploeh.AutoFixture.Kernel;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using Xunit;
    using SutAlias = RgbLedSequencerLibrary.DotCorrectionData;

    public sealed class DotCorrectionDataTests
    {
        private static readonly Type SutType = typeof(SutAlias);

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
            var sut = fixture.Create<SutAlias>();

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
            var sut = fixture.Create<SutAlias>();
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

            assertion.Verify(SutType.GetProperty(nameof(SutAlias.DebuggerDisplay)));
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
            var sut = (IReadOnlyList<LedDotCorrection>)fixture.Create<SutAlias>();

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
            var sut = new SutAlias(sequencerConfigMock.Object, expected);

            Assert.Equal(expected, sut);
        }

        [Theory]
        [AutoMoqData]
        public void EqualsOverrideCorrectlyImplemented(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            EqualsOverrideNewObjectAssertion equalsNewObjectAssertion,
            EqualsOverrideNullAssertion equalsOverrideNullAssertion,
            EqualsOverrideOtherSuccessiveAssertion equalsOverrideOtherSuccessiveAssertion,
            EqualsOverrideSelfAssertion equalsOverrideSelfAssertion,
            Fixture fixture)
        {
            var equalsOverrideTheoriesSuccessiveAssertion =
                new EqualsOverrideTheoriesSuccessiveAssertion(
                    fixture,
                    CreateDifferingLedDotCorrectionsLengthTheory(),
                    CreateDifferingLedDotCorrectionsTheory(),
                    CreateEqualTheory());
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxDotCorrection = byte.MaxValue
            };
            fixture.Customize(customization);
            var equalsMethods = SutType.GetMethods()
                .Where(m => m.Name == nameof(SutAlias.Equals))
                .ToArray();

            equalsNewObjectAssertion.Verify(equalsMethods);
            equalsOverrideNullAssertion.Verify(equalsMethods);
            equalsOverrideOtherSuccessiveAssertion.Verify(equalsMethods);
            equalsOverrideSelfAssertion.Verify(equalsMethods);
            equalsOverrideTheoriesSuccessiveAssertion.Verify(equalsMethods);
        }

        [Theory]
        [AutoMoqData]
        public void GetHashCodeOverrideCorrectlyImplemented(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            GetHashCodeSuccessiveAssertion assertion,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxDotCorrection = byte.MaxValue
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetMethod(nameof(SutAlias.GetHashCode)));
        }

        internal static EqualsOverrideTheory CreateDifferingLedDotCorrectionsLengthTheory()
        {
            const int RgbLedCountLeft = 5;
            const int RgbLedCountRight = 3;
            var fixture = new Fixture();
            var sequencerConfigLeftMock = new Mock<IRgbLedSequencerConfiguration>();
            sequencerConfigLeftMock.Setup(c => c.RgbLedCount).Returns(RgbLedCountLeft);
            sequencerConfigLeftMock.Setup(c => c.MaxDotCorrection).Returns(byte.MaxValue);
            fixture.Inject(sequencerConfigLeftMock.Object);
            var left = new SutAlias(
                sequencerConfigLeftMock.Object,
                fixture.CreateMany<LedDotCorrection>(RgbLedCountLeft).ToArray());
            var sequencerConfigRightMock = new Mock<IRgbLedSequencerConfiguration>();
            sequencerConfigRightMock.Setup(c => c.RgbLedCount).Returns(RgbLedCountRight);
            sequencerConfigRightMock.Setup(c => c.MaxDotCorrection).Returns(byte.MaxValue);
            fixture.Inject(sequencerConfigRightMock.Object);
            var right = new SutAlias(
                sequencerConfigRightMock.Object,
                fixture.CreateMany<LedDotCorrection>(RgbLedCountRight).ToArray());
            return new EqualsOverrideTheory(left, right, false);
        }

        internal static EqualsOverrideTheory CreateDifferingLedDotCorrectionsTheory()
        {
            const int RgbLedCount = 5;
            var fixture = new Fixture();
            var sequencerConfigMock = new Mock<IRgbLedSequencerConfiguration>();
            sequencerConfigMock.Setup(c => c.RgbLedCount).Returns(RgbLedCount);
            sequencerConfigMock.Setup(c => c.MaxDotCorrection).Returns(byte.MaxValue);
            fixture.Inject(sequencerConfigMock.Object);
            var uniqueBytes =
                new Queue<byte>(new Generator<byte>(fixture).Distinct().Take(RgbLedCount * 3 * 2));
            fixture.Register(() => uniqueBytes.Dequeue());
            fixture.Customize<LedDotCorrection>(
                c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));
            var left = new SutAlias(
                sequencerConfigMock.Object,
                fixture.CreateMany<LedDotCorrection>(RgbLedCount).ToArray());
            var right = new SutAlias(
                sequencerConfigMock.Object,
                fixture.CreateMany<LedDotCorrection>(RgbLedCount).ToArray());
            return new EqualsOverrideTheory(left, right, false);
        }

        internal static EqualsOverrideTheory CreateEqualTheory()
        {
            const int RgbLedCount = 5;
            var fixture = new Fixture();
            var sequencerConfigMock = new Mock<IRgbLedSequencerConfiguration>();
            sequencerConfigMock.Setup(c => c.RgbLedCount).Returns(RgbLedCount);
            sequencerConfigMock.Setup(c => c.MaxDotCorrection).Returns(byte.MaxValue);
            fixture.Inject(sequencerConfigMock.Object);
            fixture.Inject(fixture.Create<byte>());
            fixture.Customize<LedDotCorrection>(
                c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));
            var left = new SutAlias(
                sequencerConfigMock.Object,
                fixture.CreateMany<LedDotCorrection>(RgbLedCount).ToArray());
            var right = new SutAlias(
                sequencerConfigMock.Object,
                fixture.CreateMany<LedDotCorrection>(RgbLedCount).ToArray());
            return new EqualsOverrideTheory(left, right, true);
        }
    }
}