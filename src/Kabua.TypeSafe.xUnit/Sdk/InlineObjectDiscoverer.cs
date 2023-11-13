using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit.Sdk;

public class InlineObjectDiscoverer : IDataDiscoverer
{
    /// <inheritdoc/>
    public virtual IEnumerable<object[]>? GetData(IAttributeInfo dataAttribute, IMethodInfo testMethod)
    {
        if (dataAttribute is IReflectionAttributeInfo reflectionDataAttribute && testMethod is IReflectionMethodInfo reflectionTestMethod)
        {
            var attribute = (DataAttribute) reflectionDataAttribute.Attribute;
            try
            {
                return attribute.GetData(reflectionTestMethod.MethodInfo);
            }
            catch (ArgumentException)
            {
                // If we couldn't find the data on the base type, check if it is in current type.
                // This allows base classes to specify data that exists on a sub type, but not on the base type.
                var reflectionTestMethodType = reflectionTestMethod.Type as IReflectionTypeInfo;
                if (attribute is MemberDataAttribute { MemberType: null } memberDataAttribute)
                {
                    memberDataAttribute.MemberType = reflectionTestMethodType!.Type;
                }

                return attribute.GetData(reflectionTestMethod.MethodInfo);
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public virtual bool SupportsDiscoveryEnumeration(IAttributeInfo dataAttribute, IMethodInfo testMethod) 
        => !dataAttribute.GetNamedArgument<bool>("DisableDiscoveryEnumeration");
}