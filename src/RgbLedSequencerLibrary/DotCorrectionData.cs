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

namespace RgbLedSequencerLibrary
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Represents dot correction (brightness difference compensation) values for a number of RGB
    /// LEDs part of an RGB LED Sequencer.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class DotCorrectionData
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly LedDotCorrection[] ledDotCorrections;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotCorrectionData"/> class.
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

        /// <summary>
        /// Gets the debugger display string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
    }
}
