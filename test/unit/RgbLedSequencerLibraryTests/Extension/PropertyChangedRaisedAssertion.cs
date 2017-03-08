// <copyright file="PropertyChangedRaisedAssertion.cs" company="natsnudasoft">
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Kernel;
    using RgbLedSequencerLibrary;

    /// <summary>
    /// Encapsulates a unit test that verifies whether properties
    /// </summary>
    /// <seealso cref="IdiomaticAssertion" />
    public sealed class PropertyChangedRaisedAssertion : IdiomaticAssertion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedRaisedAssertion"/> class.
        /// </summary>
        /// <param name="fixture">The anonymous object creation service to use to create new
        /// specimens.</param>
        public PropertyChangedRaisedAssertion(IFixture fixture)
        {
            ParameterValidation.IsNotNull(fixture, nameof(fixture));

            this.Fixture = fixture;
        }

        /// <summary>
        /// Gets the anonymous object creation service.
        /// </summary>
        public IFixture Fixture { get; }

        /// <inheritdoc/>
        /// <summary>
        /// Verifies that the specified property raises the PropertyChanged event.
        /// </summary>
        /// <remarks>
        /// This method does nothing if the property does not have a public set method, or if no
        /// public PropertyChanged event with the correct signature was found.
        /// </remarks>
        public override void Verify(PropertyInfo propertyInfo)
        {
            ParameterValidation.IsNotNull(propertyInfo, nameof(propertyInfo));

            var propertyChangedEventInfo = propertyInfo.ReflectedType.GetEvent("PropertyChanged");

            if (propertyChangedEventInfo != null && propertyInfo.GetSetMethod() != null &&
                propertyChangedEventInfo.EventHandlerType == typeof(PropertyChangedEventHandler) &&
                propertyChangedEventInfo.GetAddMethod() != null &&
                propertyChangedEventInfo.GetRemoveMethod() != null)
            {
                var receivedEvents = new List<string>();
                Action<object, PropertyChangedEventArgs> handler = (sender, e) =>
                {
                    receivedEvents.Add(e.PropertyName);
                };
                var handlerDelegate = Delegate.CreateDelegate(
                    propertyChangedEventInfo.EventHandlerType,
                    handler.Target,
                    handler.Method);
                var context = new SpecimenContext(this.Fixture);
                var specimen = context.Resolve(propertyInfo.ReflectedType);
                var initialPropertyValue = propertyInfo.GetValue(specimen);
                object newPropertyValue;
                do
                {
                    newPropertyValue = context.Resolve(propertyInfo.PropertyType);
                }
                while (newPropertyValue == initialPropertyValue);

                try
                {
                    propertyChangedEventInfo.AddEventHandler(specimen, handlerDelegate);
                    propertyInfo.SetValue(specimen, newPropertyValue);
                }
                finally
                {
                    propertyChangedEventInfo.RemoveEventHandler(specimen, handlerDelegate);
                }

                if (receivedEvents.Count != 1 || receivedEvents[0] != propertyInfo.Name)
                {
                    throw new PropertyChangedRaisedException(propertyInfo);
                }
            }
        }
    }
}
