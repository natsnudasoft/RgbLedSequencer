// <copyright file="SequenceData.cs" company="natsnudasoft">
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
    using System.Diagnostics;
    using System.Globalization;
    using NatsnudaLibrary;

    /// <summary>
    /// Represents an RGB LED light sequence in an RGB LED Sequencer.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class SequenceData
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly SequenceStep[] sequenceSteps;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceData"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="sequenceSteps">The <see cref="SequenceStep"/> array that represents
        /// the steps in this sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="sequenceSteps"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The length of the <paramref name="sequenceSteps"/>
        /// array was greater than the maximum allowed step count.</exception>
        public SequenceData(
            IRgbLedSequencerConfiguration sequencerConfig,
            SequenceStep[] sequenceSteps)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(sequenceSteps, nameof(sequenceSteps));
            if (sequenceSteps.Length > sequencerConfig.MaxStepCount)
            {
                throw new ArgumentException(
                    "Array length must be less than or equal to " + sequencerConfig.MaxStepCount
                    + ".",
                    nameof(sequenceSteps));
            }

            this.sequenceSteps = sequenceSteps;
            this.StepCount = sequenceSteps.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceData"/> class, using the
        /// specified factory to create the number of <see cref="SequenceStep"/> instances
        /// specified by <paramref name="stepCount"/>.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="sequenceStepFactory">The factory to use to create instances of
        /// <see cref="SequenceStep"/>.</param>
        /// <param name="stepCount">The number of steps in the sequence represented by this
        /// <see cref="SequenceData"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="sequenceStepFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stepCount"/> was higher
        /// than the allowed maximum number of steps.</exception>
        public SequenceData(
            IRgbLedSequencerConfiguration sequencerConfig,
            Func<SequenceStep> sequenceStepFactory,
            int stepCount)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(sequenceStepFactory, nameof(sequenceStepFactory));
            ParameterValidation
                .IsBetweenInclusive(stepCount, 0, sequencerConfig.MaxStepCount, nameof(stepCount));

            this.sequenceSteps = new SequenceStep[stepCount];
            for (int i = 0; i < stepCount; ++i)
            {
#pragma warning disable CC0031 // Check for null before calling a delegate
                this.sequenceSteps[i] = sequenceStepFactory();
#pragma warning restore CC0031 // Check for null before calling a delegate
            }

            this.StepCount = stepCount;
        }

        /// <summary>
        /// Gets the number of steps in the sequence represented by this <see cref="SequenceData"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int StepCount { get; }

        /// <summary>
        /// Gets the debugger display string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplay => string.Format(
            CultureInfo.CurrentCulture,
            "{{{0}: {1}}}",
            nameof(this.StepCount),
            this.StepCount);

        /// <summary>
        /// Gets the <see cref="SequenceStep"/> data of the step at the specified index in this
        /// <see cref="SequenceData"/>.
        /// </summary>
        /// <param name="stepIndex">The 0 based index of the step to retrieve data for.</param>
        /// <returns>The <see cref="SequenceStep"/> data at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="stepIndex"/> is not a
        /// valid value.</exception>
        public SequenceStep this[int stepIndex]
        {
            get { return this.sequenceSteps[stepIndex]; }
        }
    }
}
