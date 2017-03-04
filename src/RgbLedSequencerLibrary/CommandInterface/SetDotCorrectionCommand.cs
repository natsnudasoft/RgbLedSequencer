// <copyright file="SetDotCorrectionCommand.cs" company="natsnudasoft">
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
    using System.Globalization;
    using System.Threading.Tasks;
    using Properties;

    /// <summary>
    /// Encapsulates a command that sends the specified <see cref="DotCorrectionData"/> to the RGB
    /// LED Sequencer.
    /// </summary>
    /// <seealso cref="RgbLedSequencerCommand" />
    public sealed class SetDotCorrectionCommand : RgbLedSequencerCommand
    {
        private readonly DotCorrectionData dotCorrection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetDotCorrectionCommand"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="rgbLedSequencer">An <see cref="IRgbLedSequencer"/> used to communicate
        /// with the RGB LED Sequencer.</param>
        /// <param name="dotCorrection">The <see cref="DotCorrectionData"/> data to send to the RGB
        /// LED Sequencer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>,
        /// <paramref name="rgbLedSequencer"/>, or
        /// <paramref name="dotCorrection"/> is <see langword="null"/>.</exception>
        public SetDotCorrectionCommand(
            IRgbLedSequencerConfiguration sequencerConfig,
            IRgbLedSequencer rgbLedSequencer,
            DotCorrectionData dotCorrection)
            : base(sequencerConfig, rgbLedSequencer)
        {
            ParameterValidation.IsNotNull(dotCorrection, nameof(dotCorrection));

            this.dotCorrection = dotCorrection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetDotCorrectionCommand"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="rgbLedSequencer">An <see cref="IRgbLedSequencer"/> used to communicate
        /// with the RGB LED Sequencer.</param>
        /// <param name="progress">The <see cref="IProgress{T}"/> provider that progress updates
        /// will be sent to, or <see langword="null"/> if no progress reporting is required.</param>
        /// <param name="dotCorrection">The <see cref="DotCorrectionData"/> data to send to the RGB
        /// LED Sequencer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>,
        /// <paramref name="rgbLedSequencer"/>, <paramref name="progress"/>, or
        /// <paramref name="dotCorrection"/> is <see langword="null"/>.</exception>
        public SetDotCorrectionCommand(
            IRgbLedSequencerConfiguration sequencerConfig,
            IRgbLedSequencer rgbLedSequencer,
            IProgress<CommandProgress> progress,
            DotCorrectionData dotCorrection)
            : base(sequencerConfig, rgbLedSequencer, progress)
        {
            ParameterValidation.IsNotNull(dotCorrection, nameof(dotCorrection));

            this.dotCorrection = dotCorrection;
        }

        /// <inheritdoc/>
        public override async Task ExecuteAsync()
        {
#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.RgbLedSequencer.HandshakeAsync().ConfigureAwait(false);
            await this.RgbLedSequencer.SendInstructionAsync(SendInstruction.SetDotCorrection)
                .ConfigureAwait(false);

            // Dot correction data is sent to the device in "reverse" - from last LED to first.
            for (int ledIndex = this.SequencerConfig.RgbLedCount - 1; ledIndex >= 0; --ledIndex)
            {
                this.ReportProgress(new CommandProgress(
                    CalculateDotCorrectionProgress(this.SequencerConfig.RgbLedCount, ledIndex),
                    GetProgressMessage(ledIndex)));
                await this.SendLedDotCorrectionAsync(this.dotCorrection[ledIndex])
                    .ConfigureAwait(false);
            }

            this.ReportProgress(new CommandProgress(100d, Resources.SetDotCorrectionComplete));
#pragma warning restore MEN010 // Avoid magic numbers
        }

        private static double CalculateDotCorrectionProgress(
            int rgbLedCount,
            int ledIndex)
        {
#pragma warning disable MEN010 // Avoid magic numbers
            var dotCorrectionProgress = (rgbLedCount - 1 - ledIndex) /
                (double)rgbLedCount;
            return ((100d - 2d) * dotCorrectionProgress) + 2d;
#pragma warning restore MEN010 // Avoid magic numbers
        }

        private static string GetProgressMessage(int ledIndex)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.DotCorrectionDataProgress,
                ledIndex + 1);
        }

        private async Task SendLedDotCorrectionAsync(LedDotCorrection ledDotCorrection)
        {
            await this.RgbLedSequencer.SendByteWhenReadyAsync(ledDotCorrection.Blue)
                .ConfigureAwait(false);
            await this.RgbLedSequencer.SendByteWhenReadyAsync(ledDotCorrection.Green)
                .ConfigureAwait(false);
            await this.RgbLedSequencer.SendByteWhenReadyAsync(ledDotCorrection.Red)
                .ConfigureAwait(false);
        }
    }
}
