// <copyright file="RgbLedSequencerSection.cs" company="natsnudasoft">
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
    using System.Configuration;

    /// <summary>
    /// Represents the RGB LED Sequencer section within a configuration file.
    /// </summary>
    /// <seealso cref="ConfigurationSection" />
    /// <seealso cref="IRgbLedSequencerConfiguration" />
    public sealed class RgbLedSequencerSection : ConfigurationSection, IRgbLedSequencerConfiguration
    {
        /// <inheritdoc/>
        /// <remarks>The RGB LED Sequencer expects a 6 bit unsigned value for this.</remarks>
        [ConfigurationProperty(nameof(MaxDotCorrection), DefaultValue = "63", IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 63)]
        public int MaxDotCorrection
        {
            get
            {
                return (int)this[nameof(this.MaxDotCorrection)];
            }
        }

        /// <inheritdoc/>
        /// <remarks>The RGB LED Sequencer expects an 8 bit unsigned value for this and will convert
        /// it to a 12 bit unsigned value.</remarks>
        [ConfigurationProperty(nameof(MaxGrayscale), DefaultValue = "255", IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 255)]
        public int MaxGrayscale
        {
            get
            {
                return (int)this[nameof(this.MaxGrayscale)];
            }
        }

        /// <inheritdoc/>
        /// <remarks>The RGB LED Sequencer expects a 16 bit unsigned value for this.</remarks>
        [ConfigurationProperty(nameof(MaxStepCount), DefaultValue = "770", IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 65535)]
        public int MaxStepCount
        {
            get
            {
                return (int)this[nameof(this.MaxStepCount)];
            }
        }

        /// <inheritdoc/>
        /// <remarks>The RGB LED Sequencer expects a 16 bit unsigned value for this.</remarks>
        [ConfigurationProperty(nameof(MaxStepDelay), DefaultValue = "65535", IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 65535)]
        public int MaxStepDelay
        {
            get
            {
                return (int)this[nameof(this.MaxStepDelay)];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty(nameof(RgbLedCount), DefaultValue = "5", IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 255)]
        public int RgbLedCount
        {
            get
            {
                return (int)this[nameof(this.RgbLedCount)];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty(nameof(SequenceCount), DefaultValue = "10", IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 255)]
        public int SequenceCount
        {
            get
            {
                return (int)this[nameof(this.SequenceCount)];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty(nameof(SerialPort), IsRequired = true)]
        public SerialPortElement SerialPort
        {
            get
            {
                return (SerialPortElement)this[nameof(this.SerialPort)];
            }
        }
    }
}
