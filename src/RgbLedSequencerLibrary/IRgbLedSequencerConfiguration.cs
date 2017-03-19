// <copyright file="IRgbLedSequencerConfiguration.cs" company="natsnudasoft">
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
    /// Represents values that describe the configuration of an RGB LED Sequencer.
    /// </summary>
    public interface IRgbLedSequencerConfiguration
    {
        /// <summary>
        /// Gets the maximum dot correction value that the RGB LED Sequencer can accept.
        /// </summary>
        int MaxDotCorrection { get; }

        /// <summary>
        /// Gets the maximum grayscale value that the RGB LED Sequencer can accept.
        /// </summary>
        int MaxGrayscale { get; }

        /// <summary>
        /// Gets the maximum number of steps allowed in a sequence in the RGB LED Sequencer.
        /// </summary>
        int MaxStepCount { get; }

        /// <summary>
        /// Gets the maximum delay allowed between steps in a sequence in the RGB LED
        /// Sequencer.
        /// </summary>
        int MaxStepDelay { get; }

        /// <summary>
        /// Gets the number of RGB LEDs that are being controlled by the RGB LED Sequencer.
        /// </summary>
        int RgbLedCount { get; }

        /// <summary>
        /// Gets the number of sequences that can be stored in the RGB LED Sequencer.
        /// </summary>
        int SequenceCount { get; }

        /// <summary>
        /// Gets the configuration of the serial port used to communicate with the RGB LED
        /// Sequencer.
        /// </summary>
        ISerialPortConfiguration SerialPort { get; }
    }
}