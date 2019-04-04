// <copyright file="SequenceDataCustomization.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerLibraryTests.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using NatsnudaLibrary.TestExtensions;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Kernel;
    using RgbLedSequencerLibrary;

    public sealed class SequenceDataCustomization : SequenceStepCustomization
    {
        public SequenceDataCustomization(Mock<IRgbLedSequencerConfiguration> sequencerConfigMock)
            : base(sequencerConfigMock)
        {
        }

        public int MaxStepCount { get; set; }

        public int? StepCount { get; set; }

        public override void Customize(IFixture fixture)
        {
            base.Customize(fixture);
            System.Diagnostics.Debug.Assert(
                !this.StepCount.HasValue ||
                    (this.StepCount.Value >= 0 && this.StepCount.Value <= this.MaxStepCount),
                "StepCount customization must be positive and less than or equal to MaxStepCount.");
            FavorStepCountConstructor(fixture);
            this.SequencerConfigMock.Setup(c => c.MaxStepCount).Returns(this.MaxStepCount);

            if (this.StepCount.HasValue)
            {
                fixture.Inject<ICollection<SequenceStep>>(
                    fixture.CreateMany<SequenceStep>(this.StepCount.Value).ToArray());
                ApplyStepCountParameterSpecimen(fixture, this.StepCount.Value);
            }
        }

        private static void FavorStepCountConstructor(IFixture fixture)
        {
            fixture.Customize<SequenceData>(c => c.FromFactory(
                new MethodInvoker(new ParameterTypeFavoringConstructorQuery(
                    typeof(IRgbLedSequencerConfiguration),
                    typeof(Func<SequenceStep>),
                    typeof(int)))));
        }

        private static void ApplyStepCountParameterSpecimen(IFixture fixture, int stepCount)
        {
            fixture.Customizations.Add(new ParameterSpecimenBuilder(
                typeof(SequenceData),
                nameof(stepCount),
                stepCount));
        }
    }
}