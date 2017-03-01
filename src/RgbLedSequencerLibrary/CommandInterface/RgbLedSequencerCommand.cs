// <copyright file="RgbLedSequencerCommand.cs" company="natsnudasoft">
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

    /// <summary>
    /// Provides an abstract base class for encapsulating an <see cref="IAsyncCommand"/> that will
    /// be executed on an instance of <see cref="IRgbLedSequencer"/>.
    /// </summary>
    /// <seealso cref="IAsyncCommand" />
    public abstract class RgbLedSequencerCommand : IAsyncCommand
    {
        private readonly IProgress<CommandProgress> progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="RgbLedSequencerCommand"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="rgbLedSequencer">An <see cref="IRgbLedSequencer"/> used to communicate
        /// with the RGB LED Sequencer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="rgbLedSequencer"/> is <see langword="null"/>.</exception>
        protected RgbLedSequencerCommand(
            IRgbLedSequencerConfiguration sequencerConfig,
            IRgbLedSequencer rgbLedSequencer)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(rgbLedSequencer, nameof(rgbLedSequencer));

            this.SequencerConfig = sequencerConfig;
            this.RgbLedSequencer = rgbLedSequencer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RgbLedSequencerCommand"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="rgbLedSequencer">An <see cref="IRgbLedSequencer"/> used to communicate
        /// with the RGB LED Sequencer.</param>
        /// <param name="progress">The <see cref="IProgress{T}"/> provider that progress updates
        /// will be sent to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>,
        /// <paramref name="rgbLedSequencer"/>, or <paramref name="progress"/> is
        /// <see langword="null"/>.</exception>
        protected RgbLedSequencerCommand(
            IRgbLedSequencerConfiguration sequencerConfig,
            IRgbLedSequencer rgbLedSequencer,
            IProgress<CommandProgress> progress)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(rgbLedSequencer, nameof(rgbLedSequencer));
            ParameterValidation.IsNotNull(progress, nameof(progress));

            this.SequencerConfig = sequencerConfig;
            this.RgbLedSequencer = rgbLedSequencer;
            this.progress = progress;
        }

        /// <summary>
        /// Gets the <see cref="IRgbLedSequencerConfiguration"/> that describes the configuration of
        /// the RGB LED Sequencer.
        /// </summary>
        protected IRgbLedSequencerConfiguration SequencerConfig { get; }

        /// <summary>
        /// Gets the <see cref="IRgbLedSequencer"/> used to communicate with the RGB LED Sequencer.
        /// </summary>
        protected IRgbLedSequencer RgbLedSequencer { get; }

        /// <inheritdoc/>
        public abstract Task ExecuteAsync();

        /// <summary>
        /// Reports a progress update if there is a progress provider available.
        /// </summary>
        /// <param name="commandProgress">The <see cref="CommandProgress"/> that represents the
        /// current progress of the command.</param>
        protected void ReportProgress(CommandProgress commandProgress)
        {
            ParameterValidation.IsNotNull(commandProgress, nameof(commandProgress));

            if (this.progress != null)
            {
                this.progress.Report(commandProgress);
            }
        }
    }
}
