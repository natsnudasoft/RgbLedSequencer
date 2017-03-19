// <copyright file="ISerialPortConfiguration.cs" company="natsnudasoft">
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
    using System.IO.Ports;

    /// <summary>
    /// Represents values that describe the configuration of a serial port.
    /// </summary>
    public interface ISerialPortConfiguration
    {
        /// <summary>
        /// Gets the name of the port to use for communications.
        /// </summary>
        string PortName { get; }

        /// <summary>
        /// Gets the serial baud rate.
        /// </summary>
        int BaudRate { get; }

        /// <summary>
        /// Gets the parity-checking protocol.
        /// </summary>
        Parity Parity { get; }

        /// <summary>
        /// Gets the standard length of data bits per byte.
        /// </summary>
        int DataBits { get; }

        /// <summary>
        /// Gets the standard number of stop bits per byte.
        /// </summary>
        StopBits StopBits { get; }

        /// <summary>
        /// Gets the number of milliseconds before a time-out occurs when a read operation does not
        /// finish.
        /// </summary>
        int ReadTimeout { get; }

        /// <summary>
        /// Gets the number of milliseconds before a time-out occurs when a write operation does not
        /// finish.
        /// </summary>
        int WriteTimeout { get; }
    }
}
