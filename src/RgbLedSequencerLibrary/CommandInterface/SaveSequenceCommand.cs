// <copyright file="SaveSequenceCommand.cs" company="natsnudasoft">
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
    /// Encapsulates a command that saves the specified <see cref="SequenceData"/> to the RGB LED
    /// Sequencer at the specified index.
    /// </summary>
    /// <seealso cref="RgbLedSequencerCommand" />
    public sealed class SaveSequenceCommand : RgbLedSequencerCommand
    {
        private readonly byte sequenceIndex;
        private readonly SequenceData sequence;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveSequenceCommand"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="rgbLedSequencer">An <see cref="IRgbLedSequencer"/> used to communicate
        /// with the RGB LED Sequencer.</param>
        /// <param name="sequenceIndex">The index to save the specified sequence to in the RGB LED
        /// Sequencer.</param>
        /// <param name="sequence">The <see cref="SequenceData"/> data to send to the RGB LED
        /// Sequencer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>,
        /// <paramref name="rgbLedSequencer"/>, or <paramref name="sequence"/> is
        /// <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sequenceIndex"/> is not a
        /// valid value.</exception>
        public SaveSequenceCommand(
            IRgbLedSequencerConfiguration sequencerConfig,
            IRgbLedSequencer rgbLedSequencer,
            byte sequenceIndex,
            SequenceData sequence)
            : base(sequencerConfig, rgbLedSequencer)
        {
            ParameterValidation.IsBetweenInclusive(
                sequenceIndex,
                0,
                this.SequencerConfig.SequenceCount - 1,
                nameof(sequenceIndex));
            ParameterValidation.IsNotNull(sequence, nameof(sequence));

            this.sequenceIndex = sequenceIndex;
            this.sequence = sequence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveSequenceCommand"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="rgbLedSequencer">An <see cref="IRgbLedSequencer"/> used to communicate
        /// with the RGB LED Sequencer.</param>
        /// <param name="progress">The <see cref="IProgress{T}"/> provider that progress updates
        /// will be sent to, or <see langword="null"/> if no progress reporting is required.</param>
        /// <param name="sequenceIndex">The index to save the specified sequence to in the RGB LED
        /// Sequencer.</param>
        /// <param name="sequence">The <see cref="SequenceData"/> data to send to the RGB LED
        /// Sequencer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>,
        /// <paramref name="rgbLedSequencer"/>, <paramref name="progress"/>, or
        /// <paramref name="sequence"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sequenceIndex"/> is not a
        /// valid value.</exception>
        public SaveSequenceCommand(
            IRgbLedSequencerConfiguration sequencerConfig,
            IRgbLedSequencer rgbLedSequencer,
            IProgress<CommandProgress> progress,
            byte sequenceIndex,
            SequenceData sequence)
            : base(sequencerConfig, rgbLedSequencer, progress)
        {
            ParameterValidation.IsBetweenInclusive(
                sequenceIndex,
                0,
                this.SequencerConfig.SequenceCount - 1,
                nameof(sequenceIndex));
            ParameterValidation.IsNotNull(sequence, nameof(sequence));

            this.sequenceIndex = sequenceIndex;
            this.sequence = sequence;
        }

        /// <inheritdoc/>
        public override async Task ExecuteAsync()
        {
#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.RgbLedSequencer.HandshakeAsync().ConfigureAwait(false);
            await this.RgbLedSequencer.SendInstructionAsync(SendInstruction.SaveSequence)
                .ConfigureAwait(false);
            await this.RgbLedSequencer.SendByteWhenReadyAsync(this.sequenceIndex)
                .ConfigureAwait(false);
            await this.RgbLedSequencer.SendWordWhenReadyAsync(this.sequence.StepCount)
                .ConfigureAwait(false);
            for (int stepIndex = 0; stepIndex < this.sequence.StepCount; ++stepIndex)
            {
                await this.SendSequenceStepAsync(stepIndex).ConfigureAwait(false);
            }

            this.ReportProgress(new CommandProgress(100d, Resources.SaveSequenceComplete));
#pragma warning restore MEN010 // Avoid magic numbers
        }

        private static double CalculateSequenceProgress(
            int rgbLedCount,
            int stepCount,
            int stepIndex,
            int ledIndex)
        {
#pragma warning disable MEN010 // Avoid magic numbers
            var sequenceProgress = ((stepIndex * rgbLedCount) + (rgbLedCount - 1 - ledIndex)) /
                (double)(rgbLedCount * stepCount);
            return ((100d - 2d) * sequenceProgress) + 2d;
#pragma warning restore MEN010 // Avoid magic numbers
        }

        private static string GetProgressMessage(int stepIndex, int ledIndex)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.SequenceDataProgress,
                stepIndex + 1,
                ledIndex + 1);
        }

        private async Task SendSequenceStepAsync(int stepIndex)
        {
            var rgbLedCount = this.SequencerConfig.RgbLedCount;
            var stepCount = this.sequence.StepCount;
            var sequenceStep = this.sequence[stepIndex];
            for (int ledIndex = rgbLedCount - 1; ledIndex >= 0; --ledIndex)
            {
                this.ReportProgress(new CommandProgress(
                    CalculateSequenceProgress(rgbLedCount, stepCount, stepIndex, ledIndex),
                    GetProgressMessage(stepIndex, ledIndex)));
                await this.SendLedGrayscaleAsync(sequenceStep.GrayscaleData[ledIndex])
                    .ConfigureAwait(false);
            }

            await this.RgbLedSequencer.SendWordWhenReadyAsync(sequenceStep.StepDelay)
                .ConfigureAwait(false);
        }

        private async Task SendLedGrayscaleAsync(LedGrayscale ledGrayscale)
        {
            await this.RgbLedSequencer.SendByteWhenReadyAsync(ledGrayscale.Blue)
                .ConfigureAwait(false);
            await this.RgbLedSequencer.SendByteWhenReadyAsync(ledGrayscale.Green)
                .ConfigureAwait(false);
            await this.RgbLedSequencer.SendByteWhenReadyAsync(ledGrayscale.Red)
                .ConfigureAwait(false);
        }
    }
}
