// <copyright file="CommandProgress.cs" company="natsnudasoft">
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

    /// <summary>
    /// Represents the progress of a task of the RGB LED Sequencer.
    /// </summary>
    public sealed class CommandProgress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProgress"/> class.
        /// </summary>
        /// <param name="progressPercentage">The current progress percentage of the task this
        /// <see cref="CommandProgress"/> is representing.</param>
        /// <param name="currentAction">The current action being performed in the task this
        /// <see cref="CommandProgress"/> is representing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="currentAction"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="currentAction"/> is empty.
        /// </exception>
        public CommandProgress(double progressPercentage, string currentAction)
        {
            ParameterValidation
                .IsBetweenInclusive(progressPercentage, 0, 100, nameof(progressPercentage));
            ParameterValidation.IsNotNull(currentAction, nameof(currentAction));
            ParameterValidation.IsNotEmpty(currentAction, nameof(currentAction));

            this.ProgressPercentage = progressPercentage;
            this.CurrentAction = currentAction;
        }

        /// <summary>
        /// Gets the current progress percentage of the task this <see cref="CommandProgress"/> is
        /// representing.
        /// </summary>
        public double ProgressPercentage { get; }

        /// <summary>
        /// Gets the current action being performed in the task this <see cref="CommandProgress"/>
        /// is representing.
        /// </summary>
        public string CurrentAction { get; }
    }
}
