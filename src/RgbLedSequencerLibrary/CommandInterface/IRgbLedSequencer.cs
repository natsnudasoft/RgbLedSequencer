// <copyright file="IRgbLedSequencer.cs" company="natsnudasoft">
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

namespace RgbLedSequencerLibrary.CommandInterface
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface describing operations for communicating with an RGB LED Sequencer.
    /// </summary>
    public interface IRgbLedSequencer
    {
        /// <summary>
        /// Gets the serial port adapter used to communicate with the RGB LED Sequencer.
        /// </summary>
        ISerialPortAdapter SerialPortAdapter { get; }

        /// <summary>
        /// Attempts to perform communication with the RGB LED Sequencer, waiting for a handshake
        /// signal.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task HandshakeAsync();

        /// <summary>
        /// Sends a byte to the RGB LED Sequencer when a ready signal has been received.
        /// </summary>
        /// <param name="value">The value to send to the RGB LED Sequencer.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SendByteWhenReadyAsync(byte value);

        /// <summary>
        /// Sends a word (16 bit unsigned value) to the RGB LED Sequencer when a ready signal has
        /// been received.
        /// </summary>
        /// <param name="value">The value to send to the RGB LED Sequencer, only the lower 16 bits
        /// are used.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SendWordWhenReadyAsync(int value);

        /// <summary>
        /// Sends the specified instruction to the RGB LED Sequencer as a byte value.
        /// </summary>
        /// <param name="instruction">The <see cref="SendInstruction"/> to send to the RGB LED
        /// Sequencer.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SendInstructionAsync(SendInstruction instruction);
    }
}