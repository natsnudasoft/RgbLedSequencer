// <copyright file="RgbLedSequencerTests.cs" company="natsnudasoft">
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
    using System.Threading.Tasks;
    using Helper;
    using Moq;
    using NatsnudaLibrary.TestExtensions;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using Xunit;
    using SutAlias = RgbLedSequencerLibrary.RgbLedSequencer;

    public sealed class RgbLedSequencerTests
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
        public void ConstructorSetsCorrectInitializedMembers(
            ConstructorInitializedMemberAssertion assertion)
        {
            assertion.Verify(
                SutType.GetProperty(nameof(SutAlias.SequencerConfig)),
                SutType.GetProperty(nameof(SutAlias.PicaxeCommandInterface)));
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public async Task ContinueSendsCommandSequenceAsync(
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            SutAlias sut)
        {
            await sut.ContinueAsync().ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.Continue),
                Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task SleepSendsCommandSequenceAsync(
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            SutAlias sut)
        {
            await sut.SleepAsync().ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.Sleep),
                Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task ReadDotCorrectionSendsCommandSequenceAndReturnsDotCorrectionAsync(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5
            };
            fixture.Customize(customization);
            var sut = fixture.Create<SutAlias>();

            var dotCorrectionData = await sut.ReadDotCorrectionAsync().ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.ReadDotCorrection),
                Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.ReadByteAsync(),
                Times.Exactly(customization.RgbLedCount * 3));
            for (int i = 0; i < customization.RgbLedCount; ++i)
            {
                Assert.IsType<LedDotCorrection>(dotCorrectionData[i]);
                Assert.NotNull(dotCorrectionData[i]);
            }
        }

        [Theory]
        [AutoMoqData]
        public void SetDotCorrectionHasCorrectGuardClauses(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5
            };
            fixture.Customize(customization);
            var assertion = new GuardClauseAssertion(
                fixture,
                new ParameterNullReferenceBehaviorExpectation(fixture));

            assertion.Verify(SutType.GetMethod(nameof(SutAlias.SetDotCorrectionAsync)));
        }

        [Theory]
        [AutoMoqData]
        public async Task SetDotCorrectionSendsCommandSequenceAsync(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            Fixture fixture)
        {
            var customization = new DotCorrectionDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5
            };
            fixture.Customize(customization);
            var dotCorrection = fixture.Create<DotCorrectionData>();
            var sut = fixture.Create<SutAlias>();

            await sut.SetDotCorrectionAsync(dotCorrection).ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.SetDotCorrection),
                Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendByteWhenReadyAsync(It.IsAny<byte>()),
                Times.Exactly(customization.RgbLedCount * 3));
        }

        [Theory]
        [AutoMoqData]
        public void PlaySequenceHasCorrectGuardClauses(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const int SequenceCount = 10;
            sequencerConfigMock.Setup(c => c.SequenceCount).Returns(SequenceCount);
#pragma warning disable SA1118 // Parameter must not span multiple lines
            var behaviorExpectation = new CompositeBehaviorExpectation(
                new ExceptionBehaviorExpectation<ArgumentOutOfRangeException>(
                    fixture,
                    "sequenceIndex",
                    (byte)(SequenceCount + 1)));
#pragma warning restore SA1118 // Parameter must not span multiple lines
            var assertion = new GuardClauseAssertion(fixture, behaviorExpectation);
            assertion.Verify(SutType.GetMethod(nameof(SutAlias.PlaySequenceAsync)));
        }

        [Theory]
        [AutoMoqData]
        public async Task PlaySequenceSendsCommandSequenceAsync(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            Fixture fixture)
        {
            const byte SequenceIndex = 5;
            const int SequenceCount = 10;
            sequencerConfigMock.Setup(s => s.SequenceCount).Returns(SequenceCount);
            var sut = fixture.Create<SutAlias>();

            await sut.PlaySequenceAsync(SequenceIndex).ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.PlaySequence),
                Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendByteWhenReadyAsync(SequenceIndex),
                Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task ReadSequenceSendsCommandSequenceAndReturnsSequenceAsync(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            Fixture fixture)
        {
            const byte SequenceIndex = 5;
            const int SequenceCount = 10;
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepCount = 8,
                StepCount = 5,
                MaxGrayscale = byte.MaxValue,
                MaxStepDelay = int.MaxValue
            };
            fixture.Customize(customization);
            sequencerConfigMock.Setup(s => s.SequenceCount).Returns(SequenceCount);
            picaxeCommandInterfaceMock
                .Setup(p => p.ReadWordAsync())
                .ReturnsAsync(customization.StepCount.Value);
            var sut = fixture.Create<SutAlias>();

            var sequenceData = await sut.ReadSequenceAsync(SequenceIndex).ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.ReadSequence),
                Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendByteWhenReadyAsync(SequenceIndex),
                Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.ReadWordAsync(),
                Times.Exactly(customization.StepCount.Value + 1));
            picaxeCommandInterfaceMock.Verify(
                s => s.ReadByteAsync(),
                Times.Exactly(customization.RgbLedCount * 3 * customization.StepCount.Value));
            for (int i = 0; i < customization.StepCount; ++i)
            {
                Assert.IsType<SequenceStep>(sequenceData[i]);
                Assert.NotNull(sequenceData[i]);
            }
        }

        [Theory]
        [AutoMoqData]
        public void SaveSequenceHasCorrectGuardClauses(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepCount = 8,
                StepCount = 5,
                MaxGrayscale = byte.MaxValue,
                MaxStepDelay = int.MaxValue
            };
            fixture.Customize(customization);
            const byte SequenceIndex = 5;
            const int SequenceCount = 10;
            sequencerConfigMock.Setup(s => s.SequenceCount).Returns(SequenceCount);
            ApplySequenceIndexSpecimen(fixture, SequenceIndex);
#pragma warning disable SA1118 // Parameter must not span multiple lines
            var behaviorExpectation = new CompositeBehaviorExpectation(
                new ParameterNullReferenceBehaviorExpectation(fixture),
                new ExceptionBehaviorExpectation<ArgumentOutOfRangeException>(
                    fixture,
                    "sequenceIndex",
                    (byte)(SequenceCount + 1)));
#pragma warning restore SA1118 // Parameter must not span multiple lines
            var assertion = new GuardClauseAssertion(fixture, behaviorExpectation);

            assertion.Verify(SutType.GetMethod(nameof(SutAlias.SaveSequenceAsync)));
        }

        [Theory]
        [AutoMoqData]
        public async Task SaveSequenceSendsCommandSequenceAsync(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            Fixture fixture)
        {
            var customization = new SequenceDataCustomization(sequencerConfigMock)
            {
                RgbLedCount = 5,
                MaxStepCount = 8,
                StepCount = 5,
                MaxGrayscale = byte.MaxValue,
                MaxStepDelay = int.MaxValue
            };
            fixture.Customize(customization);
            const byte SequenceIndex = 5;
            const int SequenceCount = 10;
            sequencerConfigMock.Setup(s => s.SequenceCount).Returns(SequenceCount);
            var sequence = fixture.Create<SequenceData>();
            var sut = fixture.Create<SutAlias>();

            await sut.SaveSequenceAsync(SequenceIndex, sequence).ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.SaveSequence),
                Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendByteWhenReadyAsync(It.IsAny<byte>()),
                Times.Exactly((customization.RgbLedCount * 3 * customization.StepCount.Value) + 1));
            picaxeCommandInterfaceMock.Verify(
                s => s.SendWordWhenReadyAsync(It.IsAny<int>()),
                Times.Exactly(customization.StepCount.Value + 1));
        }

        [Theory]
        [AutoMoqData]
        public async Task ClearSequencesSendsCommandSequenceAsync(
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            SutAlias sut)
        {
            await sut.ClearSequencesAsync().ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.ClearSequences),
                Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task CommandReportsProgressAsync(
            [Frozen]Mock<IProgress<CommandProgress>> progress,
            [Greedy]SutAlias sut)
        {
            await sut.ContinueAsync().ConfigureAwait(false);

            progress.Verify(p => p.Report(It.IsAny<CommandProgress>()), Times.AtLeastOnce());
        }

        private static void ApplySequenceIndexSpecimen(IFixture fixture, byte sequenceIndex)
        {
            fixture.Customizations.Add(new ParameterSpecimenBuilder(
                SutType,
                nameof(sequenceIndex),
                sequenceIndex));
        }
    }
}
