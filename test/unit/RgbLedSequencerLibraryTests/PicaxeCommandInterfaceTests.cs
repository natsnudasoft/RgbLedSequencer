// <copyright file="PicaxeCommandInterfaceTests.cs" company="natsnudasoft">
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
    using AutoFixture;
    using AutoFixture.Idioms;
    using AutoFixture.Xunit2;
    using Moq;
    using Natsnudasoft.NatsnudaLibrary.TestExtensions;
    using Natsnudasoft.RgbLedSequencerLibrary;
    using Xunit;
    using SutAlias = Natsnudasoft.RgbLedSequencerLibrary.PicaxeCommandInterface;

    public sealed class PicaxeCommandInterfaceTests
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
            assertion.Verify(SutType.GetProperty(nameof(SutAlias.SerialPortAdapter)));
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public async Task HandshakeSendsAndReceivesHandshakeSequenceAsync(Fixture fixture)
        {
            var serialPortAdapterMock = new Mock<ISerialPortAdapter>(MockBehavior.Strict);
            fixture.Inject(serialPortAdapterMock);
            var sequence = new MockSequence();
            serialPortAdapterMock.InSequence(sequence).SetupSet(s => s.BreakState = true);
            serialPortAdapterMock.InSequence(sequence).Setup(s => s.DiscardInBuffer());
            serialPortAdapterMock.InSequence(sequence).SetupSet(s => s.BreakState = false);
            serialPortAdapterMock.InSequence(sequence)
                .Setup(s => s.ReadByteAsync())
                .ReturnsAsync((byte)ReceiveInstruction.Handshake);
            serialPortAdapterMock.InSequence(sequence)
                .Setup(s => s.WriteByteAsync((byte)SendInstruction.Handshake))
                .Returns(Task.CompletedTask);
            var sut = fixture.Create<SutAlias>();

            await sut.HandshakeAsync().ConfigureAwait(false);

            serialPortAdapterMock
                .Verify(s => s.WriteByteAsync((byte)SendInstruction.Handshake), Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task SendByteWhenReadyChecksForReadyThenSendsByteAsync(
            byte value,
            Fixture fixture)
        {
            var serialPortAdapterMock = new Mock<ISerialPortAdapter>(MockBehavior.Strict);
            fixture.Inject(serialPortAdapterMock);
            var sequence = new MockSequence();
            serialPortAdapterMock.InSequence(sequence)
                .Setup(s => s.ReadByteAsync())
                .ReturnsAsync((byte)ReceiveInstruction.Ready);
            serialPortAdapterMock.InSequence(sequence)
                .Setup(s => s.WriteByteAsync(value))
                .Returns(Task.CompletedTask);
            var sut = fixture.Create<SutAlias>();

            await sut.SendByteWhenReadyAsync(value).ConfigureAwait(false);

            serialPortAdapterMock.Verify(s => s.WriteByteAsync(value), Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task SendWordWhenReadyChecksForReadyThenSendsWordAsync(
            ushort value,
            Fixture fixture)
        {
            var serialPortAdapterMock = new Mock<ISerialPortAdapter>(MockBehavior.Strict);
            fixture.Inject(serialPortAdapterMock);
            var sequence = new MockSequence();
            serialPortAdapterMock.InSequence(sequence)
                .Setup(s => s.ReadByteAsync())
                .ReturnsAsync((byte)ReceiveInstruction.Ready);
            serialPortAdapterMock.InSequence(sequence)
                .Setup(s => s.WriteByteAsync(unchecked((byte)value)))
                .Returns(Task.CompletedTask);
            serialPortAdapterMock.InSequence(sequence)
                .Setup(s => s.ReadByteAsync())
                .ReturnsAsync((byte)ReceiveInstruction.Ready);
            serialPortAdapterMock.InSequence(sequence)
                .Setup(s => s.WriteByteAsync(unchecked((byte)(value >> 8))))
                .Returns(Task.CompletedTask);
            var sut = fixture.Create<SutAlias>();

            await sut.SendWordWhenReadyAsync(value).ConfigureAwait(false);

            serialPortAdapterMock
                .Verify(s => s.WriteByteAsync(unchecked((byte)(value >> 8))), Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task SendInstructionWritesInstructionByteAsync(
            SendInstruction sendInstruction,
            Fixture fixture)
        {
            var serialPortAdapterMock = new Mock<ISerialPortAdapter>(MockBehavior.Strict);
            fixture.Inject(serialPortAdapterMock);
            var sequence = new MockSequence();
            serialPortAdapterMock.InSequence(sequence)
                .Setup(s => s.WriteByteAsync((byte)sendInstruction))
                .Returns(Task.CompletedTask);
            var sut = fixture.Create<SutAlias>();

            await sut.SendInstructionAsync(sendInstruction).ConfigureAwait(false);

            serialPortAdapterMock
                .Verify(s => s.WriteByteAsync((byte)sendInstruction), Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task UnexpectedInstructionReceivedThrowsAsync(
            [Frozen]Mock<ISerialPortAdapter> serialPortAdapterMock,
            Fixture fixture)
        {
            serialPortAdapterMock
                .Setup(s => s.ReadByteAsync())
                .ReturnsAsync((byte)ReceiveInstruction.Undefined);
            var sut = fixture.Create<SutAlias>();

            var ex = await Record.ExceptionAsync(() => sut.HandshakeAsync()).ConfigureAwait(false);

            Assert.IsType<UnexpectedInstructionException>(ex);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReadByteReadsCorrectByteAsync(
            byte expectedValue,
            [Frozen]Mock<ISerialPortAdapter> serialPortAdapterMock,
            Fixture fixture)
        {
            serialPortAdapterMock
                .Setup(s => s.ReadByteAsync())
                .ReturnsAsync(expectedValue);
            var sut = fixture.Create<SutAlias>();

            var actualValue = await sut.ReadByteAsync().ConfigureAwait(false);

            Assert.Equal(expectedValue, actualValue);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReadWordReadsCorrectWordAsync(
            ushort expectedValue,
            [Frozen]Mock<ISerialPortAdapter> serialPortAdapterMock,
            Fixture fixture)
        {
            serialPortAdapterMock
                .SetupSequence(s => s.ReadByteAsync())
                .ReturnsAsync((byte)expectedValue)
                .ReturnsAsync((byte)(expectedValue >> 8));
            var sut = fixture.Create<SutAlias>();

            var actualValue = await sut.ReadWordAsync().ConfigureAwait(false);

            Assert.Equal(expectedValue, actualValue);
        }
    }
}