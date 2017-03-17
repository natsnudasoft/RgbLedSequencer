// <copyright file="ReceiveInstruction.cs" company="natsnudasoft">
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
    /// <summary>
    /// Defines the available instruction set that can be -received- by the
    /// <see cref="RgbLedSequencerLibrary"/> from the physical RGB LED Sequencer.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1028:EnumStorageShouldBeInt32",
        Justification = "Consistent with RGB LED Sequencer.")]
    public enum ReceiveInstruction : byte
    {
        /// <summary>
        /// An undefined command was received.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The RGB LED Sequencer is ready to receive more data.
        /// </summary>
        Ready = 16,

        /// <summary>
        /// The RGB LED Sequencer is waiting for a handshake signal.
        /// </summary>
        Handshake = 17
    }
}
