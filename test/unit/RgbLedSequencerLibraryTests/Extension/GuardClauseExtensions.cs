// <copyright file="GuardClauseExtensions.cs" company="natsnudasoft">
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
    using System.Runtime.CompilerServices;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Kernel;
    using RgbLedSequencerLibrary;

    /// <summary>
    /// Provides methods to extend the <see cref="GuardClauseAssertion"/> class.
    /// </summary>
    public static class GuardClauseExtensions
    {
        private static readonly Type TaskReturnType = typeof(GuardClauseAssertion).Assembly.GetType(
            "Ploeh.AutoFixture.Idioms.GuardClauseAssertion+TaskReturnMethodInvokeCommand");

        /// <summary>
        /// Creates a new <see cref="IGuardClauseCommand"/> from the specified
        /// <see cref="IGuardClauseCommand"/> that replaces the expansion with one that uses
        /// <see cref="ParameterInfo"/> instead of <see cref="Type"/> and supports async method.
        /// </summary>
        /// <param name="fixture">The anonymous object creation service used to workaround
        /// a problem with guard clause assertions.</param>
        /// <param name="command">The <see cref="IGuardClauseCommand"/> that will be executed
        /// as part of the assertion.</param>
        /// <returns>A new instance of <see cref="IGuardClauseCommand"/> with the newly supplied
        /// features.</returns>
        public static IGuardClauseCommand CreateExtendedCommand(
            IFixture fixture,
            IGuardClauseCommand command)
        {
            ParameterValidation.IsNotNull(command, nameof(command));

            var methodInvokeCommand = GetMethodInvokeCommand(command);
            IGuardClauseCommand commandToExecute;
            var expansion = methodInvokeCommand?.Expansion as IndexedReplacement<object>;
            if (methodInvokeCommand != null && expansion != null)
            {
                var instanceMethod = methodInvokeCommand.Method as InstanceMethod;
                var staticMethod = methodInvokeCommand.Method as StaticMethod;
                var asyncAttribute =
                    instanceMethod?.Method?.GetCustomAttribute<AsyncStateMachineAttribute>(false) ??
                    staticMethod?.Method?.GetCustomAttribute<AsyncStateMachineAttribute>(false);
                if (asyncAttribute != null)
                {
                    commandToExecute =
                        new ReflectionExceptionUnwrappingCommand(new AsyncMethodInvokeCommand(
                            methodInvokeCommand.Method,
                            RecreateExpansion(fixture, methodInvokeCommand.Method, expansion),
                            methodInvokeCommand.ParameterInfo));
                }
                else
                {
                    commandToExecute =
                        new ReflectionExceptionUnwrappingCommand(new MethodInvokeCommand(
                            methodInvokeCommand.Method,
                            RecreateExpansion(fixture, methodInvokeCommand.Method, expansion),
                            methodInvokeCommand.ParameterInfo));
                }
            }
            else
            {
                commandToExecute = command;
            }

            return commandToExecute;
        }

        /// <summary>
        /// Attempts to retrieve an instance of <see cref="MethodInvokeCommand"/> from an instance
        /// of <see cref="IGuardClauseCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="IGuardClauseCommand"/> to attempt to extract an
        /// instance of <see cref="MethodInvokeCommand"/> from.</param>
        /// <returns>An instance of <see cref="MethodInvokeCommand"/> if one was found; otherwise
        /// <see langword="null"/>.</returns>
        /// <exception cref="ArgumentException">The underlying command could not be extracted via
        /// reflection.</exception>
        public static MethodInvokeCommand GetMethodInvokeCommand(IGuardClauseCommand command)
        {
            var methodInvokeCommand =
                (command as ReflectionExceptionUnwrappingCommand)?.Command as MethodInvokeCommand;

            if (methodInvokeCommand == null && command.GetType() == TaskReturnType)
            {
                var newCommand = TaskReturnType.GetField(
                    nameof(command),
                    BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(command)
                    as IGuardClauseCommand;

                if (newCommand == null)
                {
                    throw new ArgumentException(
                        "Base command could not be extracted from Task command.",
                        nameof(command));
                }

                methodInvokeCommand = (newCommand as ReflectionExceptionUnwrappingCommand)?.Command
                    as MethodInvokeCommand;
            }

            return methodInvokeCommand;
        }

        private static IndexedReplacement<object> RecreateExpansion(
            IFixture fixture,
            IMethod method,
            IndexedReplacement<object> oldExpansion)
        {
            var context = new SpecimenContext(fixture);
            return new IndexedReplacement<object>(
                oldExpansion.ReplacementIndex,
                method.Parameters.Select(p => context.Resolve(p)));
        }
    }
}
