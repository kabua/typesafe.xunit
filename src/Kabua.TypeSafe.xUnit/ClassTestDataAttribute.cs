using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit;

/// <summary>
/// Provides a data source for a data theory, with the data coming from a class
/// which must implement IEnumerable&lt;object[]&gt;.
/// Caution: the property is completely enumerated by .ToList() before any test is run. Hence it should return independent object sets.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ClassTestDataAttribute : DataAttribute
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public ClassTestDataAttribute(Type @class, Type testData)
    {
        Class = @class;
        TestData = testData;
    }

    /// <summary>
    /// Gets the type of the class that provides the data.
    /// </summary>
    public Type Class { get; private set; }

    /// <summary>
    /// Gets the type of the data.
    /// </summary>
    public Type TestData { get; private set; }

    /// <inheritdoc/>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (Activator.CreateInstance(Class) is not IEnumerable data)
            throw new ArgumentException($"{Class.FullName} must implement IEnumerable<{TestData}> to be used as ClassTestData(typeof({Class.Name}), typeof({TestData.Name})) for the test method named '{testMethod.Name}' on {testMethod.DeclaringType!.FullName}");

        foreach (object obj in data)
        {
            if (obj.GetType() != TestData)
            {
                throw new ArgumentException($"{Class.FullName} must implement IEnumerable<{TestData}> to be used as ClassTestData(typeof({Class.Name}), typeof({TestData.Name})) for the test method named '{testMethod.Name}' on {testMethod.DeclaringType!.FullName}");
            }

            yield return new []{ obj };
        }
    }
}

#if NET6_0_OR_GREATER
/// <summary>
/// Provides a data source for a data theory, with the data coming from a class
/// which must implement IEnumerable&lt;object[]&gt;.
/// Caution: the property is completely enumerated by .ToList() before any test is run. Hence it should return independent object sets.
/// </summary>
/// <typeparam name="TClass">Class type that contains the test data</typeparam>
/// <typeparam name="TData">The type of test data</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ClassTestDataAttribute<TClass, TData> : ClassTestDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public ClassTestDataAttribute()
        : base(typeof(TClass), typeof(TData))
    {
    }

    /// <inheritdoc/>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (Activator.CreateInstance(Class) is not IEnumerable<TData> data)
            throw new ArgumentException($"{Class.FullName} must implement IEnumerable<{typeof(TData).Name}> to be used as ClassData<{Class.Name}, {typeof(TData).Name}> for the test method named '{testMethod.Name}' on {testMethod.DeclaringType!.FullName}");

        return data.Select(d => new object[]{ d! });
    }
}
#endif