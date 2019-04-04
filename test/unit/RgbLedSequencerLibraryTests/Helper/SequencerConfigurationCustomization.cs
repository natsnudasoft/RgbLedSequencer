// <copyright file="SequencerConfigurationCustomization.cs" company="natsnudasoft">
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
    using Moq;
    using NatsnudaLibrary;
    using Ploeh.AutoFixture;
    using RgbLedSequencerLibrary;

    public abstract class SequencerConfigurationCustomization : ICustomization
    {
        protected SequencerConfigurationCustomization(
            Mock<IRgbLedSequencerConfiguration> sequencerConfigMock)
        {
            ParameterValidation.IsNotNull(sequencerConfigMock, nameof(sequencerConfigMock));

            this.SequencerConfigMock = sequencerConfigMock;
        }

        public Mock<IRgbLedSequencerConfiguration> SequencerConfigMock { get; }

        public abstract void Customize(IFixture fixture);
    }
}