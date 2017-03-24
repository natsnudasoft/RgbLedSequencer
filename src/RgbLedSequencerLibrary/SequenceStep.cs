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

namespace Natsnudasoft.RgbLedSequencerLibrary
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using NatsnudaLibrary;

    /// <summary>
    /// Describes an individual step, and the delay time of the step, within an RGB LED Sequencer
    /// sequence.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class SequenceStep : IEquatable<SequenceStep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceStep"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="grayscaleData">The <see cref="RgbLedSequencerLibrary.GrayscaleData"/> that
        /// this step has.
        /// </param>
        /// <param name="stepDelay">The step delay (time to wait at the end of this step) value for
        /// this step. This value must be positive, and can not be larger than the maximum step
        /// delay defined in the specified configuration.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="grayscaleData"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stepDelay"/> is less than
        /// zero or larger than the maximum step delay defined in the specified configuration.
        /// </exception>
        public SequenceStep(
            IRgbLedSequencerConfiguration sequencerConfig,
            GrayscaleData grayscaleData,
            int stepDelay)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(grayscaleData, nameof(grayscaleData));
            ParameterValidation.IsBetweenInclusive(
                stepDelay,
                0,
                sequencerConfig.MaxStepDelay,
                nameof(stepDelay));

            this.GrayscaleData = grayscaleData;
            this.StepDelay = stepDelay;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceStep"/> class, using the
        /// specified factory to create a <see cref="GrayscaleData"/> instance.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="grayscaleDataFactory">The factory to use to create instances of
        /// <see cref="RgbLedSequencerLibrary.GrayscaleData"/>.</param>
        /// <param name="stepDelay">The step delay (time to wait at the end of this step) value for
        /// this step. This value must be positive, and can not be larger than the maximum step
        /// delay defined in the specified configuration.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="grayscaleDataFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stepDelay"/> is less than
        /// zero or larger than the maximum step delay defined in the specified configuration.
        /// </exception>
        public SequenceStep(
            IRgbLedSequencerConfiguration sequencerConfig,
            Func<GrayscaleData> grayscaleDataFactory,
            int stepDelay)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(grayscaleDataFactory, nameof(grayscaleDataFactory));
            ParameterValidation.IsBetweenInclusive(
                stepDelay,
                0,
                sequencerConfig.MaxStepDelay,
                nameof(stepDelay));

#pragma warning disable CC0031 // Check for null before calling a delegate
            this.GrayscaleData = grayscaleDataFactory();
#pragma warning restore CC0031 // Check for null before calling a delegate
            this.StepDelay = stepDelay;
        }

        /// <summary>
        /// Gets the grayscale data for the RGB LED Sequencer at this step.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public GrayscaleData GrayscaleData { get; }

        /// <summary>
        /// Gets the step delay (time to wait at the end of this step) value for this step.
        /// This value must be positive, can not be larger than the maximum value defined in the
        /// application config, and will be automatically clamped.
        /// </summary>
        public int StepDelay { get; }

        /// <summary>
        /// Gets the debugger display string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DebuggerDisplay => string.Format(
            CultureInfo.CurrentCulture,
            "{{{0}: {1}, {2}: {3}}}",
            nameof(this.GrayscaleData),
            this.GrayscaleData.DebuggerDisplay,
            nameof(this.StepDelay),
            this.StepDelay);

        /// <inheritdoc/>
        public bool Equals(SequenceStep other)
        {
            bool result;
            if (ReferenceEquals(other, null))
            {
                result = false;
            }
            else if (ReferenceEquals(other, this))
            {
                result = true;
            }
            else
            {
                var grayscaleDataEqual = ReferenceEquals(other.GrayscaleData, this.GrayscaleData) ||
                    (other.GrayscaleData?.Equals(this.GrayscaleData)).GetValueOrDefault();
                result = grayscaleDataEqual && other.StepDelay == this.StepDelay;
            }

            return result;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as SequenceStep);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            const int InitPrime = 17;
            const int MultPrime = 23;
            var hash = InitPrime;
            unchecked
            {
                hash = (hash * MultPrime)
                    + (this.GrayscaleData != null ? this.GrayscaleData.GetHashCode() : 0);
                hash = (hash * MultPrime) + this.StepDelay.GetHashCode();
            }

            return hash;
        }
    }
}
