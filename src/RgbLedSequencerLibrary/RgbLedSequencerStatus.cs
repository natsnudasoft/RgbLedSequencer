// <copyright file="RgbLedSequencerStatus.cs" company="natsnudasoft">
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

    /// <summary>
    /// Represents the status of an RGB LED Sequencer.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct RgbLedSequencerStatus : IEquatable<RgbLedSequencerStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RgbLedSequencerStatus" /> struct.
        /// </summary>
        /// <param name="isAsleep">Whether or not the RGB LED Sequencer that this status represents
        /// is asleep.</param>
        /// <param name="currentSequenceIndex">The index of the current sequence that is playing on
        /// the RGB LED Sequencer that this status represents.</param>
        public RgbLedSequencerStatus(bool isAsleep, byte currentSequenceIndex)
        {
            this.IsAsleep = isAsleep;
            this.CurrentSequenceIndex = currentSequenceIndex;
        }

        /// <summary>
        /// Gets a value indicating whether or not the RGB LED Sequencer is asleep.
        /// </summary>
        /// <value><see langword="true"/> if the RGB LED Sequencer is asleep; otherwise,
        /// <see langword="false"/>.</value>
        public bool IsAsleep { get; }

        /// <summary>
        /// Gets the index of the current sequence that is playing on the RGB LED Sequencer.
        /// </summary>
        public byte CurrentSequenceIndex { get; }

        /// <summary>
        /// Gets the debugger display string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DebuggerDisplay =>
            string.Format(
                CultureInfo.CurrentCulture,
                "{{{0}: {1}, {2}: {3}}}",
                nameof(this.IsAsleep),
                this.IsAsleep,
                nameof(this.CurrentSequenceIndex),
                this.CurrentSequenceIndex);

        /// <summary>
        /// Determines if the specified operands are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(RgbLedSequencerStatus left, RgbLedSequencerStatus right)
        {
            return left.IsAsleep == right.IsAsleep &&
                left.CurrentSequenceIndex == right.CurrentSequenceIndex;
        }

        /// <summary>
        /// Determines if the specified operands are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> is not equal to
        /// <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(RgbLedSequencerStatus left, RgbLedSequencerStatus right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public bool Equals(RgbLedSequencerStatus other)
        {
            return other == this;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is RgbLedSequencerStatus && (RgbLedSequencerStatus)obj == this;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            const int InitPrime = 17;
            const int MultPrime = 23;
            var hash = InitPrime;
            unchecked
            {
                hash = (hash * MultPrime) + this.IsAsleep.GetHashCode();
                hash = (hash * MultPrime) + this.CurrentSequenceIndex.GetHashCode();
            }

            return hash;
        }
    }
}