// <copyright file="ParameterValidation.cs" company="natsnudasoft">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using static System.FormattableString;

    /// <summary>
    /// Provides a class to manage the validation of parameters and the throwing of exceptions if
    /// validation errors occur.
    /// </summary>
    [DebuggerStepThrough]
    public static class ParameterValidation
    {
        /// <summary>
        /// Validates that the specified value is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate is not <see langword="null"/>.</param>
        /// <param name="valueName">The name of the parameter being validated.</param>
        [ContractArgumentValidator]
        public static void IsNotNull<T>([ValidatedNotNull]T value, string valueName)
            where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(valueName);
            }
        }

        /// <summary>
        /// Validates that the specified nullable value has a value assigned.
        /// </summary>
        /// <typeparam name="T">The type of the nullable value being validated.</typeparam>
        /// <param name="value">The nullable value to validate is not <see langword="null"/>.
        /// </param>
        /// <param name="valueName">The name of the parameter being validated.</param>
        [ContractArgumentValidator]
        public static void IsNotNull<T>([ValidatedNotNull]T? value, string valueName)
            where T : struct
        {
            if (!value.HasValue)
            {
                throw new ArgumentNullException(valueName);
            }
        }

        /// <summary>
        /// Validates that the specified string is not empty.
        /// </summary>
        /// <param name="value">The string to validate is not empty.</param>
        /// <param name="valueName">The name of the parameter being validated.</param>
        public static void IsNotEmpty(string value, string valueName)
        {
            if (value?.Length == 0)
            {
                throw new ArgumentException("Value must not be empty.", valueName);
            }
        }

        /// <summary>
        /// Validates that the specified value is greater than a specified compare value.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate is greater than the specified compare value.
        /// </param>
        /// <param name="compareValue">The compare value to validate against.</param>
        /// <param name="valueName">The name of the parameter being validated.</param>
        public static void IsGreaterThan<T>(T value, T compareValue, string valueName)
            where T : IComparable<T>
        {
            if (value?.CompareTo(compareValue) <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    valueName,
                    Invariant($"Value must be greater than {compareValue}."));
            }
        }

        /// <summary>
        /// Validates that the specified value is less than a specified compare value.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate is less than the specified compare value.
        /// </param>
        /// <param name="compareValue">The compare value to validate against.</param>
        /// <param name="valueName">The name of the parameter being validated.</param>
        public static void IsLessThan<T>(T value, T compareValue, string valueName)
            where T : IComparable<T>
        {
            if (value?.CompareTo(compareValue) >= 0)
            {
                throw new ArgumentOutOfRangeException(
                    valueName,
                    Invariant($"Value must be less than {compareValue}."));
            }
        }

        /// <summary>
        /// Validates that the specified value is greater than or equal to a specified compare
        /// value.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate is greater than or equal to the specified
        /// compare value.</param>
        /// <param name="compareValue">The compare value to validate against.</param>
        /// <param name="valueName">The name of the parameter being validated.</param>
        public static void IsGreaterThanOrEqualTo<T>(T value, T compareValue, string valueName)
            where T : IComparable<T>
        {
            if (value?.CompareTo(compareValue) < 0)
            {
                throw new ArgumentOutOfRangeException(
                    valueName,
                    Invariant($"Value must be greater than or equal to {compareValue}."));
            }
        }

        /// <summary>
        /// Validates that the specified value is less than or equal to a specified compare
        /// value.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate is less than or equal to the specified
        /// compare value.</param>
        /// <param name="compareValue">The compare value to validate against.</param>
        /// <param name="valueName">The name of the parameter being validated.</param>
        public static void IsLessThanOrEqualTo<T>(T value, T compareValue, string valueName)
            where T : IComparable<T>
        {
            if (value?.CompareTo(compareValue) > 0)
            {
                throw new ArgumentOutOfRangeException(
                    valueName,
                    Invariant($"Value must be less than or equal to {compareValue}."));
            }
        }

        /// <summary>
        /// Validates that the specified value is between a specified minimum and maximum value.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate is between the specified minimum and maximum
        /// value.</param>
        /// <param name="minValue">The minimum value to validate against.</param>
        /// <param name="maxValue">The maximum value to validate against.</param>
        /// <param name="valueName">The name of the parameter being validated.</param>
        public static void IsBetween<T>(T value, T minValue, T maxValue, string valueName)
            where T : IComparable<T>
        {
            if (value?.CompareTo(minValue) <= 0 || value?.CompareTo(maxValue) >= 0)
            {
                throw new ArgumentOutOfRangeException(
                    valueName,
                    Invariant($"Value must be between {minValue} and {maxValue}."));
            }
        }

        /// <summary>
        /// Validates that the specified value is between a specified minimum inclusive and maximum
        /// inclusive value.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate is between the specified minimum and maximum
        /// value.</param>
        /// <param name="minValue">The minimum inclusive value to validate against.</param>
        /// <param name="maxValue">The maximum inclusive value to validate against.</param>
        /// <param name="valueName">The name of the parameter being validated.</param>
        public static void IsBetweenInclusive<T>(T value, T minValue, T maxValue, string valueName)
            where T : IComparable<T>
        {
            if (value?.CompareTo(minValue) < 0 || value?.CompareTo(maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException(
                    valueName,
                    Invariant(
                        $"Value must be between {minValue} inclusive and {maxValue} inclusive."));
            }
        }

        /// <summary>
        /// Validates that the specified value is <see langword="true"/>.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="message">The error message that explains the reason for the exception that
        /// will be thrown if this validation fails.</param>
        /// <param name="valueName">The name of the value being validated.</param>
        public static void IsTrue(bool value, [Localizable(false)]string message, string valueName)
        {
            if (!value)
            {
                throw new ArgumentException(message, valueName);
            }
        }

        /// <summary>
        /// Validates that the specified value is <see langword="false"/>.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="message">The error message that explains the reason for the exception that
        /// will be thrown if this validation fails.</param>
        /// <param name="valueName">The name of the value being validated.</param>
        public static void IsFalse(bool value, [Localizable(false)]string message, string valueName)
        {
            IsTrue(!value, message, valueName);
        }
    }
}
