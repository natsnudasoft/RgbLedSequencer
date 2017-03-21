// <copyright file="DotCorrectionDataCustomization.cs" company="natsnudasoft">
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

    public sealed class DotCorrectionDataCustomization : LedDotCorrectionCustomization
    {
        public DotCorrectionDataCustomization(
            Mock<IRgbLedSequencerConfiguration> sequencerConfigMock)
            : base(sequencerConfigMock)
        {
        }

        public int RgbLedCount { get; set; }

        public override void Customize(IFixture fixture)
        {
            System.Diagnostics.Debug.Assert(
                this.RgbLedCount >= 0,
                "RgbLedCount customization must be greater than or equal to 0.");
            FavorFactoryConstructor(fixture);
            this.SequencerConfigMock.Setup(c => c.RgbLedCount).Returns(this.RgbLedCount);
            fixture.Inject<ICollection<LedDotCorrection>>(
                fixture.CreateMany<LedDotCorrection>(this.RgbLedCount).ToArray());
        }

        private static void FavorFactoryConstructor(IFixture fixture)
        {
            fixture.Customize<DotCorrectionData>(c => c.FromFactory(
                new MethodInvoker(new ParameterTypeFavoringConstructorQuery(
                    typeof(IRgbLedSequencerConfiguration),
                    typeof(Func<LedDotCorrection>)))));
        }
    }
}
