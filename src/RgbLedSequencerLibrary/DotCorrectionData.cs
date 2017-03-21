// <copyright file="DotCorrectionData.cs" company="natsnudasoft">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using NatsnudaLibrary;

    /// <summary>
    /// Represents dot correction (brightness difference compensation) values for a number of RGB
    /// LEDs part of an RGB LED Sequencer.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "We don't follow this convention.")]
    public sealed class DotCorrectionData :
        IReadOnlyList<LedDotCorrection>,
        IEnumerable<LedDotCorrection>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly LedDotCorrection[] ledDotCorrections;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotCorrectionData"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="ledDotCorrections">The <see cref="LedDotCorrection"/> collection that
        /// represents the dot correction value of a number of RGB LEDs.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="ledDotCorrections"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The length of the
        /// <paramref name="ledDotCorrections"/> array does not match the number of RGB LEDs.
        /// </exception>
        public DotCorrectionData(
            IRgbLedSequencerConfiguration sequencerConfig,
            ICollection<LedDotCorrection> ledDotCorrections)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(ledDotCorrections, nameof(ledDotCorrections));
            if (ledDotCorrections.Count != sequencerConfig.RgbLedCount)
            {
                throw new ArgumentException(
                    "Collection size must match the number of RGB LEDs.",
                    nameof(ledDotCorrections));
            }

            this.ledDotCorrections = new LedDotCorrection[ledDotCorrections.Count];
            ledDotCorrections.CopyTo(this.ledDotCorrections, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotCorrectionData"/> class, using the
        /// specified factory to create the number of <see cref="LedDotCorrection"/> instances
        /// defined in the specified <see cref="IRgbLedSequencerConfiguration"/>.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="ledDotCorrectionFactory">The factory to use to create instances of
        /// <see cref="LedDotCorrection"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="ledDotCorrectionFactory"/> is <see langword="null"/>.</exception>
        public DotCorrectionData(
            IRgbLedSequencerConfiguration sequencerConfig,
            Func<LedDotCorrection> ledDotCorrectionFactory)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(ledDotCorrectionFactory, nameof(ledDotCorrectionFactory));

            var rgbLedCount = sequencerConfig.RgbLedCount;
            this.ledDotCorrections = new LedDotCorrection[rgbLedCount];
            for (int i = 0; i < rgbLedCount; ++i)
            {
#pragma warning disable CC0031 // Check for null before calling a delegate
                this.ledDotCorrections[i] = ledDotCorrectionFactory();
#pragma warning restore CC0031 // Check for null before calling a delegate
            }
        }

        /// <inheritdoc/>
        int IReadOnlyCollection<LedDotCorrection>.Count
        {
            get { return this.ledDotCorrections.Length; }
        }

        /// <summary>
        /// Gets the debugger display string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DebuggerDisplay => "{" +
            string.Join(" ", this.ledDotCorrections.Select(l => l.DebuggerDisplay)) + "}";

        /// <summary>
        /// Gets the <see cref="LedDotCorrection"/> of the RGB LED of the specified number.
        /// </summary>
        /// <param name="ledIndex">The 0 based index of the RGB LED to get the
        /// <see cref="LedDotCorrection"/> data for.</param>
        /// <returns>The <see cref="LedDotCorrection"/> of the specified RGB LED.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="ledIndex"/> is not a valid
        /// value.</exception>
        public LedDotCorrection this[int ledIndex]
        {
            get { return this.ledDotCorrections[ledIndex]; }
        }

        /// <inheritdoc/>
        LedDotCorrection IReadOnlyList<LedDotCorrection>.this[int index]
        {
            get { return this[index]; }
        }

        /// <inheritdoc/>
        public IEnumerator<LedDotCorrection> GetEnumerator()
        {
            return ((IEnumerable<LedDotCorrection>)this.ledDotCorrections).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
