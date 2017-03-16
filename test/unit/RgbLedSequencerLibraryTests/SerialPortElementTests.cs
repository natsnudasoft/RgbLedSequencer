// <copyright file="SerialPortElementTests.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerLibraryTests
{
    using System;
    using System.IO.Ports;
    using NatsnudaLibrary.TestExtensions;
    using RgbLedSequencerLibrary;
    using Xunit;

    public sealed class SerialPortElementTests
    {
        private static readonly Type SutType = typeof(SerialPortElement);

        [Theory]
        [InlineAutoMoqData(nameof(SerialPortElement.PortName), "COM1")]
        [InlineAutoMoqData(nameof(SerialPortElement.BaudRate), 38400)]
        [InlineAutoMoqData(nameof(SerialPortElement.Parity), Parity.None)]
        [InlineAutoMoqData(nameof(SerialPortElement.DataBits), 8)]
        [InlineAutoMoqData(nameof(SerialPortElement.StopBits), StopBits.One)]
        [InlineAutoMoqData(nameof(SerialPortElement.ReadTimeout), 3000)]
        [InlineAutoMoqData(nameof(SerialPortElement.WriteTimeout), 3000)]
        public void SerialPortConfigurationPropertyCorrectDefaultValues(
            string propertyName,
            object expectedDefault,
            SerialPortElement sut)
        {
            var actualDefault =
                SutType.GetProperty(propertyName).GetValue(sut);

            Assert.Equal(expectedDefault, actualDefault);
        }
    }
}
