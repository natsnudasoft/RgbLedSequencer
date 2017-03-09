// <copyright file="UnexpectedInstructionException.cs" company="natsnudasoft">
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

namespace RgbLedSequencerLibrary.CommandInterface
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using static System.FormattableString;

    /// <summary>
    /// The exception that is thrown when an instruction that was received from an RGB LED Sequencer
    /// was an unexpected value.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public sealed class UnexpectedInstructionException : Exception
    {
        private const string DefaultMessage = "Unexpected instruction received.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstructionException"/> class.
        /// </summary>
        public UnexpectedInstructionException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstructionException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.
        /// </param>
        public UnexpectedInstructionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstructionException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">The exception that is the cause of the current exception.
        /// </param>
        public UnexpectedInstructionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstructionException"/> class.
        /// </summary>
        /// <param name="expectedInstruction">The instruction that was expected from the RGB LED
        /// Sequencer.</param>
        /// <param name="receivedInstruction">The instruction that was received from the RGB LED
        /// Sequencer.</param>
        public UnexpectedInstructionException(
            ReceiveInstruction expectedInstruction,
            ReceiveInstruction receivedInstruction)
            : base(CreateMessage(expectedInstruction, receivedInstruction))
        {
            this.ExpectedInstruction = expectedInstruction;
            this.ReceivedInstruction = receivedInstruction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedInstructionException"/> class.
        /// </summary>
        /// <param name="expectedInstruction">The instruction that was expected from the RGB LED
        /// Sequencer.</param>
        /// <param name="receivedInstruction">The instruction that was received from the RGB LED
        /// Sequencer.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.
        /// </param>
        public UnexpectedInstructionException(
            ReceiveInstruction expectedInstruction,
            ReceiveInstruction receivedInstruction,
            Exception innerException)
            : base(CreateMessage(expectedInstruction, receivedInstruction), innerException)
        {
            this.ExpectedInstruction = expectedInstruction;
            this.ReceivedInstruction = receivedInstruction;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private UnexpectedInstructionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ExpectedInstruction = (ReceiveInstruction)info
                .GetByte(nameof(this.ExpectedInstruction));
            this.ReceivedInstruction = (ReceiveInstruction)info
                .GetByte(nameof(this.ReceivedInstruction));
        }

        /// <summary>
        /// Gets the instruction that was expected from the RGB LED Sequencer.
        /// </summary>
        public ReceiveInstruction ExpectedInstruction { get; }

        /// <summary>
        /// Gets the instruction that was received from the RGB LED Sequencer.
        /// </summary>
        public ReceiveInstruction ReceivedInstruction { get; }

        /// <inheritdoc/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this.ExpectedInstruction), this.ExpectedInstruction);
            info.AddValue(nameof(this.ReceivedInstruction), this.ReceivedInstruction);
        }

        private static string CreateMessage(
            ReceiveInstruction expectedInstruction,
            ReceiveInstruction receivedInstruction)
        {
            return Invariant($"Expected {expectedInstruction} but received {receivedInstruction}.");
        }
    }
}