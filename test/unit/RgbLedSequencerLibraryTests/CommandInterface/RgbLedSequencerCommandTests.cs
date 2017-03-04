// <copyright file="RgbLedSequencerCommandTests.cs" company="natsnudasoft">
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
    using System.Threading.Tasks;
    using Extension;
    using Moq;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Kernel;
    using Ploeh.AutoFixture.Xunit2;
    using RgbLedSequencerLibrary;
    using RgbLedSequencerLibrary.CommandInterface;
    using Xunit;

    public sealed class RgbLedSequencerCommandTests
    {
        private static readonly Type SutType = typeof(RgbLedSequencerCommandStub);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            ConstructorInitializedMemberAssertion assertion)
        {
            assertion.Verify(
                SutType.GetProperty(nameof(RgbLedSequencerCommandStub.SequencerConfig)),
                SutType.GetProperty(nameof(RgbLedSequencerCommandStub.RgbLedSequencer)));
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ReportProgressHasCorrectGuardClauses(Fixture fixture)
        {
            ApplyProgressPercentageSpecimen(fixture, 50d);
            var assertion = new GuardClauseAssertion(
                fixture,
                new ParameterNullReferenceBehaviourExpectation(fixture));

            assertion.Verify(SutType.GetMethod(nameof(RgbLedSequencerCommandStub.ReportProgress)));
        }

        [Theory]
        [AutoMoqData]
        public void ReportProgressCallsReport(
            [Frozen, Parameter("progressPercentage", 50d)]CommandProgress commandProgress,
            [Frozen]Mock<IProgress<CommandProgress>> progressMock,
            Fixture fixture)
        {
            fixture.Customize<RgbLedSequencerCommandStub>(
                c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));
            var sut = fixture.Create<RgbLedSequencerCommandStub>();

            sut.ReportProgress(commandProgress);

            progressMock.Verify(p => p.Report(commandProgress), Times.Once());
        }

        private static void ApplyProgressPercentageSpecimen(
            IFixture fixture,
            double progressPercentage)
        {
            fixture.Customizations.Add(new ParameterSpecimenBuilder(
                typeof(CommandProgress),
                nameof(progressPercentage),
                progressPercentage));
        }

        private sealed class RgbLedSequencerCommandStub : RgbLedSequencerCommand
        {
            public RgbLedSequencerCommandStub(
                IRgbLedSequencerConfiguration sequencerConfig,
                IRgbLedSequencer rgbLedSequencer)
                : base(sequencerConfig, rgbLedSequencer)
            {
            }

            public RgbLedSequencerCommandStub(
                IRgbLedSequencerConfiguration sequencerConfig,
                IRgbLedSequencer rgbLedSequencer,
                IProgress<CommandProgress> progress)
                : base(sequencerConfig, rgbLedSequencer, progress)
            {
            }

            public new IRgbLedSequencerConfiguration SequencerConfig
            {
                get { return base.SequencerConfig; }
            }

            public new IRgbLedSequencer RgbLedSequencer
            {
                get { return base.RgbLedSequencer; }
            }

            public override Task ExecuteAsync()
            {
                return Task.CompletedTask;
            }

            public new void ReportProgress(CommandProgress commandProgress)
            {
                base.ReportProgress(commandProgress);
            }
        }
    }
}
