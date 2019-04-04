// <copyright file="RgbLedSequencer.cs" company="natsnudasoft">
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
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using NatsnudaLibrary;
    using Properties;

    /// <summary>
    /// Provides an implementation of a high level interface describing operations available on an
    /// RGB LED Sequencer.
    /// </summary>
    /// <seealso cref="IRgbLedSequencer" />
    public sealed class RgbLedSequencer : IRgbLedSequencer
    {
        private readonly IProgress<CommandProgress> progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="RgbLedSequencer"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="picaxeCommandInterface">An <see cref="IPicaxeCommandInterface"/> used to
        /// communicate with the RGB LED Sequencer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="picaxeCommandInterface"/> is <see langword="null"/>.</exception>
        public RgbLedSequencer(
            IRgbLedSequencerConfiguration sequencerConfig,
            IPicaxeCommandInterface picaxeCommandInterface)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(picaxeCommandInterface, nameof(picaxeCommandInterface));

            this.SequencerConfig = sequencerConfig;
            this.PicaxeCommandInterface = picaxeCommandInterface;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RgbLedSequencer"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="picaxeCommandInterface">An <see cref="IPicaxeCommandInterface"/> used to
        /// communicate with the RGB LED Sequencer.</param>
        /// <param name="progress">The <see cref="IProgress{T}"/> provider that progress updates
        /// will be sent to, or <see langword="null"/> if no progress reporting is required.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>,
        /// <paramref name="picaxeCommandInterface"/>, or <paramref name="progress"/> is
        /// <see langword="null"/>.</exception>
        public RgbLedSequencer(
            IRgbLedSequencerConfiguration sequencerConfig,
            IPicaxeCommandInterface picaxeCommandInterface,
            IProgress<CommandProgress> progress)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(picaxeCommandInterface, nameof(picaxeCommandInterface));
            ParameterValidation.IsNotNull(progress, nameof(progress));

            this.SequencerConfig = sequencerConfig;
            this.PicaxeCommandInterface = picaxeCommandInterface;
            this.progress = progress;
        }

        /// <summary>
        /// Gets the <see cref="IRgbLedSequencerConfiguration"/> that describes the configuration of
        /// the RGB LED Sequencer.
        /// </summary>
        public IRgbLedSequencerConfiguration SequencerConfig { get; }

        /// <summary>
        /// Gets the <see cref="IPicaxeCommandInterface"/> used to communicate with the RGB LED
        /// Sequencer.
        /// </summary>
        public IPicaxeCommandInterface PicaxeCommandInterface { get; }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open, or the
        /// underlying stream of the port is closed.</exception>
        /// <exception cref="IOException">The underlying port is in an invalid state, or an attempt
        /// to set the state of the underlying port failed.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task ContinueAsync()
        {
#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.PicaxeCommandInterface.HandshakeAsync().ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendInstructionAsync(SendInstruction.Continue)
                .ConfigureAwait(false);
            this.ReportProgress(new CommandProgress(100d, Resources.ContinueComplete));
#pragma warning restore MEN010 // Avoid magic numbers
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open, or the
        /// underlying stream of the port is closed.</exception>
        /// <exception cref="IOException">The underlying port is in an invalid state, or an attempt
        /// to set the state of the underlying port failed.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task SleepAsync()
        {
#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.PicaxeCommandInterface.HandshakeAsync().ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendInstructionAsync(SendInstruction.Sleep)
                .ConfigureAwait(false);
            this.ReportProgress(new CommandProgress(100d, Resources.SleepComplete));
#pragma warning restore MEN010 // Avoid magic numbers
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open, or the
        /// underlying stream of the port is closed.</exception>
        /// <exception cref="IOException">The underlying port is in an invalid state, or an attempt
        /// to set the state of the underlying port failed.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task<DotCorrectionData> ReadDotCorrectionAsync()
        {
#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.PicaxeCommandInterface.HandshakeAsync().ConfigureAwait(false);
            await this.PicaxeCommandInterface
                .SendInstructionAsync(SendInstruction.ReadDotCorrection)
                .ConfigureAwait(false);

            var rgbLedCount = this.SequencerConfig.RgbLedCount;
            var ledDotCorrections = new LedDotCorrection[rgbLedCount];
            for (int ledIndex = rgbLedCount - 1; ledIndex >= 0; --ledIndex)
            {
                this.ReportProgress(new CommandProgress(
                    CalculateDotCorrectionProgress(this.SequencerConfig.RgbLedCount, ledIndex),
                    GetReadDotCorrectionProgressMessage(ledIndex)));
                ledDotCorrections[ledIndex] = await this.ReadLedDotCorrectionAsync()
                    .ConfigureAwait(false);
            }

            this.ReportProgress(new CommandProgress(100d, Resources.ReadDotCorrectionComplete));
#pragma warning restore MEN010 // Avoid magic numbers

            return new DotCorrectionData(this.SequencerConfig, ledDotCorrections);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="dotCorrection"/> is not a
        /// valid value.</exception>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open, or the
        /// underlying stream of the port is closed.</exception>
        /// <exception cref="IOException">The underlying port is in an invalid state, or an attempt
        /// to set the state of the underlying port failed.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task SetDotCorrectionAsync(DotCorrectionData dotCorrection)
        {
            ParameterValidation.IsNotNull(dotCorrection, nameof(dotCorrection));

#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.PicaxeCommandInterface.HandshakeAsync().ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendInstructionAsync(SendInstruction.SetDotCorrection)
                .ConfigureAwait(false);

            // Dot correction data is sent to the device in "reverse" - from last LED to first.
            for (int ledIndex = this.SequencerConfig.RgbLedCount - 1; ledIndex >= 0; --ledIndex)
            {
                this.ReportProgress(new CommandProgress(
                    CalculateDotCorrectionProgress(this.SequencerConfig.RgbLedCount, ledIndex),
                    GetSetDotCorrectionProgressMessage(ledIndex)));
                await this.SendLedDotCorrectionAsync(dotCorrection[ledIndex])
                    .ConfigureAwait(false);
            }

            this.ReportProgress(new CommandProgress(100d, Resources.SetDotCorrectionComplete));
#pragma warning restore MEN010 // Avoid magic numbers
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sequenceIndex"/> is not a
        /// valid value.</exception>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open, or the
        /// underlying stream of the port is closed.</exception>
        /// <exception cref="IOException">The underlying port is in an invalid state, or an attempt
        /// to set the state of the underlying port failed.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task PlaySequenceAsync(byte sequenceIndex)
        {
            ParameterValidation.IsBetweenInclusive(
                sequenceIndex,
                0,
                Math.Max(0, this.SequencerConfig.SequenceCount - 1),
                nameof(sequenceIndex));

#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.PicaxeCommandInterface.HandshakeAsync().ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendInstructionAsync(SendInstruction.PlaySequence)
                .ConfigureAwait(false);
            this.ReportProgress(new CommandProgress(75d, Resources.PlaySequenceInProgress));
            await this.PicaxeCommandInterface.SendByteWhenReadyAsync(sequenceIndex)
                .ConfigureAwait(false);
            this.ReportProgress(new CommandProgress(100d, Resources.PlaySequenceComplete));
#pragma warning restore MEN010 // Avoid magic numbers
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sequenceIndex"/> is not a
        /// valid value.</exception>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open, or the
        /// underlying stream of the port is closed.</exception>
        /// <exception cref="IOException">The underlying port is in an invalid state, or an attempt
        /// to set the state of the underlying port failed.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task<SequenceData> ReadSequenceAsync(byte sequenceIndex)
        {
            ParameterValidation.IsBetweenInclusive(
                sequenceIndex,
                0,
                Math.Max(0, this.SequencerConfig.SequenceCount - 1),
                nameof(sequenceIndex));

#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.PicaxeCommandInterface.HandshakeAsync().ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendInstructionAsync(SendInstruction.ReadSequence)
                .ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendByteWhenReadyAsync(sequenceIndex)
                .ConfigureAwait(false);

            var stepCount = await this.PicaxeCommandInterface.ReadWordAsync().ConfigureAwait(false);
            var sequenceSteps = new SequenceStep[stepCount];
            for (int stepIndex = 0; stepIndex < stepCount; ++stepIndex)
            {
                sequenceSteps[stepIndex] = await this
                    .ReadSequenceStepAsync(stepCount, stepIndex)
                    .ConfigureAwait(false);
            }

            this.ReportProgress(new CommandProgress(100d, Resources.ReadSequenceComplete));
#pragma warning restore MEN010 // Avoid magic numbers

            return new SequenceData(this.SequencerConfig, sequenceSteps);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sequenceIndex"/> is not a
        /// valid value.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sequence"/> is
        /// <see langword="null"/>.</exception>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open, or the
        /// underlying stream of the port is closed.</exception>
        /// <exception cref="IOException">The underlying port is in an invalid state, or an attempt
        /// to set the state of the underlying port failed.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task SaveSequenceAsync(byte sequenceIndex, SequenceData sequence)
        {
            ParameterValidation.IsBetweenInclusive(
                sequenceIndex,
                0,
                Math.Max(0, this.SequencerConfig.SequenceCount - 1),
                nameof(sequenceIndex));
            ParameterValidation.IsNotNull(sequence, nameof(sequence));

#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.PicaxeCommandInterface.HandshakeAsync().ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendInstructionAsync(SendInstruction.SaveSequence)
                .ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendByteWhenReadyAsync(sequenceIndex)
                .ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendWordWhenReadyAsync(sequence.StepCount)
                .ConfigureAwait(false);
            for (int stepIndex = 0; stepIndex < sequence.StepCount; ++stepIndex)
            {
                await this.SendSequenceStepAsync(sequence, stepIndex).ConfigureAwait(false);
            }

            this.ReportProgress(new CommandProgress(100d, Resources.SaveSequenceComplete));
#pragma warning restore MEN010 // Avoid magic numbers
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">A read or write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The underlying port is not open, or the
        /// underlying stream of the port is closed.</exception>
        /// <exception cref="IOException">The underlying port is in an invalid state, or an attempt
        /// to set the state of the underlying port failed.</exception>
        /// <exception cref="UnexpectedInstructionException">An unexpected instruction was received
        /// from the RGB LED Sequencer.</exception>
        public async Task ClearSequencesAsync()
        {
#pragma warning disable MEN010 // Avoid magic numbers
            this.ReportProgress(new CommandProgress(0d, Resources.HandshakeProgress));
            await this.PicaxeCommandInterface.HandshakeAsync().ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendInstructionAsync(SendInstruction.ClearSequences)
                .ConfigureAwait(false);
            this.ReportProgress(new CommandProgress(100d, Resources.ClearSequencesComplete));
#pragma warning restore MEN010 // Avoid magic numbers
        }

        private static double CalculateDotCorrectionProgress(
            int rgbLedCount,
            int ledIndex)
        {
            const double ProgressStart = 2d;
            const double ProgressEnd = 100d;
            var dotCorrectionProgress = (rgbLedCount - 1 - ledIndex) /
                (double)rgbLedCount;
            return ((ProgressEnd - ProgressStart) * dotCorrectionProgress) + ProgressStart;
        }

        private static double CalculateSequenceProgress(
            int rgbLedCount,
            int stepCount,
            int stepIndex,
            int ledIndex)
        {
            const double ProgressStart = 2d;
            const double ProgressEnd = 100d;
            var sequenceProgress = ((stepIndex * rgbLedCount) + (rgbLedCount - 1 - ledIndex)) /
                (double)(rgbLedCount * stepCount);
            return ((ProgressEnd - ProgressStart) * sequenceProgress) + ProgressStart;
        }

        private static string GetReadDotCorrectionProgressMessage(int ledIndex)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.ReadDotCorrectionDataProgress,
                ledIndex + 1);
        }

        private static string GetSetDotCorrectionProgressMessage(int ledIndex)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.SendDotCorrectionDataProgress,
                ledIndex + 1);
        }

        private static string GetReadSequenceProgressMessage(int stepIndex, int ledIndex)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.ReadSequenceDataProgress,
                stepIndex + 1,
                ledIndex + 1);
        }

        private static string GetSendSequenceProgressMessage(int stepIndex, int ledIndex)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.SendSequenceDataProgress,
                stepIndex + 1,
                ledIndex + 1);
        }

        private async Task<SequenceStep> ReadSequenceStepAsync(int stepCount, int stepIndex)
        {
            var rgbLedCount = this.SequencerConfig.RgbLedCount;
            var ledGrayscales = new LedGrayscale[rgbLedCount];
            for (int ledIndex = rgbLedCount - 1; ledIndex >= 0; --ledIndex)
            {
                this.ReportProgress(new CommandProgress(
                    CalculateSequenceProgress(rgbLedCount, stepCount, stepIndex, ledIndex),
                    GetReadSequenceProgressMessage(stepIndex, ledIndex)));
                ledGrayscales[ledIndex] = await this.ReadLedGrayscaleAsync().ConfigureAwait(false);
            }

            var stepDelay = await this.PicaxeCommandInterface.ReadWordAsync().ConfigureAwait(false);
            var grayscaleData = new GrayscaleData(this.SequencerConfig, ledGrayscales);
            return new SequenceStep(this.SequencerConfig, grayscaleData, stepDelay);
        }

        private async Task SendSequenceStepAsync(SequenceData sequence, int stepIndex)
        {
            var rgbLedCount = this.SequencerConfig.RgbLedCount;
            var stepCount = sequence.StepCount;
            var sequenceStep = sequence[stepIndex];
            for (int ledIndex = rgbLedCount - 1; ledIndex >= 0; --ledIndex)
            {
                this.ReportProgress(new CommandProgress(
                    CalculateSequenceProgress(rgbLedCount, stepCount, stepIndex, ledIndex),
                    GetSendSequenceProgressMessage(stepIndex, ledIndex)));
                await this.SendLedGrayscaleAsync(sequenceStep.GrayscaleData[ledIndex])
                    .ConfigureAwait(false);
            }

            await this.PicaxeCommandInterface.SendWordWhenReadyAsync(sequenceStep.StepDelay)
                .ConfigureAwait(false);
        }

        private async Task<LedDotCorrection> ReadLedDotCorrectionAsync()
        {
            var blue = await this.PicaxeCommandInterface.ReadByteAsync().ConfigureAwait(false);
            var green = await this.PicaxeCommandInterface.ReadByteAsync().ConfigureAwait(false);
            var red = await this.PicaxeCommandInterface.ReadByteAsync().ConfigureAwait(false);
            return new LedDotCorrection(this.SequencerConfig, red, green, blue);
        }

        private async Task<LedGrayscale> ReadLedGrayscaleAsync()
        {
            var blue = await this.PicaxeCommandInterface.ReadByteAsync().ConfigureAwait(false);
            var green = await this.PicaxeCommandInterface.ReadByteAsync().ConfigureAwait(false);
            var red = await this.PicaxeCommandInterface.ReadByteAsync().ConfigureAwait(false);
            return new LedGrayscale(this.SequencerConfig, red, green, blue);
        }

        private async Task SendLedDotCorrectionAsync(LedDotCorrection ledDotCorrection)
        {
            await this.PicaxeCommandInterface.SendByteWhenReadyAsync(ledDotCorrection.Blue)
                .ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendByteWhenReadyAsync(ledDotCorrection.Green)
                .ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendByteWhenReadyAsync(ledDotCorrection.Red)
                .ConfigureAwait(false);
        }

        private async Task SendLedGrayscaleAsync(LedGrayscale ledGrayscale)
        {
            await this.PicaxeCommandInterface.SendByteWhenReadyAsync(ledGrayscale.Blue)
                .ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendByteWhenReadyAsync(ledGrayscale.Green)
                .ConfigureAwait(false);
            await this.PicaxeCommandInterface.SendByteWhenReadyAsync(ledGrayscale.Red)
                .ConfigureAwait(false);
        }

        private void ReportProgress(CommandProgress commandProgress)
        {
            ParameterValidation.IsNotNull(commandProgress, nameof(commandProgress));

            if (this.progress != null)
            {
                this.progress.Report(commandProgress);
            }
        }
    }
}