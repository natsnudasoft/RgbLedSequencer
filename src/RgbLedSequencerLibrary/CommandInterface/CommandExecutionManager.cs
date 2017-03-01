// <copyright file="CommandExecutionManager.cs" company="natsnudasoft">
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
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class that manages the execution of instances of <see cref="IAsyncCommand"/> on
    /// an RGB LED Sequencer, ensuring that only one command can run at a time.
    /// </summary>
    public sealed class CommandExecutionManager
    {
        private readonly object exclusiveOperationLock = new object();
        private bool isExclusiveOperationRunning;

        /// <summary>
        /// Executes the specified <see cref="IAsyncCommand"/> on the RGB LED Sequencer, if no other
        /// command is already running.
        /// </summary>
        /// <param name="command">The exclusive <see cref="IAsyncCommand"/> to execute.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="UnauthorizedAccessException">A command is already running on the RGB
        /// LED Sequencer.</exception>
        public async Task ExecuteCommandAsync(IAsyncCommand command)
        {
            ParameterValidation.IsNotNull(command, nameof(command));

            try
            {
                this.BeginExclusiveOperation();
                await command.ExecuteAsync().ConfigureAwait(false);
            }
            finally
            {
                this.EndExclusiveOperation();
            }
        }

        private void BeginExclusiveOperation()
        {
            lock (this.exclusiveOperationLock)
            {
                if (this.isExclusiveOperationRunning)
                {
                    throw new UnauthorizedAccessException(
                        "An exclusive operation is already running for this RGB LED Sequencer.");
                }

                this.isExclusiveOperationRunning = true;
            }
        }

        private void EndExclusiveOperation()
        {
            lock (this.exclusiveOperationLock)
            {
                this.isExclusiveOperationRunning = false;
            }
        }
    }
}
