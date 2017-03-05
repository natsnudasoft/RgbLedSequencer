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

namespace RgbLedSequencerLibraryTests.CommandInterface
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using RgbLedSequencerLibrary.CommandInterface;
    using RgbLedSequencerLibraryTests.Extension;
    using Xunit;

    public sealed class RgbLedSequencerTests
    {
        private static readonly Type SutType = typeof(RgbLedSequencer);

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
                SutType.GetProperty(nameof(RgbLedSequencer.SequencerConfig)),
                SutType.GetProperty(nameof(RgbLedSequencer.PicaxeCommandInterface)));
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
            RgbLedSequencer sut)
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
            RgbLedSequencer sut)
        {
            await sut.SleepAsync().ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.Sleep),
                Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public void SetDotCorrectionHasCorrectGuardClauses(Fixture fixture)
        {
            var assertion = new GuardClauseAssertion(
                fixture,
                new ParameterNullReferenceBehaviourExpectation(fixture));

            assertion.Verify(SutType.GetMethod(nameof(RgbLedSequencer.SetDotCorrectionAsync)));
        }

        [Theory]
        [AutoMoqData]
        public async Task SetDotCorrectionSendsCommandSequenceAsync(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            Fixture fixture)
        {
            const int RgbLedCount = 5;
            sequencerConfigMock.Setup(s => s.RgbLedCount).Returns(RgbLedCount);
            var dotCorrection = fixture.Create<DotCorrectionData>();
            var sut = fixture.Create<RgbLedSequencer>();

            await sut.SetDotCorrectionAsync(dotCorrection).ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.SetDotCorrection),
                Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendByteWhenReadyAsync(It.IsAny<byte>()),
                Times.Exactly(RgbLedCount * 3));
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
            var behaviourExpectation = new CompositeBehaviorExpectation(
                new ExceptionBehaviourExpectation<ArgumentOutOfRangeException>(
                    fixture,
                    "sequenceIndex",
                    (byte)(SequenceCount + 1)));
#pragma warning restore SA1118 // Parameter must not span multiple lines
            var assertion = new GuardClauseAssertion(fixture, behaviourExpectation);
            assertion.Verify(SutType.GetMethod(nameof(RgbLedSequencer.PlaySequenceAsync)));
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
            var sut = fixture.Create<RgbLedSequencer>();

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
        public void SaveSequenceHasCorrectGuardClauses(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const int RgbLedCount = 5;
            const byte SequenceIndex = 5;
            const int SequenceCount = 10;
            const int MaxStepCount = 8;
            const int StepCount = 5;
            sequencerConfigMock.Setup(s => s.RgbLedCount).Returns(RgbLedCount);
            sequencerConfigMock.Setup(s => s.SequenceCount).Returns(SequenceCount);
            sequencerConfigMock.Setup(s => s.MaxStepCount).Returns(MaxStepCount);
            ApplySequenceIndexSpecimen(fixture, SequenceIndex);
            ApplyStepCountSpecimen(fixture, StepCount);
#pragma warning disable SA1118 // Parameter must not span multiple lines
            var behaviourExpectation = new CompositeBehaviorExpectation(
                new ParameterNullReferenceBehaviourExpectation(fixture),
                new ExceptionBehaviourExpectation<ArgumentOutOfRangeException>(
                    fixture,
                    "sequenceIndex",
                    (byte)(SequenceCount + 1)));
#pragma warning restore SA1118 // Parameter must not span multiple lines
            var assertion = new GuardClauseAssertion(fixture, behaviourExpectation);
            assertion.Verify(SutType.GetMethod(nameof(RgbLedSequencer.SaveSequenceAsync)));
        }

        [Theory]
        [AutoMoqData]
        public async Task SaveSequenceSendsCommandSequenceAsync(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            Fixture fixture)
        {
            const int RgbLedCount = 5;
            const byte SequenceIndex = 5;
            const int SequenceCount = 10;
            const int MaxStepCount = 8;
            const int StepCount = 5;
            sequencerConfigMock.Setup(s => s.RgbLedCount).Returns(RgbLedCount);
            sequencerConfigMock.Setup(s => s.SequenceCount).Returns(SequenceCount);
            sequencerConfigMock.Setup(s => s.MaxStepCount).Returns(MaxStepCount);
            ApplyStepCountSpecimen(fixture, StepCount);
            var sequence = fixture.Create<SequenceData>();
            var sut = fixture.Create<RgbLedSequencer>();

            await sut.SaveSequenceAsync(SequenceIndex, sequence).ConfigureAwait(false);

            picaxeCommandInterfaceMock.Verify(s => s.HandshakeAsync(), Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.SaveSequence),
                Times.Once());
            picaxeCommandInterfaceMock.Verify(
                s => s.SendByteWhenReadyAsync(It.IsAny<byte>()),
                Times.Exactly((RgbLedCount * 3 * StepCount) + 1));
            picaxeCommandInterfaceMock.Verify(
                s => s.SendWordWhenReadyAsync(It.IsAny<int>()),
                Times.Exactly(StepCount + 1));
        }

        [Theory]
        [AutoMoqData]
        public async Task ClearSequencesSendsCommandSequenceAsync(
            [Frozen]Mock<IPicaxeCommandInterface> picaxeCommandInterfaceMock,
            RgbLedSequencer sut)
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
            [Greedy]RgbLedSequencer sut)
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

        private static void ApplyStepCountSpecimen(IFixture fixture, int stepCount)
        {
            fixture.Customizations.Add(new ParameterSpecimenBuilder(
                typeof(SequenceData),
                nameof(stepCount),
                stepCount));
        }
    }
}
