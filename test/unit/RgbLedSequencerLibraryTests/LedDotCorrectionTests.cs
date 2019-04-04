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
    using System.Linq;
    using AutoFixture;
    using AutoFixture.Idioms;
    using AutoFixture.Xunit2;
    using Moq;
    using Natsnudasoft.NatsnudaLibrary.TestExtensions;
    using Natsnudasoft.RgbLedSequencerLibrary;
    using Natsnudasoft.RgbLedSequencerLibraryTests.Helper;
    using Xunit;
    using SutAlias = Natsnudasoft.RgbLedSequencerLibrary.LedDotCorrection;

    public sealed class LedDotCorrectionTests
    {
        private static readonly Type SutType = typeof(SutAlias);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectDefaultMembers(
            byte maxDotCorrection,
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            sequencerConfigMock.Setup(c => c.MaxDotCorrection).Returns(maxDotCorrection);
            var sut = fixture.Build<SutAlias>().OmitAutoProperties().Create();

            Assert.Equal(maxDotCorrection, sut.Red);
            Assert.Equal(maxDotCorrection, sut.Green);
            Assert.Equal(maxDotCorrection, sut.Blue);
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture,
            ConstructorInitializedMemberAssertion assertion)
        {
            var customization = new LedDotCorrectionCustomization(sequencerConfigMock)
            {
                MaxDotCorrection = byte.MaxValue,
            };
            fixture.Customize(customization);

            assertion.Verify(
                SutType.GetProperty(nameof(SutAlias.Red)),
                SutType.GetProperty(nameof(SutAlias.Green)),
                SutType.GetProperty(nameof(SutAlias.Blue)));
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture,
            DoesNotThrowAssertion assertion)
        {
            var customization = new LedDotCorrectionCustomization(sequencerConfigMock)
            {
                MaxDotCorrection = byte.MaxValue,
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void DebuggerDisplayDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture,
            DoesNotThrowAssertion assertion)
        {
            var customization = new LedDotCorrectionCustomization(sequencerConfigMock)
            {
                MaxDotCorrection = byte.MaxValue,
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetProperty(nameof(SutAlias.DebuggerDisplay)));
        }

        [Theory]
        [AutoMoqData]
        public void EqualsOperatorOverloadCorrectlyImplemented(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new LedDotCorrectionCustomization(sequencerConfigMock)
            {
                MaxDotCorrection = byte.MaxValue,
            };
            fixture.Customize(customization);
            var red = fixture.Create<byte>();
            var green = fixture.Create<byte>();
            var blue = fixture.Create<byte>();
            var ledDotCorrection1 =
                new SutAlias(sequencerConfigMock.Object, red, green, blue);
            var ledDotCorrection2 =
                new SutAlias(sequencerConfigMock.Object, red, green, blue);

            var result = ledDotCorrection1 == ledDotCorrection2;

            Assert.True(result);
        }

        [Theory]
        [AutoMoqData]
        public void NotEqualsOperatorOverloadCorrectlyImplemented(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new LedDotCorrectionCustomization(sequencerConfigMock)
            {
                MaxDotCorrection = byte.MaxValue,
            };
            fixture.Customize(customization);
            var red = fixture.Create<byte>();
            var green = fixture.Create<byte>();
            var blue = fixture.Create<byte>();
            var red2 = unchecked((byte)(red + 1));
            var green2 = unchecked((byte)(green + 1));
            var blue2 = unchecked((byte)(blue + 1));
            var ledDotCorrection1 =
                new SutAlias(sequencerConfigMock.Object, red, green, blue);
            var ledDotCorrection2 =
                new SutAlias(sequencerConfigMock.Object, red2, green2, blue2);

            var result = ledDotCorrection1 != ledDotCorrection2;

            Assert.True(result);
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
            var customization = new LedDotCorrectionCustomization(sequencerConfigMock)
            {
                MaxDotCorrection = byte.MaxValue,
            };
            fixture.Customize(customization);
            var equalsMethods =
                SutType.GetMethods().Where(m => m.Name == nameof(SutAlias.Equals));

            equalsNewObjectAssertion.Verify(equalsMethods);
            equalsOverrideNullAssertion.Verify(equalsMethods);
            equalsOverrideOtherSuccessiveAssertion.Verify(equalsMethods);
            equalsOverrideSelfAssertion.Verify(equalsMethods);
        }

        [Theory]
        [AutoMoqData]
        public void GetHashCodeOverrideCorrectlyImplemented(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            GetHashCodeSuccessiveAssertion assertion,
            Fixture fixture)
        {
            var customization = new LedDotCorrectionCustomization(sequencerConfigMock)
            {
                MaxDotCorrection = byte.MaxValue,
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetMethod(nameof(SutAlias.GetHashCode)));
        }
    }
}