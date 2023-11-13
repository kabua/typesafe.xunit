using System;
using Xunit.Abstractions;

namespace Kabua.TypeSafe.xUnit;

public record DisplayNameFormattingContext
{

    /// <summary>
    /// Defines a set of properties that might be used when formatting a DisplayName
    /// </summary>
    /// <param name="factAttribute">The <see cref="FormattedTheoryAttribute"/> that is associated with the test</param>
    /// <param name="testCase">The test data in question</param>
    /// <param name="method">The test method information</param>
    /// <param name="baseDisplayName">The display name as normally formatted by <see cref="xUnit"/>.</param>
    /// <param name="arguments">The current set of arguments that will be passed to the <paramref name="Method"/>.</param>
    /// <param name="genericTypes">Any Generic types that were detected.</param>
    /// <param name="testId">The current test number</param>
    public DisplayNameFormattingContext(IAttributeInfo factAttribute, ITestCase testCase, IMethodInfo method, string baseDisplayName,
        object?[] arguments, ITypeInfo[]? genericTypes, int? testId)
    {
        FactAttribute = factAttribute;
        TestCase = testCase;
        Method = method;
        BaseDisplayName = baseDisplayName;
        Arguments = arguments ?? Array.Empty<object?>();
        GenericTypes = genericTypes;
        TestId = testId;
    }

    public IAttributeInfo FactAttribute { get; }
    public ITestCase TestCase { get; }
    public IMethodInfo Method  { get; }
    public string BaseDisplayName  { get; }
    public object?[] Arguments  { get; }
    public ITypeInfo[]? GenericTypes  { get; }
    public int? TestId  { get; }

    /// <summary>
    /// Format the <paramref name="displayName"/> with the <see cref="TestId"/>, if defined, as defined by the <paramref name="testNumberDisplayNameFormat"/>.
    /// </summary>
    /// <param name="displayName">The display name that needs to be updated with the <see cref="TestId"/>.</param>
    /// <param name="testNumberDisplayNameFormat">The test number format string that will be used. Default: <c>"{0,3:D}) {1}"</c></param>
    /// <returns>The formatted display name.</returns>
    public string GetNumberedDisplayName(string displayName, string testNumberDisplayNameFormat = "{0,3:D}) {1}")
    {
        if (TestId.HasValue)
        {
            displayName = string.Format(testNumberDisplayNameFormat, TestId, displayName);
        }

        return displayName;
    }

    /// <summary>
    /// A thin wrapper which called <see cref="DisplayNameExtensions.GetDisplayNameWithArguments"/>
    /// </summary>
    /// <param name="displayName">The display name to use, if <c>null</c>, use <see cref="BaseDisplayName"/> instead. Default: <c>null</c>.</param>
    /// <returns>The formatted display name.</returns>
    public string GetDisplayNameWithArguments(string? displayName = null) 
        => Method.GetDisplayNameWithArguments(displayName ?? BaseDisplayName, Arguments, GenericTypes);

    /// <summary>
    /// A thin wrapper which called <see cref="DisplayNameExtensions.GetArgumentsWithNames"/>
    /// </summary>
    /// <param name="displayName">The display name to use. Default: <c>null</c>.</param>
    /// <returns>The formatted display name.</returns>
    public string GetArgumentsWithNames(string? displayName = null)
        => displayName != null ? $"{displayName} ({Method.GetArgumentsWithNames(Arguments)})" : Method.GetArgumentsWithNames(Arguments);

    /// <summary>
    /// A thin wrapper which called <see cref="DisplayNameExtensions.GetArgumentsWithoutNames"/>
    /// </summary>
    /// <param name="displayName">The display name to use. Default: <c>null</c>.</param>
    /// <returns>The formatted display name.</returns>
    public string GetArgumentsWithoutNames(string? displayName = null)
        => displayName != null ? $"{displayName} ({Method.GetArgumentsWithoutNames(Arguments)})" : Method.GetArgumentsWithoutNames(Arguments);
}