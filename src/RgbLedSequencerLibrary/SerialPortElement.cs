// <copyright file="SerialPortElement.cs" company="natsnudasoft">
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
    using System.Configuration;
    using System.IO.Ports;

    /// <summary>
    /// Represents COM Port configuration within an RGB LED Sequencer configuration section.
    /// </summary>
    /// <seealso cref="ConfigurationElement" />
    public sealed class SerialPortElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the name of the port to use for communications.
        /// </summary>
        [ConfigurationProperty(nameof(PortName), DefaultValue = "COM1", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public string PortName
        {
            get
            {
                return (string)this[nameof(this.PortName)];
            }
        }

        /// <summary>
        /// Gets the serial baud rate.
        /// </summary>
        [ConfigurationProperty(nameof(BaudRate), DefaultValue = 38400, IsRequired = false)]
        [ConfigurationValidator(typeof(BaudRateValidator))]
        public int BaudRate
        {
            get
            {
                return (int)this[nameof(this.BaudRate)];
            }
        }

        /// <summary>
        /// Gets the parity-checking protocol.
        /// </summary>
        [ConfigurationProperty(nameof(Parity), DefaultValue = Parity.None, IsRequired = false)]
        public Parity Parity
        {
            get
            {
                return (Parity)this[nameof(this.Parity)];
            }
        }

        /// <summary>
        /// Gets the standard length of data bits per byte.
        /// </summary>
        [ConfigurationProperty(nameof(DataBits), DefaultValue = 8, IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MinValue = 5, MaxValue = 8)]
        public int DataBits
        {
            get
            {
                return (int)this[nameof(this.DataBits)];
            }
        }

        /// <summary>
        /// Gets the standard number of stop bits per byte.
        /// </summary>
        [ConfigurationProperty(nameof(StopBits), DefaultValue = StopBits.One, IsRequired = false)]
        public StopBits StopBits
        {
            get
            {
                return (StopBits)this[nameof(this.StopBits)];
            }
        }

        /// <summary>
        /// Gets the number of milliseconds before a time-out occurs when a read operation does not
        /// finish.
        /// </summary>
        [ConfigurationProperty(nameof(ReadTimeout), DefaultValue = 3000, IsRequired = false)]
        public int ReadTimeout
        {
            get
            {
                return (int)this[nameof(this.ReadTimeout)];
            }
        }

        /// <summary>
        /// Gets the number of milliseconds before a time-out occurs when a write operation does not
        /// finish.
        /// </summary>
        [ConfigurationProperty(nameof(WriteTimeout), DefaultValue = 3000, IsRequired = false)]
        public int WriteTimeout
        {
            get
            {
                return (int)this[nameof(this.WriteTimeout)];
            }
        }
    }
}
