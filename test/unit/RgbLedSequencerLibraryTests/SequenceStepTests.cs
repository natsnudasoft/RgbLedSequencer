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

namespace Natsnudasoft.RgbLedSequencerLibraryTests
{
    using System;
    using System.Linq;
    using Helper;
    using Moq;
    using NatsnudaLibrary.TestExtensions;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using Xunit;
    using SutAlias = RgbLedSequencerLibrary.SequenceStep;

    public sealed class SequenceStepTests
    {
        private static readonly Type SutType = typeof(SutAlias);

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
                StepDelay = 500,
                MaxGrayscale = byte.MaxValue
            };
            fixture.Customize(customization);
            var behaviorExpectation = new ParameterNullReferenceBehaviorExpectation(fixture);
            var assertion = new GuardClauseAssertion(fixture, behaviorExpectation);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture,
            ConstructorInitializedMemberAssertion assertion)
        {
            var customization = new SequenceStepCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxGrayscale = byte.MaxValue,
                MaxStepDelay = int.MaxValue
            };
            fixture.Customize(customization);

            assertion.Verify(
                SutType.GetProperty(nameof(SutAlias.GrayscaleData)),
                SutType.GetProperty(nameof(SutAlias.StepDelay)));
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorWithFactorySetsCorrectInitializedMembers(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new GrayscaleDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxGrayscale = byte.MaxValue
            };
            fixture.Customize(customization);
            var grayscaleData = fixture.Create<GrayscaleData>();
            Func<GrayscaleData> grayscaleDataFactory = () => grayscaleData;
            var sut = new SutAlias(sequencerConfigMock.Object, grayscaleDataFactory, 0);

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
                StepDelay = 500,
                MaxGrayscale = byte.MaxValue
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetConstructors());
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
                StepDelay = 0,
                MaxGrayscale = byte.MaxValue
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetProperty(nameof(SutAlias.DebuggerDisplay)));
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
                    CreateDifferingGrayscaleDataTheory(),
                    CreateDifferingStepDelayTheory(),
                    CreateEqualTheory());
            var customization = new SequenceStepCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxGrayscale = byte.MaxValue,
                MaxStepDelay = int.MaxValue
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
            var customization = new SequenceStepCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxGrayscale = byte.MaxValue,
                MaxStepDelay = int.MaxValue
            };
            fixture.Customize(customization);

            assertion.Verify(SutType.GetMethod(nameof(SutAlias.GetHashCode)));
        }

        internal static EqualsOverrideTheory CreateDifferingGrayscaleDataTheory()
        {
            const int StepDelay = 100;
            var sequencerConfigMock = new Mock<IRgbLedSequencerConfiguration>();
            sequencerConfigMock.Setup(c => c.MaxStepDelay).Returns(int.MaxValue);
            var differingGrayscalesTheory =
                GrayscaleDataTests.CreateDifferingLedGrayscalesTheory();
            var left = new SutAlias(
                sequencerConfigMock.Object,
                differingGrayscalesTheory.Left as GrayscaleData,
                StepDelay);
            var right = new SutAlias(
                sequencerConfigMock.Object,
                differingGrayscalesTheory.Right as GrayscaleData,
                StepDelay);
            return new EqualsOverrideTheory(left, right, false);
        }

        internal static EqualsOverrideTheory CreateDifferingStepDelayTheory()
        {
            const int StepDelayLeft = 50;
            const int StepDelayRight = 100;
            var sequencerConfigMock = new Mock<IRgbLedSequencerConfiguration>();
            sequencerConfigMock.Setup(c => c.MaxStepDelay).Returns(int.MaxValue);
            var equalGrayscalesTheory = GrayscaleDataTests.CreateEqualTheory();
            var left = new SutAlias(
                sequencerConfigMock.Object,
                equalGrayscalesTheory.Left as GrayscaleData,
                StepDelayLeft);
            var right = new SutAlias(
                sequencerConfigMock.Object,
                equalGrayscalesTheory.Right as GrayscaleData,
                StepDelayRight);
            return new EqualsOverrideTheory(left, right, false);
        }

        internal static EqualsOverrideTheory CreateEqualTheory()
        {
            const int StepDelay = 100;
            var sequencerConfigMock = new Mock<IRgbLedSequencerConfiguration>();
            sequencerConfigMock.Setup(c => c.MaxStepDelay).Returns(int.MaxValue);
            var equalGrayscalesTheory = GrayscaleDataTests.CreateEqualTheory();
            var left = new SutAlias(
                sequencerConfigMock.Object,
                equalGrayscalesTheory.Left as GrayscaleData,
                StepDelay);
            var right = new SutAlias(
                sequencerConfigMock.Object,
                equalGrayscalesTheory.Right as GrayscaleData,
                StepDelay);
            return new EqualsOverrideTheory(left, right, true);
        }
    }
}