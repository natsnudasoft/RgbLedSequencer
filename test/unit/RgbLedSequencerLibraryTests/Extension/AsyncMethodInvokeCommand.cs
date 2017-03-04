// <copyright file="AsyncMethodInvokeCommand.cs" company="natsnudasoft">
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

namespace RgbLedSequencerLibraryTests.Extension
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Kernel;

    /// <summary>
    /// Extends the <see cref="MethodInvokeCommand"/> with a class capable of handling async
    /// methods.
    /// </summary>
    /// <seealso cref="MethodInvokeCommand" />
    /// <seealso cref="IGuardClauseCommand" />
    public sealed class AsyncMethodInvokeCommand : MethodInvokeCommand, IGuardClauseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncMethodInvokeCommand"/> class.
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="expansion">An expansion which is used to transform the single value in the
        /// Execute method into an appropriate number of input arguments for the method.</param>
        /// <param name="parameterInfo">The parameter that is being sent to this method.</param>
        public AsyncMethodInvokeCommand(
            IMethod method,
            IExpansion<object> expansion,
            ParameterInfo parameterInfo)
            : base(method, expansion, parameterInfo)
        {
        }

        /// <inheritdoc/>
        void IGuardClauseCommand.Execute(object value)
        {
            var instanceMethod = this.Method as InstanceMethod;
            var staticMethod = this.Method as StaticMethod;
            Task returnedTask;
            if (instanceMethod != null)
            {
                returnedTask = instanceMethod.Method.Invoke(
                    instanceMethod.Owner,
                    this.Expansion.Expand(value).ToArray()) as Task;
            }
            else if (staticMethod != null)
            {
                returnedTask = staticMethod.Method.Invoke(
                    null,
                    this.Expansion.Expand(value).ToArray()) as Task;
            }
            else
            {
                this.Execute(value);
                returnedTask = Task.CompletedTask;
            }

            if (returnedTask == null)
            {
                throw new GuardClauseException("Async method did not return a Task.");
            }

            Exception ex = null;
            var thread = new Thread(() => RunTask(returnedTask, out ex));
            thread.Start();
            thread.Join();

            if (ex != null)
            {
                throw ex;
            }
        }

        private static void RunTask(Task task, out Exception caughtException)
        {
            caughtException = null;
            try
            {
                task.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
        }
    }
}
