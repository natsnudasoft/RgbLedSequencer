// <copyright file="LedGrayscale.cs" company="natsnudasoft">
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
    /// Represents grayscale (PWM brightness control) values for an RGB LED.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class LedGrayscale : INotifyPropertyChanged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IRgbLedSequencerConfiguration sequencerConfig;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte redGrayscale;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte greenGrayscale;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte blueGrayscale;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedGrayscale"/> class.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> that
        /// describes the configuration of the RGB LED Sequencer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/> is
        /// <see langword="null"/>.</exception>
        public LedGrayscale(IRgbLedSequencerConfiguration sequencerConfig)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));

            this.sequencerConfig = sequencerConfig;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the grayscale (PWM brightness control) value for the red part of an RGB
        /// LED. This value can not be larger than the maximum value defined in the application
        /// config, and will be automatically clamped.
        /// </summary>
        public byte Red
        {
            get
            {
                return this.redGrayscale;
            }

            set
            {
                value = (byte)Math.Min(value, this.sequencerConfig.MaxGrayscale);
                if (this.redGrayscale != value)
                {
                    this.redGrayscale = value;
                    this.OnPropertyChanged(nameof(this.Red));
                }
            }
        }

        /// <summary>
        /// Gets or sets the grayscale (PWM brightness control) value for the green part of an RGB
        /// LED. This value can not be larger than the maximum value defined in the application
        /// config, and will be automatically clamped.
        /// </summary>
        public byte Green
        {
            get
            {
                return this.greenGrayscale;
            }

            set
            {
                value = (byte)Math.Min(value, this.sequencerConfig.MaxGrayscale);
                if (this.greenGrayscale != value)
                {
                    this.greenGrayscale = value;
                    this.OnPropertyChanged(nameof(this.Green));
                }
            }
        }

        /// <summary>
        /// Gets or sets the grayscale (PWM brightness control) value for the blue part of an RGB
        /// LED. This value can not be larger than the maximum value defined in the application
        /// config, and will be automatically clamped.
        /// </summary>
        public byte Blue
        {
            get
            {
                return this.blueGrayscale;
            }

            set
            {
                value = (byte)Math.Min(value, this.sequencerConfig.MaxGrayscale);
                if (this.blueGrayscale != value)
                {
                    this.blueGrayscale = value;
                    this.OnPropertyChanged(nameof(this.Blue));
                }
            }
        }

        /// <summary>
        /// Gets the debugger display string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DebuggerDisplay => "#" +
            this.Red.ToString("X2", CultureInfo.CurrentCulture) +
            this.Green.ToString("X2", CultureInfo.CurrentCulture) +
            this.Blue.ToString("X2", CultureInfo.CurrentCulture);

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
