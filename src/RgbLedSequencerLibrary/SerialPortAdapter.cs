// <copyright file="SerialPortAdapter.cs" company="natsnudasoft">
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
    using System.IO;
    using System.IO.Ports;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an adapter for the <see cref="SerialPort"/> that reads and writes bytes to the
    /// underlying stream directly, allowing for asynchronous operations.
    /// </summary>
    /// <remarks>
    /// Based on information from:
    /// http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class SerialPortAdapter : ISerialPortAdapter, IDisposable
    {
        private readonly byte[] readBuffer = new byte[1];
        private readonly byte[] writeBuffer = new byte[1];
        private readonly SerialPort serialPort;
        private readonly bool isSerialPortOwner;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortAdapter"/> class, using the
        /// configuration values in an instance of <see cref="IRgbLedSequencerConfiguration"/> to
        /// create a default instance of <see cref="SerialPort"/>.
        /// </summary>
        /// <param name="sequencerConfig">The <see cref="IRgbLedSequencerConfiguration"/> to use
        /// to create an instance of <see cref="SerialPort"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequencerConfig"/> is
        /// <see langword="null"/>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Object is disposed in Dispose method.")]
        public SerialPortAdapter(IRgbLedSequencerConfiguration sequencerConfig)
        {
            ParameterValidation.IsNotNull(sequencerConfig, nameof(sequencerConfig));

            try
            {
                this.serialPort = new SerialPort
                {
                    PortName = sequencerConfig.SerialPort.PortName,
                    BaudRate = sequencerConfig.SerialPort.BaudRate,
                    Parity = sequencerConfig.SerialPort.Parity,
                    DataBits = sequencerConfig.SerialPort.DataBits,
                    StopBits = sequencerConfig.SerialPort.StopBits,
                    ReadTimeout = sequencerConfig.SerialPort.ReadTimeout,
                    WriteTimeout = sequencerConfig.SerialPort.WriteTimeout,
                    DtrEnable = true,
                    RtsEnable = true
                };
            }
            catch
            {
                this.serialPort?.Dispose();
                throw;
            }

            this.isSerialPortOwner = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortAdapter"/> class.
        /// </summary>
        /// <param name="serialPort">The <see cref="SerialPort"/> instance that this adapter class
        /// will wrap.</param>
        public SerialPortAdapter(SerialPort serialPort)
        {
            ParameterValidation.IsNotNull(serialPort, nameof(serialPort));

            this.serialPort = serialPort;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"><see cref="PortName"/> was set to empty, a value
        /// that starts with "\\", or a value that is not valid.</exception>
        /// <exception cref="ArgumentNullException"><see cref="PortName"/> was set to null.
        /// </exception>
        /// <exception cref="InvalidOperationException">The specified port is open.</exception>
        public string PortName
        {
            get { return this.serialPort.PortName; }

            set { this.serialPort.PortName = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="BaudRate"/> was set to a value
        /// less than or equal to zero, or greater than the maximum allowable baud rate for the
        /// device.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public int BaudRate
        {
            get { return this.serialPort.BaudRate; }

            set { this.serialPort.BaudRate = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="Parity"/> was set to a value
        /// that is not a valid value in the <see cref="System.IO.Ports.Parity"/> enumeration.
        /// </exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public Parity Parity
        {
            get { return this.serialPort.Parity; }

            set { this.serialPort.Parity = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="DataBits"/> was set to a value
        /// less than 5 or more than 8.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public int DataBits
        {
            get { return this.serialPort.DataBits; }

            set { this.serialPort.DataBits = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="StopBits"/> was set to a value
        /// that is not a valid value in the <see cref="System.IO.Ports.StopBits"/> enumeration.
        /// </exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public StopBits StopBits
        {
            get { return this.serialPort.StopBits; }

            set { this.serialPort.StopBits = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="ReadTimeout"/> was set to a
        /// value less then zero that was not <see cref="SerialPort.InfiniteTimeout"/>.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public int ReadTimeout
        {
            get { return this.serialPort.ReadTimeout; }

            set { this.serialPort.ReadTimeout = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="WriteTimeout"/> was set to a
        /// value less then zero that was not <see cref="SerialPort.InfiniteTimeout"/>.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public int WriteTimeout
        {
            get { return this.serialPort.WriteTimeout; }

            set { this.serialPort.WriteTimeout = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">The specified port is not open.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public bool BreakState
        {
            get { return this.serialPort.BreakState; }

            set { this.serialPort.BreakState = value; }
        }

        /// <inheritdoc/>
        public bool IsOpen
        {
            get { return this.serialPort.IsOpen; }
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">The specified port is not open.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public bool CDHolding
        {
            get { return this.serialPort.CDHolding; }
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">The specified port is not open.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public bool CtsHolding
        {
            get { return this.serialPort.CtsHolding; }
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">The specified port is not open.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public bool DsrHolding
        {
            get { return this.serialPort.DsrHolding; }
        }

        /// <inheritdoc/>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public bool DtrEnable
        {
            get { return this.serialPort.DtrEnable; }

            set { this.serialPort.DtrEnable = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="Handshake"/> was set to a value
        /// that is not a valid value in the <see cref="System.IO.Ports.Handshake"/> enumeration.
        /// </exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public Handshake Handshake
        {
            get { return this.serialPort.Handshake; }

            set { this.serialPort.Handshake = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">The specified port is not open.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public bool RtsEnable
        {
            get { return this.serialPort.RtsEnable; }

            set { this.serialPort.RtsEnable = value; }
        }

        /// <inheritdoc/>
        /// <exception cref="UnauthorizedAccessException">Access is denied to the port. The current
        /// process, or another process on the system, already has the specified COM port open.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">One or more of the properties for this
        /// instance are invalid. For example, the <see cref="Parity"/>, <see cref="DataBits"/>,
        /// or <see cref="Handshake"/> properties are not valid values; the <see cref="BaudRate"/>
        /// is less than or equal to zero; the <see cref="ReadTimeout"/> or
        /// <see cref="WriteTimeout"/> property is less than zero and is not
        /// <see cref="SerialPort.InfiniteTimeout"/>.</exception>
        /// <exception cref="ArgumentException">The port name does not begin with "COM", or the file
        /// type of the port is not supported.</exception>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        /// <exception cref="InvalidOperationException">The specified port is already open.
        /// </exception>
        public void Open()
        {
            this.serialPort.Open();
        }

        /// <inheritdoc/>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        public void Close()
        {
            this.serialPort.Close();
        }

        /// <inheritdoc/>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        /// <exception cref="InvalidOperationException">The underlying stream is closed.</exception>
        public void DiscardInBuffer()
        {
            this.serialPort.DiscardInBuffer();
        }

        /// <inheritdoc/>
        /// <exception cref="IOException">The port is in an invalid state, or an attempt to set the
        /// state of the underlying port failed.</exception>
        /// <exception cref="InvalidOperationException">The underlying stream is closed.</exception>
        public void DiscardOutBuffer()
        {
            this.serialPort.DiscardOutBuffer();
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">The read operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The specified port is not open.</exception>
        public async Task<byte> ReadByteAsync()
        {
            // Stream.ReadAsync does not handle cancellation so we just close the stream
            // to timeout.
            using (var cts = new CancellationTokenSource(this.ReadTimeout))
            using (cts.Token.Register(() => this.serialPort.BaseStream.Close()))
            {
                try
                {
                    await this.serialPort.BaseStream.ReadAsync(this.readBuffer, 0, 1)
                        .ConfigureAwait(false);
                }
                catch (IOException) when (cts.IsCancellationRequested)
                {
                    throw new TimeoutException("The read operation has timed out.");
                }
            }

            return this.readBuffer[0];
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">The write operation timed out.</exception>
        /// <exception cref="InvalidOperationException">The specified port is not open.</exception>
        public async Task WriteByteAsync(byte value)
        {
            // Stream.WriteAsync does not handle cancellation so we just close the stream
            // to timeout.
            using (var timeoutCts = new CancellationTokenSource(this.WriteTimeout))
            using (timeoutCts.Token.Register(() => this.serialPort.BaseStream.Close()))
            {
                try
                {
                    this.writeBuffer[0] = value;
                    await this.serialPort.BaseStream.WriteAsync(this.writeBuffer, 0, 1)
                        .ConfigureAwait(false);
                }
                catch (IOException) when (timeoutCts.IsCancellationRequested)
                {
                    throw new TimeoutException("The write operation has timed out.");
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.isSerialPortOwner)
                {
                    this.serialPort.Dispose();
                }
            }
        }
    }
}
