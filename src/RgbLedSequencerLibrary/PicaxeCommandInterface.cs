// <copyright file="PicaxeCommandInterface.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerLibrary
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Natsnudasoft.NatsnudaLibrary;

    /// <summary>
    /// Provides an implementation of a low level communications interface to a PICAXE controlling
    /// an RGB LED Sequencer.
    /// </summary>
    /// <seealso cref="IPicaxeCommandInterface" />
    public sealed class PicaxeCommandInterface : IPicaxeCommandInterface
    {
        private const int BreakStateTime = 500;
        private const int HandshakeTime = 50;

        /// <summary>
        /// Initializes a new instance of the <see cref="PicaxeCommandInterface"/> class.
        /// </summary>
        /// <param name="serialPortAdapter">The <see cref="ISerialPortAdapter"/> to use to
        /// communicate with the RGB LED Sequencer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serialPortAdapter"/> is
        /// <see langword="null"/>.</exception>
        public PicaxeCommandInterface(ISerialPortAdapter serialPortAdapter)
        {
            ParameterValidation.IsNotNull(serialPortAdapter, nameof(serialPortAdapter));

            this.SerialPortAdapter = serialPortAdapter;
        }

        /// <summary>
        /// Gets the serial port adapter used to communicate with the RGB LED Sequencer.
        /// </summary>
        public ISerialPortAdapter SerialPortAdapter { get; }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open, or the
        /// underlying stream of the port is closed.</exception>
        /// <exception cref="IOException">The underlying port is in an invalid state, or an attempt
        /// to set the state of the underlying port failed.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task HandshakeAsync()
        {
            this.SerialPortAdapter.BreakState = true;
            await Task.Delay(BreakStateTime).ConfigureAwait(false);
            this.SerialPortAdapter.DiscardInBuffer();
            this.SerialPortAdapter.BreakState = false;
            await this.CheckReceivedInstructionAsync(ReceiveInstruction.Handshake)
                .ConfigureAwait(false);
            await this.SendInstructionAsync(SendInstruction.Handshake).ConfigureAwait(false);
            await Task.Delay(HandshakeTime).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task SendByteWhenReadyAsync(byte value)
        {
            await this.CheckReceivedInstructionAsync(ReceiveInstruction.Ready)
                .ConfigureAwait(false);
            await this.SerialPortAdapter.WriteByteAsync(value).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task SendWordWhenReadyAsync(int value)
        {
            const int ByteSize = 8;
            var valueBytes = new byte[2];

            // Get bytes in little endian order
            valueBytes[0] = unchecked((byte)value);
            valueBytes[1] = unchecked((byte)(value >> ByteSize));

            await this.SendByteWhenReadyAsync(valueBytes[0]).ConfigureAwait(false);
            await this.SendByteWhenReadyAsync(valueBytes[1]).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open.</exception>
        public Task SendInstructionAsync(SendInstruction instruction)
        {
            return this.SerialPortAdapter.WriteByteAsync((byte)instruction);
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open.</exception>
        public Task<byte> ReadByteAsync()
        {
            return this.SerialPortAdapter.ReadByteAsync();
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open.</exception>
        public async Task<int> ReadWordAsync()
        {
            const int ByteSize = 8;
            int word = await this.SerialPortAdapter.ReadByteAsync().ConfigureAwait(false);
            word |= await this.SerialPortAdapter.ReadByteAsync().ConfigureAwait(false)
                << ByteSize;
            return word;
        }

        private async Task CheckReceivedInstructionAsync(ReceiveInstruction expectedInstruction)
        {
            var receivedInstruction = (ReceiveInstruction)await this.SerialPortAdapter
                .ReadByteAsync()
                .ConfigureAwait(false);
            if (receivedInstruction != expectedInstruction)
            {
                throw new UnexpectedInstructionException(expectedInstruction, receivedInstruction);
            }
        }
    }
}