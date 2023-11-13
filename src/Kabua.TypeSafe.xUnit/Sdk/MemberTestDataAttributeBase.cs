using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Kabua.TypeSafe.xUnit.Sdk;

/// <summary>
/// Provides a base class for attributes that will provide member data. The member data must return
/// something compatible with <see cref="IEnumerable"/>.
/// Caution: the property is completely enumerated by .ToList() before any test is run. Hence it should return independent object sets.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class MemberTestDataAttributeBase : MemberDataAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemberDataAttributeBase"/> class.
    /// </summary>
    /// <param name="memberName">The name of the public static member on the test class that will provide the test data</param>
    /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else)</param>
    protected MemberTestDataAttributeBase(string memberName, object[] parameters) : base(memberName, parameters)
    {
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null)
            throw new ArgumentNullException(nameof(testMethod));

        var type = MemberType ?? testMethod.DeclaringType;
        var accessor = GetPropertyAccessor(type) ?? GetFieldAccessor(type) ?? GetMethodAccessor(type);
        if (accessor == null)
        {
            var parameterText = Parameters?.Length > 0 ? $" with parameter types: {string.Join(", ", Parameters.Select(p => p?.GetType().FullName ?? "(null)"))}" : "";
            throw new ArgumentException($"Could not find public static member (property, field, or method) named '{MemberName}' on {type?.FullName}{parameterText}");
        }

        var obj = accessor();
        if (obj == null)
            return null!;

        if (obj is not IEnumerable dataItems)
            throw new ArgumentException($"Property {MemberName} on {type?.FullName} did not return IEnumerable");

        return dataItems.Cast<object>().Select(item => ConvertDataItem(testMethod, item));
    }

    private Func<object?>? GetPropertyAccessor(Type? type)
    {
        PropertyInfo? propInfo = null;
        for (var reflectionType = type; reflectionType != null; reflectionType = reflectionType.GetTypeInfo().BaseType)
        {
            propInfo = reflectionType.GetProperty(MemberName, DefaultBindingFlags);

            if (propInfo != null)
                break;
        }

        if (propInfo == null || propInfo.GetMethod == null || !propInfo.GetMethod.IsStatic)
            return null;

        return () => propInfo.GetValue(null, null);
    }

    private Func<object?>? GetFieldAccessor(Type? type)
    {
        FieldInfo? fieldInfo = null;
        for (var reflectionType = type; reflectionType != null; reflectionType = reflectionType.GetTypeInfo().BaseType)
        {
            fieldInfo = reflectionType.GetField(MemberName, DefaultBindingFlags);
            if (fieldInfo != null)
                break;
        }

        if (fieldInfo == null || !fieldInfo.IsStatic)
            return null;

        return () => fieldInfo.GetValue(null);
    }

    private Func<object?>? GetMethodAccessor(Type? type)
    {
        MethodInfo? methodInfo = null;
        var parameterTypes = Parameters == null ? Type.EmptyTypes : Parameters.Select(p => p?.GetType()).ToArray();
        for (var reflectionType = type; reflectionType != null; reflectionType = reflectionType.GetTypeInfo().BaseType)
        {
            methodInfo = reflectionType.GetRuntimeMethods()
               .FirstOrDefault(m => m.Name == MemberName && ParameterTypesCompatible(m.GetParameters(), parameterTypes));
            if (methodInfo != null)
                break;
        }

        if (methodInfo == null || !methodInfo.IsStatic)
            return null;

        return () => methodInfo.Invoke(null, Parameters);
    }

    private static bool ParameterTypesCompatible(ParameterInfo[] parameters, Type?[] parameterTypes)
    {
        if (parameters?.Length != parameterTypes.Length)
            return false;

        return !parameters.Where((t, idx) => parameterTypes[idx] != null && !t.ParameterType.GetTypeInfo().IsAssignableFrom(parameterTypes[idx]!.GetTypeInfo())).Any();
    }

    private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
}