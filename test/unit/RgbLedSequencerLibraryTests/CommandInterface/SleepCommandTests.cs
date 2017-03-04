// <copyright file="SleepCommandTests.cs" company="natsnudasoft">
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
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary.CommandInterface;
    using Xunit;

    public sealed class SleepCommandTests
    {
        private static readonly Type SutType = typeof(SleepCommand);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(
            GuardClauseAssertion assertion)
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
        public async Task ExecuteSendsCommandSequenceAsync(
            [Frozen]Mock<IRgbLedSequencer> rgbLedSequencerMock,
            SleepCommand sut)
        {
            await sut.ExecuteAsync().ConfigureAwait(false);

            rgbLedSequencerMock.Verify(s => s.HandshakeAsync(), Times.Once());
            rgbLedSequencerMock.Verify(
                s => s.SendInstructionAsync(SendInstruction.Sleep),
                Times.Once());
        }
    }
}
