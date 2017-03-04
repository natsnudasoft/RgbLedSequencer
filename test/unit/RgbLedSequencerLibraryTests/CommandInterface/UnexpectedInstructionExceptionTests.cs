// <copyright file="UnexpectedInstructionExceptionTests.cs" company="natsnudasoft">
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
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Extension;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.SemanticComparison.Fluent;
    using RgbLedSequencerLibrary.CommandInterface;
    using Xunit;

    public sealed class UnexpectedInstructionExceptionTests
    {
        private static readonly Type SutType = typeof(UnexpectedInstructionException);

        [Theory]
        [AutoMoqData]
        public void ConstructorSetsCorrectInitializedMembers(
            ConstructorInitializedMemberAssertion assertion)
        {
            assertion.Verify(
                SutType.GetProperty(nameof(UnexpectedInstructionException.ExpectedInstruction)),
                SutType.GetProperty(nameof(UnexpectedInstructionException.ReceivedInstruction)));
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ExceptionSerializationCopiesAllMembers(
            ReceiveInstruction expectedInstruction,
            ReceiveInstruction receivedInstruction)
        {
            var sutExpected =
                new UnexpectedInstructionException(expectedInstruction, receivedInstruction);
            UnexpectedInstructionException sutActual;

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, sutExpected);
                memoryStream.Position = 0;
                sutActual =
                    (UnexpectedInstructionException)binaryFormatter.Deserialize(memoryStream);
            }

            Assert.NotSame(sutExpected, sutActual);
            sutActual
                .AsSource()
                .OfLikeness<UnexpectedInstructionException>()
                .Without(ex => ex.Data)
                .Without(ex => ex.HelpLink)
                .Without(ex => ex.HResult)
                .Without(ex => ex.Source)
                .Without(ex => ex.StackTrace)
                .Without(ex => ex.TargetSite)
                .ShouldEqual(sutExpected);
        }
    }
}
