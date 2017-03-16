// <copyright file="SendInstruction.cs" company="natsnudasoft">
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
    /// <summary>
    /// Defines the available instruction set that can be -sent- by the
    /// <see cref="RgbLedSequencerLibrary"/> to the physical RGB LED Sequencer.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1028:EnumStorageShouldBeInt32",
        Justification = "Consistent with RGB LED Sequencer.")]
    public enum SendInstruction : byte
    {
        /// <summary>
        /// Continue from the break state of the RGB LED Sequencer (used to wake the device from
        /// sleeping without performing a command).
        /// </summary>
        Continue = 0,

        /// <summary>
        /// Set dot correction on the RGB LED Sequencer.
        /// </summary>
        SetDotCorrection = 1,

        /// <summary>
        /// Play the sequence at a specified index on the RGB LED Sequencer.
        /// </summary>
        PlaySequence = 2,

        /// <summary>
        /// Save a specified sequence to the RGB LED Sequencer.
        /// </summary>
        SaveSequence = 3,

        /// <summary>
        /// Sets the RGB LED Sequencer to sleep (low power) mode.
        /// </summary>
        Sleep = 4,

        /// <summary>
        /// Send a handshake signal to the RGB LED Sequencer.
        /// </summary>
        Handshake = 5,

        /// <summary>
        /// Clear all sequences from the RGB LED Sequencer.
        /// </summary>
        ClearSequences = 6,

        /// <summary>
        /// Read the sequence currently stored at a specified index from the RGB LED Sequencer.
        /// </summary>
        ReadSequence = 7,

        /// <summary>
        /// Read the currently stored dot correction data from the RGB LED Sequencer.
        /// </summary>
        ReadDotCorrection = 8
    }
}
