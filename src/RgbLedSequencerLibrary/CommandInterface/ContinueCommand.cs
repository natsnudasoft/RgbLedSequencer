// <copyright file="ContinueCommand.cs" company="natsnudasoft">
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
    using System;
    using System.Threading.Tasks;
    using Properties;

    /// <summary>
    /// Encapsulates a command that instructs the RGB LED Sequencer to continue from a break (used
    /// to wake the device without performing a command).
    /// </summary>
    /// <seealso cref="RgbLedSequencerCommand" />
    public sealed class ContinueCommand : RgbLedSequencerCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContinueCommand"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="rgbLedSequencer">An <see cref="IRgbLedSequencer"/> used to communicate
        /// with the RGB LED Sequencer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="rgbLedSequencer"/> is <see langword="null"/>.</exception>
        public ContinueCommand(
            IRgbLedSequencerConfiguration sequencerConfig,
            IRgbLedSequencer rgbLedSequencer)
            : base(sequencerConfig, rgbLedSequencer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinueCommand"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="rgbLedSequencer">An <see cref="IRgbLedSequencer"/> used to communicate
        /// with the RGB LED Sequencer.</param>
        /// <param name="progress">The <see cref="IProgress{T}"/> provider that progress updates
        /// will be sent to, or <see langword="null"/> if no progress reporting is required.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>,
        /// <paramref name="rgbLedSequencer"/>, or <paramref name="progress"/> is
        /// <see langword="null"/>.</exception>
        public ContinueCommand(
            IRgbLedSequencerConfiguration sequencerConfig,
            IRgbLedSequencer rgbLedSequencer,
            IProgress<CommandProgress> progress)
            : base(sequencerConfig, rgbLedSequencer, progress)
        {
        }

        /// <inheritdoc/>
        public override async Task ExecuteAsync()
        {
#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.RgbLedSequencer.HandshakeAsync().ConfigureAwait(false);
            await this.RgbLedSequencer.SendInstructionAsync(SendInstruction.Continue)
                .ConfigureAwait(false);
            this.ReportProgress(new CommandProgress(100d, Resources.ContinueComplete));
#pragma warning restore MEN010 // Avoid magic numbers
        }
    }
}
