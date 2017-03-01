// <copyright file="ParameterValidationTests.cs" company="natsnudasoft">
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
    using RgbLedSequencerLibrary;
    using Xunit;

    public sealed class ParameterValidationTests
    {
        [Fact]
        public void IsNotNullWithNullValueThrows()
        {
            const string NullValue = null;
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsNotNull(NullValue, ParamName));

            Assert.IsType<ArgumentNullException>(ex);
            Assert.Equal(ParamName, ((ArgumentNullException)ex).ParamName);
        }

        [Fact]
        public void IsNotNullWithValueDoesNotThrow()
        {
            const string NotNullString = "value";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsNotNull(NotNullString, ParamName));

            Assert.Null(ex);
        }

        [Fact]
        public void IsNotNullWithNullableNoValueThrows()
        {
            int? nullableNoValue = null;
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsNotNull(nullableNoValue, ParamName));

            Assert.IsType<ArgumentNullException>(ex);
            Assert.Equal(ParamName, ((ArgumentNullException)ex).ParamName);
        }

        [Fact]
        public void IsNotNullWithNullableValueDoesNotThrow()
        {
            int? nullableValue = 5;
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsNotNull(nullableValue, ParamName));

            Assert.Null(ex);
        }

        [Fact]
        public void IsNotEmptyWithEmptyValueThrows()
        {
            const string EmptyString = "";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsNotEmpty(EmptyString, ParamName));

            Assert.IsType<ArgumentException>(ex);
            Assert.Equal(ParamName, ((ArgumentException)ex).ParamName);
        }

        [Theory]
        [InlineData("value")]
        [InlineData(null)]
        public void IsNotEmptyWithAllowedValueDoesNotThrow(string value)
        {
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsNotEmpty(value, ParamName));

            Assert.Null(ex);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(15)]
        public void IsGreaterThanWithDisallowedValueThrows(int value)
        {
            const int CompareValue = 15;
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsGreaterThan(value, CompareValue, ParamName));

            Assert.IsType<ArgumentOutOfRangeException>(ex);
            Assert.Equal(ParamName, ((ArgumentOutOfRangeException)ex).ParamName);
        }

        [Theory]
        [InlineData("u")]
        [InlineData(null)]
        public void IsGreaterThanWithAllowedValueDoesNotThrow(string value)
        {
            const string CompareValue = "t";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsGreaterThan(value, CompareValue, ParamName));

            Assert.Null(ex);
        }

        [Theory]
        [InlineData(40)]
        [InlineData(30)]
        public void IsLessThanWithDisallowedValueThrows(int value)
        {
            const int CompareValue = 30;
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsLessThan(value, CompareValue, ParamName));

            Assert.IsType<ArgumentOutOfRangeException>(ex);
            Assert.Equal(ParamName, ((ArgumentOutOfRangeException)ex).ParamName);
        }

        [Theory]
        [InlineData("s")]
        [InlineData(null)]
        public void IsLessThanWithAllowedValueDoesNotThrow(string value)
        {
            const string CompareValue = "t";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsLessThan(value, CompareValue, ParamName));

            Assert.Null(ex);
        }

        [Fact]
        public void IsGreaterThanOrEqualToWithDisallowedValueThrows()
        {
            const int Value = 10;
            const int CompareValue = 15;
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsGreaterThanOrEqualTo(Value, CompareValue, ParamName));

            Assert.IsType<ArgumentOutOfRangeException>(ex);
            Assert.Equal(ParamName, ((ArgumentOutOfRangeException)ex).ParamName);
        }

        [Theory]
        [InlineData("t")]
        [InlineData("u")]
        [InlineData(null)]
        public void IsGreaterThanOrEqualToWithAllowedValueDoesNotThrow(string value)
        {
            const string CompareValue = "t";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsGreaterThanOrEqualTo(value, CompareValue, ParamName));

            Assert.Null(ex);
        }

        [Fact]
        public void IsLessThanOrEqualToWithDisallowedValueThrows()
        {
            const int Value = 67;
            const int CompareValue = 25;
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsLessThanOrEqualTo(Value, CompareValue, ParamName));

            Assert.IsType<ArgumentOutOfRangeException>(ex);
            Assert.Equal(ParamName, ((ArgumentOutOfRangeException)ex).ParamName);
        }

        [Theory]
        [InlineData("s")]
        [InlineData("t")]
        [InlineData(null)]
        public void IsLessThanOrEqualToWithAllowedValueDoesNotThrow(string value)
        {
            const string CompareValue = "t";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsLessThanOrEqualTo(value, CompareValue, ParamName));

            Assert.Null(ex);
        }

        [Theory]
        [InlineData(102)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(50)]
        public void IsBetweenWithDisallowedValueThrows(int value)
        {
            const int MinValue = 10;
            const int MaxValue = 50;
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsBetween(value, MinValue, MaxValue, ParamName));

            Assert.IsType<ArgumentOutOfRangeException>(ex);
            Assert.Equal(ParamName, ((ArgumentOutOfRangeException)ex).ParamName);
        }

        [Theory]
        [InlineData("t")]
        [InlineData(null)]
        public void IsBetweenWithAllowedValueDoesNotThrow(string value)
        {
            const string MinValue = "s";
            const string MaxValue = "u";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsBetween(value, MinValue, MaxValue, ParamName));

            Assert.Null(ex);
        }

        [Theory]
        [InlineData(102)]
        [InlineData(5)]
        public void IsBetweenInclusiveWithDisallowedValueThrows(int value)
        {
            const int MinValue = 10;
            const int MaxValue = 50;
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsBetweenInclusive(value, MinValue, MaxValue, ParamName));

            Assert.IsType<ArgumentOutOfRangeException>(ex);
            Assert.Equal(ParamName, ((ArgumentOutOfRangeException)ex).ParamName);
        }

        [Theory]
        [InlineData("s")]
        [InlineData("t")]
        [InlineData("u")]
        [InlineData(null)]
        public void IsBetweenInclusiveWithAllowedValueDoesNotThrow(string value)
        {
            const string MinValue = "s";
            const string MaxValue = "u";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsBetweenInclusive(value, MinValue, MaxValue, ParamName));

            Assert.Null(ex);
        }

        [Fact]
        public void IsTrueWithFalseValueThrows()
        {
            const bool Value = false;
            const string Message = "This is a message.";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsTrue(Value, Message, ParamName));

            Assert.IsType<ArgumentException>(ex);
            Assert.Equal(ParamName, ((ArgumentException)ex).ParamName);
        }

        [Fact]
        public void IsTrueWithTrueValueDoesNowThrow()
        {
            const bool Value = true;
            const string Message = "This is a message.";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsTrue(Value, Message, ParamName));

            Assert.Null(ex);
        }

        [Fact]
        public void IsFalseWithTrueValueThrows()
        {
            const bool Value = true;
            const string Message = "This is a message.";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsFalse(Value, Message, ParamName));

            Assert.IsType<ArgumentException>(ex);
            Assert.Equal(ParamName, ((ArgumentException)ex).ParamName);
        }

        [Fact]
        public void IsFalseWithFalseValueDoesNotThrow()
        {
            const bool Value = false;
            const string Message = "This is a message.";
            const string ParamName = "testParam";
            var ex = Record.Exception(
                () => ParameterValidation.IsFalse(Value, Message, ParamName));

            Assert.Null(ex);
        }
    }
}
