// <copyright file="BaudRateValidatorTests.cs" company="natsnudasoft">
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
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using Xunit;

    public sealed class BaudRateValidatorTests
    {
        [Theory]
        [InlineAutoData(typeof(string))]
        [InlineAutoData(new object[] { null })]
        public void CanValidateInvalidTypeShouldReturnFalse(
            Type invalidType,
            BaudRateValidator sut)
        {
            var result = sut.CanValidate(invalidType);

            Assert.False(result);
        }

        [Theory]
        [InlineAutoData(typeof(int))]
        public void CanValidateValidTypeShouldReturnTrue(Type validType, BaudRateValidator sut)
        {
            var result = sut.CanValidate(validType);

            Assert.True(result);
        }

        [Theory]
        [InlineAutoData(new object[] { null })]
        [InlineAutoData("67")]
        [InlineAutoData(int.MinValue)]
        [InlineAutoData(4000)]
        public void ValidateInvalidValueShouldThrow(object value, BaudRateValidator sut)
        {
            var ex = Record.Exception(() => sut.Validate(value));

            Assert.IsType<ArgumentException>(ex);
        }

        [Theory]
        [InlineAutoData(4800)]
        [InlineAutoData(38400)]
        [InlineAutoData(115200)]
        public void ValidateValidValueShouldNotThrow(int value, BaudRateValidator sut)
        {
            var ex = Record.Exception(() => sut.Validate(value));

            Assert.Null(ex);
        }
    }
}
