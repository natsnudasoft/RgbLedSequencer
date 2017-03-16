// <copyright file="ISerialPortAdapter.cs" company="natsnudasoft">
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
    using System.IO;
    using System.IO.Ports;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface for a <see cref="SerialPort"/> adapter.
    /// </summary>
    public interface ISerialPortAdapter
    {
        /// <summary>
        /// Gets or sets the name of the port to use for communications.
        /// </summary>
        string PortName { get; set; }

        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        int BaudRate { get; set; }

        /// <summary>
        /// Gets or sets the parity-checking protocol.
        /// </summary>
        Parity Parity { get; set; }

        /// <summary>
        /// Gets or sets the standard length of data bits per byte.
        /// </summary>
        int DataBits { get; set; }

        /// <summary>
        /// Gets or sets the standard number of stop bits per byte.
        /// </summary>
        StopBits StopBits { get; set; }

        /// <summary>
        /// Gets a value indicating whether the port is open or not.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets or sets the number of milliseconds before a time-out occurs when a read operation
        /// does not finish.
        /// </summary>
        int ReadTimeout { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds before a time-out occurs when a write operation
        /// does not finish.
        /// </summary>
        int WriteTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the break state is set.
        /// </summary>
        bool BreakState { get; set; }

        /// <summary>
        /// Gets a value indicating whether the Carrier Detect line for the port is triggered.
        /// </summary>
        bool CDHolding { get; }

        /// <summary>
        /// Gets a value indicating whether the Clear-to-Send line for the port is triggered.
        /// </summary>
        bool CtsHolding { get; }

        /// <summary>
        /// Gets a value indicating whether the Data Set Ready (DSR) signal for the port is
        /// triggered.
        /// </summary>
        bool DsrHolding { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the Data Terminal Ready (DTR) signal is enabled
        /// during serial communication.
        /// </summary>
        bool DtrEnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Request to Send (RTS) signal is enabled
        /// during serial communication.
        /// </summary>
        bool RtsEnable { get; set; }

        /// <summary>
        /// Gets or sets the handshaking protocol for serial port transmission.
        /// </summary>
        Handshake Handshake { get; set; }

        /// <summary>
        /// Opens a new serial port connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the port connection, and disposes of the internal <see cref="Stream" /> object.
        /// </summary>
        void Close();

        /// <summary>
        /// Discards data from the serial driver's receive buffer.
        /// </summary>
        void DiscardInBuffer();

        /// <summary>
        /// Discards data from the serial driver's transmit buffer.
        /// </summary>
        void DiscardOutBuffer();

        /// <summary>
        /// Reads one byte from the serial port.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation. The
        /// result of the <see cref="Task{TResult}"/> contains the byte that was read from the
        /// serial port.</returns>
        Task<byte> ReadByteAsync();

        /// <summary>
        /// Writes one byte to the serial port.
        /// </summary>
        /// <param name="value">The byte to write to the serial port.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task WriteByteAsync(byte value);
    }
}
