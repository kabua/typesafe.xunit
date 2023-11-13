using System;
using Kabua.TypeSafe.xUnit.Sdk;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit;

/// <summary>
/// Provides a data source for a data theory, with the data coming from one of the following sources:
/// 1. A static property
/// 2. A static field
/// 3. A static method (with parameters)
/// The member must return something compatible with IEnumerable&lt;object[]&gt; with the test data.
/// Caution: the property is completely enumerated by .ToList() before any test is run. Hence it should return independent object sets.
/// </summary>
[DataDiscoverer("Xunit.Sdk.MemberDataDiscoverer", "xunit.core")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class MemberTestDataAttribute : MemberTestDataAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="memberName"/> class.
    /// </summary>
    /// <param name="testData">The type of the per data item</param>
    /// <param name="memberName">The name of the public static member on the test class that will provide the test data</param>
    /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else)</param>
    public MemberTestDataAttribute(Type testData, string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        TestData = testData;
    }

    /// <summary>
    /// Gets the type of the data.
    /// </summary>
    public Type TestData { get; private set; }

    /// <inheritdoc/>
    protected override object[] ConvertDataItem(MethodInfo testMethod, object? obj)
    {
        if (obj == null)
            return null!;

        if (obj.GetType() == TestData)
        {
            return new[] { obj };
        }

        if (obj is Array array)
        {
            var itemType = array.GetType().GetElementType();

            if (itemType == TestData)
            {
                return (object[]) obj;
            }
        }

        throw new ArgumentException($"Property {MemberName} on {MemberType ?? testMethod.DeclaringType} yielded an item that is not an {TestData.Name}");
    }
}

#if NET6_0_OR_GREATER
/// <summary>
/// Provides a data source for a data theory, with the data coming from one of the following sources:
/// 1. A static property
/// 2. A static field
/// 3. A static method (with parameters)
/// The member must return something compatible with IEnumerable&lt;object[]&gt; with the test data.
/// Caution: the property is completely enumerated by .ToList() before any test is run. Hence it should return independent object sets.
/// </summary>
[DataDiscoverer("Xunit.Sdk.MemberDataDiscoverer", "xunit.core")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class MemberTestDataAttribute<T> : MemberTestDataAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="memberName"/> class.
    /// </summary>
    /// <param name="memberName">The name of the public static member on the test class that will provide the test data</param>
    /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else)</param>
    public MemberTestDataAttribute(string memberName, params object[] parameters)
        : base(memberName, parameters) { }

    /// <inheritdoc/>
    protected override object[] ConvertDataItem(MethodInfo testMethod, object? obj)
    {
        if (obj == null)
            return null!;

        if (obj is T)
        {
            return new[] { obj };
        }

        if (obj is T[])
        {
            return (object[]) obj;
        }

        throw new ArgumentException($"Property {MemberName} on {MemberType ?? testMethod.DeclaringType} yielded an item that is not an {typeof(T).Name}");
    }
}
#endif