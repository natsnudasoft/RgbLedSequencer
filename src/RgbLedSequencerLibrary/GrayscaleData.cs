// <copyright file="GrayscaleData.cs" company="natsnudasoft">
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
    /// Represents grayscale (PWM brightness control) values for a number of RGB LEDs part of
    /// an RGB LED Sequencer.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class GrayscaleData
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly LedGrayscale[] ledGrayscales;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleData"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <param name="ledGrayscaleFactory">The factory to use to create instances of
        /// <see cref="LedGrayscale"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/>, or
        /// <paramref name="ledGrayscaleFactory"/> is <see langword="null"/>.</exception>
        public GrayscaleData(
            IRgbLedSequencerConfiguration sequencerConfig,
            Func<LedGrayscale> ledGrayscaleFactory)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));
            ParameterValidation.IsNotNull(ledGrayscaleFactory, nameof(ledGrayscaleFactory));

            var rgbLedCount = sequencerConfig.RgbLedCount;
            this.ledGrayscales = new LedGrayscale[rgbLedCount];
            for (int i = 0; i < rgbLedCount; ++i)
            {
#pragma warning disable CC0031 // Check for null before calling a delegate
                this.ledGrayscales[i] = ledGrayscaleFactory();
#pragma warning restore CC0031 // Check for null before calling a delegate
            }
        }

        /// <summary>
        /// Gets the debugger display string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplay => "{" +
            string.Join(" ", this.ledGrayscales.Select(l => l.DebuggerDisplay)) + "}";

        /// <summary>
        /// Gets the <see cref="LedGrayscale"/> of the RGB LED of the specified number.
        /// </summary>
        /// <param name="ledIndex">The 0 based index of the RGB LED to get the
        /// <see cref="LedGrayscale"/> data for.</param>
        /// <returns>The <see cref="LedGrayscale"/> of the specified RGB LED.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="ledIndex"/> is not a valid
        /// value.</exception>
        public LedGrayscale this[int ledIndex]
        {
            get { return this.ledGrayscales[ledIndex]; }
        }
    }
}
