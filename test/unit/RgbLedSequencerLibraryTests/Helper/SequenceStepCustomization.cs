// <copyright file="SequenceStepCustomization.cs" company="natsnudasoft">
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
    using Moq;
    using NatsnudaLibrary.TestExtensions;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Dsl;
    using Ploeh.AutoFixture.Kernel;
    using RgbLedSequencerLibrary;

    public class SequenceStepCustomization : GrayscaleDataCustomization
    {
        public SequenceStepCustomization(Mock<IRgbLedSequencerConfiguration> sequencerConfigMock)
            : base(sequencerConfigMock)
        {
        }

        public int MaxStepDelay { get; set; }

        public int? StepDelay { get; set; }

        public override void Customize(IFixture fixture)
        {
            base.Customize(fixture);
            System.Diagnostics.Debug.Assert(
                !this.StepDelay.HasValue ||
                    (this.StepDelay.Value >= 0 && this.StepDelay.Value <= this.MaxStepDelay),
                "StepDelay customization must be positive and less than or equal to MaxStepDelay.");
            FavorFactoryConstructor(fixture, this.StepDelay);
            this.SequencerConfigMock.Setup(c => c.MaxStepDelay).Returns(this.MaxStepDelay);
        }

        private static void FavorFactoryConstructor(IFixture fixture, int? stepDelay)
        {
            Func<ICustomizationComposer<SequenceStep>, ISpecimenBuilder> customization =
                c =>
                {
                    var builder = c.FromFactory(new MethodInvoker(
                        new ParameterTypeFavoringConstructorQuery(
                            typeof(IRgbLedSequencerConfiguration),
                            typeof(Func<GrayscaleData>))));
                    if (stepDelay.HasValue)
                    {
                        builder = builder.With(s => s.StepDelay, stepDelay.Value);
                    }

                    return builder;
                };
            fixture.Customize(customization);
        }
    }
}
