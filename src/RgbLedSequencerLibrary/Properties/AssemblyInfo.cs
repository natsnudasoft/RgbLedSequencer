// <copyright file="AssemblyInfo.cs" company="natsnudasoft">
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

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(true)]

// Internals visible to unit testing and Moq proxy.
#pragma warning disable MEN002 // Line is too long
[assembly: InternalsVisibleTo("RgbLedSequencerLibraryTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c39abccdeb8d139e8a29fb5dd3955a660fcfa1c411372fc1eee8c3a70020bd00a0bbf786f71d127b19124c7fe18129adf54c52b274e4aa4e718dd7c760bead52c90163e6e22e2cab4122f122c3db8f2557fdefa572751627a9829f3b8e2a8043313980a6bce709aec2bebbdee7f177915bdad196743e6cc0b2ad764158bacdae")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
#pragma warning restore MEN002 // Line is too long

[assembly: AssemblyTitle("RgbLedSequencerLibrary")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("RgbLedSequencerLibrary")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("e1ee2bb5-34e1-45d8-ae84-a10f9251ef1b")]
