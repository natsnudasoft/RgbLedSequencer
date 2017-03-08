// <copyright file="SequenceStep.cs" company="natsnudasoft">
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

namespace RgbLedSequencerLibrary
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Describes an individual step, and the delay time of the step, within an RGB LED Sequencer
    /// sequence.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class SequenceStep : INotifyPropertyChanged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IRgbLedSequencerConfiguration sequencerConfig;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int stepDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceStep"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="grayscaleData">The <see cref="RgbLedSequencerLibrary.GrayscaleData"/> that
        /// this step has.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="grayscaleData"/> is <see langword="null"/>.</exception>
        public SequenceStep(
            IRgbLedSequencerConfiguration sequencerConfig,
            GrayscaleData grayscaleData)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(grayscaleData, nameof(grayscaleData));

            this.sequencerConfig = sequencerConfig;
            this.GrayscaleData = grayscaleData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceStep"/> class, using the
        /// specified factory to create a <see cref="GrayscaleData"/> instance.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="grayscaleDataFactory">The factory to use to create instances of
        /// <see cref="RgbLedSequencerLibrary.GrayscaleData"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="grayscaleDataFactory"/> is <see langword="null"/>.</exception>
        public SequenceStep(
            IRgbLedSequencerConfiguration sequencerConfig,
            Func<GrayscaleData> grayscaleDataFactory)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(grayscaleDataFactory, nameof(grayscaleDataFactory));

            this.sequencerConfig = sequencerConfig;
#pragma warning disable CC0031 // Check for null before calling a delegate
            this.GrayscaleData = grayscaleDataFactory();
#pragma warning restore CC0031 // Check for null before calling a delegate
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the grayscale data for the RGB LED Sequencer at this step.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public GrayscaleData GrayscaleData { get; }

        /// <summary>
        /// Gets or sets the step delay (time to wait at the end of this step) value for this step.
        /// This value must be positive, can not be larger than the maximum value defined in the
        /// application config, and will be automatically clamped.
        /// </summary>
        public int StepDelay
        {
            get
            {
                return this.stepDelay;
            }

            set
            {
                value = Math.Max(0, Math.Min(value, this.sequencerConfig.MaxStepDelay));
                if (this.stepDelay != value)
                {
                    this.stepDelay = value;
                    this.OnPropertyChanged(nameof(this.StepDelay));
                }
            }
        }

        /// <summary>
        /// Gets the debugger display string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplay => string.Format(
            CultureInfo.CurrentCulture,
            "{{{0}: {1}, {2}: {3}}}",
            nameof(this.GrayscaleData),
            this.GrayscaleData.DebuggerDisplay,
            nameof(this.StepDelay),
            this.StepDelay);

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
