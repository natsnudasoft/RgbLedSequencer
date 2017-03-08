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
    /// Provides a high level interface describing operations available on an RGB LED Sequencer.
    /// </summary>
    public interface IRgbLedSequencer
    {
        /// <summary>
        /// Instructs the RGB LED Sequencer to continue from a break (used to wake the device
        /// without performing a command).
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task ContinueAsync();

        /// <summary>
        /// Instructs the RGB LED Sequencer to go into sleep mode.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SleepAsync();

        /// <summary>
        /// Reads the dot correction data currently stored in the RGB LED Sequencer.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation. The
        /// result of the <see cref="Task{TResult}"/> contains the <see cref="DotCorrectionData"/>
        /// that was read from the RGB LED Sequencer.</returns>
        Task<DotCorrectionData> ReadDotCorrectionAsync();

        /// <summary>
        /// Sends the specified <see cref="DotCorrectionData"/> to the RGB LED Sequencer.
        /// </summary>
        /// <param name="dotCorrection">The <see cref="DotCorrectionData"/> data to send to the RGB
        /// LED Sequencer.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SetDotCorrectionAsync(DotCorrectionData dotCorrection);

        /// <summary>
        /// Instructs the RGB LED Sequencer to begin playing the sequence at the specified index.
        /// </summary>
        /// <param name="sequenceIndex">The index of the sequence to begin playing on the RGB LED
        /// Sequencer.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PlaySequenceAsync(byte sequenceIndex);

        /// <summary>
        /// Reads the sequence at the specified index from the RGB LED Sequencer.
        /// </summary>
        /// <param name="sequenceIndex">The index of the sequence to begin playing on the RGB LED
        /// Sequencer.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation. The
        /// result of the <see cref="Task{TResult}"/> contains the <see cref="SequenceData"/> that
        /// was read from the RGB LED Sequencer.</returns>
        Task<SequenceData> ReadSequenceAsync(byte sequenceIndex);

        /// <summary>
        /// Saves the specified <see cref="SequenceData"/> to the RGB LED Sequencer at the specified
        /// index.
        /// </summary>
        /// <param name="sequenceIndex">The index to save the specified sequence to in the RGB LED
        /// Sequencer.</param>
        /// <param name="sequence">The <see cref="SequenceData"/> data to send to the RGB LED
        /// Sequencer.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SaveSequenceAsync(byte sequenceIndex, SequenceData sequence);

        /// <summary>
        /// Instructs the RGB LED Sequencer to clear all sequence data.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task ClearSequencesAsync();
    }
}