using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit.Sdk;

/// <summary>
/// Provides a data source for a object theory, with the object coming from inline values.
/// </summary>
[DataDiscoverer("Kabua.TypeSafe.xUnit.Sdk.InlineObjectDiscoverer", "Kabua.TypeSafe.xUnit")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class InlineObjectAttribute : DataAttribute
{
    public bool DisableDiscoveryEnumeration { get; protected set; }

    /// <summary>
    /// Create the test object
    /// </summary>
    /// <param name="testMethod"></param>
    /// <returns></returns>
    protected abstract object GetObject(MemberInfo testMethod);

    /// <inheritdoc/>
    public sealed override IEnumerable<object[]> GetData(MethodInfo testMethod) =>

        // This is called by the WPA81 version as it does not have access to attribute ctor params
        new[] { new[] { GetObject(testMethod) } };
}

#if NET6_0_OR_GREATER
/// <summary>
/// Provides a data source for a object theory, with the object coming from inline values.
/// </summary>
[DataDiscoverer("Kabua.TypeSafe.xUnit.Sdk.InlineObjectDiscoverer", "Kabua.TypeSafe.xUnit")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class InlineObjectAttribute<TObject> : DataAttribute
{
    public bool DisableDiscoveryEnumeration { get; protected set; }

    /// <summary>
    /// Create the test object
    /// </summary>
    /// <param name="testMethod"></param>
    /// <returns></returns>
    protected abstract TObject GetObject(MemberInfo testMethod);

    /// <inheritdoc/>
    public sealed override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        // This is called by the WPA81 version as it does not have access to attribute ctor params
        return new[] { new [] { (object) GetObject(testMethod)! } };
    }
}
#endif