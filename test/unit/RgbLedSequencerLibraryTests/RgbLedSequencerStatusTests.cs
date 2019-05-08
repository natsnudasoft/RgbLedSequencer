// <copyright file="RgbLedSequencerStatusTests.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerLibraryTests
{
    using System;
    using System.Linq;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Natsnudasoft.NatsnudaLibrary.TestExtensions;
    using Xunit;
    using SutAlias = Natsnudasoft.RgbLedSequencerLibrary.RgbLedSequencerStatus;

    public class RgbLedSequencerStatusTests
    {
        private static readonly Type SutType = typeof(SutAlias);

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            ConstructorInitializedMemberAssertion assertion)
        {
            assertion.Verify(
                SutType.GetProperty(nameof(SutAlias.IsAsleep)),
                SutType.GetProperty(nameof(SutAlias.CurrentSequenceIndex)));
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void DebuggerDisplayDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetProperty(nameof(SutAlias.DebuggerDisplay)));
        }

        [Theory]
        [AutoMoqData]
        public void EqualsOperatorOverloadCorrectlyImplemented(Fixture fixture)
        {
            var isAsleep = fixture.Create<bool>();
            var currentSequenceIndex = fixture.Create<byte>();
            var sut1 = new SutAlias(isAsleep, currentSequenceIndex);
            var sut2 = new SutAlias(isAsleep, currentSequenceIndex);

            var result = sut1 == sut2;

            Assert.True(result);
        }

        [Theory]
        [AutoMoqData]
        public void NotEqualsOperatorOverloadCorrectlyImplemented(Fixture fixture)
        {
            var isAsleep = fixture.Create<bool>();
            var currentSequenceIndex = fixture.Create<byte>();
            var isAsleep2 = !isAsleep;
            var currentSequenceIndex2 = unchecked((byte)(currentSequenceIndex + 1));
            var sut1 = new SutAlias(isAsleep, currentSequenceIndex);
            var sut2 = new SutAlias(isAsleep2, currentSequenceIndex2);

            var result = sut1 != sut2;

            Assert.True(result);
        }

        [Theory]
        [AutoMoqData]
        public void EqualsOverrideCorrectlyImplemented(
            EqualsOverrideNewObjectAssertion equalsNewObjectAssertion,
            EqualsOverrideNullAssertion equalsOverrideNullAssertion,
            EqualsOverrideOtherSuccessiveAssertion equalsOverrideOtherSuccessiveAssertion,
            EqualsOverrideSelfAssertion equalsOverrideSelfAssertion)
        {
            var equalsMethods = SutType.GetMethods().Where(m => m.Name == nameof(SutAlias.Equals));

            equalsNewObjectAssertion.Verify(equalsMethods);
            equalsOverrideNullAssertion.Verify(equalsMethods);
            equalsOverrideOtherSuccessiveAssertion.Verify(equalsMethods);
            equalsOverrideSelfAssertion.Verify(equalsMethods);
        }

        [Theory]
        [AutoMoqData]
        public void GetHashCodeOverrideCorrectlyImplemented(
            GetHashCodeSuccessiveAssertion assertion)
        {
            assertion.Verify(SutType.GetMethod(nameof(SutAlias.GetHashCode)));
        }
    }
}