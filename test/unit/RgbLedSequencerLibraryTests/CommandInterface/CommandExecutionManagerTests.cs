// <copyright file="CommandExecutionManagerTests.cs" company="natsnudasoft">
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

namespace RgbLedSequencerLibraryTests.CommandInterface
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Extension;
    using Moq;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary.CommandInterface;
    using Xunit;

    public sealed class CommandExecutionManagerTests
    {
        private static readonly Type SutType = typeof(CommandExecutionManager);

        [Theory]
        [AutoMoqData]
        public void ExecuteHasCorrectGuardClauses(Fixture fixture)
        {
            var assertion = new GuardClauseAssertion(
                fixture,
                new ParameterNullReferenceBehaviourExpectation(fixture));

            assertion.Verify(
                SutType.GetMethod(nameof(CommandExecutionManager.ExecuteCommandAsync)));
        }

        [Theory]
        [AutoMoqData]
        public async Task ExecuteSingleCommandExecutesCommandAsync(
            [Frozen]Mock<IAsyncCommand> commandMock,
            CommandExecutionManager sut)
        {
            commandMock.Setup(c => c.ExecuteAsync()).Returns(Task.CompletedTask);

            await sut.ExecuteCommandAsync(commandMock.Object).ConfigureAwait(false);

            commandMock.Verify(c => c.ExecuteAsync(), Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task ExecuteMultipleCommandsThrowsAsync(
            [Frozen]Mock<IAsyncCommand> commandMock,
            CommandExecutionManager sut)
        {
            const int Timeout = 1000;
            using (var cts = new CancellationTokenSource())
            {
                commandMock.Setup(c => c.ExecuteAsync()).Returns(WaitForCancelAsync(cts.Token));

                var task1 = sut.ExecuteCommandAsync(commandMock.Object);
                var task2 = sut.ExecuteCommandAsync(commandMock.Object);
                cts.CancelAfter(Timeout);
                var completedTask = await Task.WhenAny(task1, task2).ConfigureAwait(false);
                var ex = await Record.ExceptionAsync(() => completedTask).ConfigureAwait(false);

                Assert.IsType<UnauthorizedAccessException>(ex);
            }
        }

        private static async Task WaitForCancelAsync(CancellationToken cancellationToken)
        {
            const int DelayTime = 100;
            await Task.Yield();
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(DelayTime).ConfigureAwait(false);
            }
        }
    }
}
