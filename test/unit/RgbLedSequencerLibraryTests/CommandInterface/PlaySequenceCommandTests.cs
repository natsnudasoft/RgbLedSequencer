// <copyright file="PlaySequenceCommandTests.cs" company="natsnudasoft">
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
    using Extension;
    using Moq;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using RgbLedSequencerLibrary.CommandInterface;
    using Xunit;

    public sealed class PlaySequenceCommandTests
    {
        private static readonly Type SutType = typeof(PlaySequenceCommand);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture)
        {
            const int SequenceCount = 10;
            const byte SequenceIndex = 5;
            sequencerConfigMock.Setup(s => s.SequenceCount).Returns(SequenceCount);
            ApplySequenceIndexSpecimen(fixture, SequenceIndex);
#pragma warning disable SA1118 // Parameter must not span multiple lines
            var behaviourExpectation = new CompositeBehaviorExpectation(
                new ParameterNullReferenceBehaviourExpectation(fixture),
                new ExceptionBehaviourExpectation<ArgumentOutOfRangeException>(
                    fixture,
                    "sequenceIndex",
                    (byte)10));
#pragma warning restore SA1118 // Parameter must not span multiple lines
            var assertion = new GuardClauseAssertion(fixture, behaviourExpectation);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            Fixture fixture,
            DoesNotThrowAssertion assertion)
        {
            const int SequenceCount = 10;
            const int SequenceIndex = 5;
            sequencerConfigMock.Setup(s => s.SequenceCount).Returns(SequenceCount);
            ApplySequenceIndexSpecimen(fixture, SequenceIndex);

            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public async Task ExecuteSendsCommandSequenceAsync(
            [Frozen]Mock<IRgbLedSequencerConfiguration> sequencerConfigMock,
            [Frozen]Mock<IRgbLedSequencer> rgbLedSequencerMock,
            Fixture fixture)
        {
            const byte SequenceIndex = 5;
            const int SequenceCount = 10;
            ApplySequenceIndexSpecimen(fixture, SequenceIndex);
            sequencerConfigMock.Setup(s => s.SequenceCount).Returns(SequenceCount);
            var sut = fixture.Create<PlaySequenceCommand>();

            await sut.ExecuteAsync().ConfigureAwait(false);

            rgbLedSequencerMock.Verify(s => s.HandshakeAsync(), Times.Once());
            rgbLedSequencerMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.PlaySequence),
                Times.Once());
            rgbLedSequencerMock.Verify(
                s => s.SendByteWhenReadyAsync(SequenceIndex),
                Times.Once());
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
