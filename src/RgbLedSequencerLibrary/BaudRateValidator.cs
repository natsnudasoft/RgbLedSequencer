// <copyright file="BaudRateValidator.cs" company="natsnudasoft">
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
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Provides validation of baud rates for a RGB LED Sequencer.
    /// </summary>
    /// <seealso cref="ConfigurationValidatorBase" />
    public sealed class BaudRateValidator : ConfigurationValidatorBase
    {
        // While getting this data from the serial port itself is possible, doing so may report
        // higher baud rates than the RGB LED Sequencer officially supports.
        private static readonly HashSet<int> SupportedBaudRates = new HashSet<int>
        {
            110,
            300,
            600,
            1200,
            1800,
            2400,
            4800,
            7200,
            9600,
            14400,
            19200,
            38400,
            57600,
            76800,
            115200
        };

        /// <inheritdoc/>
        public override bool CanValidate(Type type)
        {
            return type == typeof(int);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not the correct type, or
        /// is not an allowed baud rate.</exception>
        public override void Validate(object value)
        {
            if (value == null || value.GetType() != typeof(int))
            {
                throw new ArgumentException(
                    "Value is not the correct type.",
                    nameof(value));
            }

            if (!SupportedBaudRates.Contains((int)value))
            {
                throw new ArgumentException(
                    "Value is not an allowed baud rate.",
                    nameof(value));
            }
        }
    }
}
