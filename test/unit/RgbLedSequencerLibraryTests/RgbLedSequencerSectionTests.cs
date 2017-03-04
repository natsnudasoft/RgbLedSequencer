// <copyright file="RgbLedSequencerSectionTests.cs" company="natsnudasoft">
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

namespace RgbLedSequencerLibraryTests
{
    using System;
    using Extension;
    using RgbLedSequencerLibrary;
    using Xunit;

    public sealed class RgbLedSequencerSectionTests
    {
        private static readonly Type SutType = typeof(RgbLedSequencerSection);

        [Theory]
        [InlineAutoMoqData(nameof(RgbLedSequencerSection.MaxDotCorrection), 63)]
        [InlineAutoMoqData(nameof(RgbLedSequencerSection.MaxGrayscale), 255)]
        [InlineAutoMoqData(nameof(RgbLedSequencerSection.MaxStepCount), 770)]
        [InlineAutoMoqData(nameof(RgbLedSequencerSection.MaxStepDelay), 65535)]
        [InlineAutoMoqData(nameof(RgbLedSequencerSection.RgbLedCount), 5)]
        [InlineAutoMoqData(nameof(RgbLedSequencerSection.SequenceCount), 10)]
        public void ConfigurationPropertiesCorrectDefaultValues(
            string propertyName,
            object expectedDefault,
            RgbLedSequencerSection sut)
        {
            var actualDefault = SutType.GetProperty(propertyName).GetValue(sut);

            Assert.Equal(expectedDefault, actualDefault);
        }

        [Theory]
        [InlineAutoMoqData(nameof(RgbLedSequencerSection.SerialPort))]
        public void ConfigurationElementPropertiesNotNull(
            string propertyName,
            RgbLedSequencerSection sut)
        {
            var actualValue = SutType.GetProperty(propertyName).GetValue(sut);

            Assert.NotNull(actualValue);
        }
    }
}
