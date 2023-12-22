using System.Reflection;
using Kabua.TypeSafe.xUnit.UnitTests.F04_MemberTestData_real_world_example_objects;

namespace Kabua.TypeSafe.xUnit.UnitTests.F07_Combined_real_world_example_objects;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class BuilderTestDataAttribute : InlineObjectAttribute
{
    public BuilderTestDataAttribute(string pattern)
        : this(pattern, pattern)
    {
    }

    public BuilderTestDataAttribute(string pattern, string expectedValue)
        : this(pattern, new[] { expectedValue })
    {
    }

    public BuilderTestDataAttribute(string pattern, params string[] expectedValues)
    {
        Pattern = pattern;
        ExpectedValues = expectedValues;
    }

    public bool UsePatternSize { get; set; }

    public string Pattern { get; }

    public string[] ExpectedValues { get; }

    public Type[] Types { get; set; } = { };

    protected override object GetObject(MemberInfo testMethod) 
    {
        var itemCount = Math.Max(ExpectedValues.Length, Types.Length);

        var expectedValues = new List<ExpectedTestData>();

        for (int i = 0; i < itemCount; i++)
        {
            var value = i < ExpectedValues.Length ? ExpectedValues[i] : string.Empty;
            var type = i < Types.Length ? Types[i] : null;
            var expected = new ExpectedTestData(value, UsePatternSize ? Pattern.Length : value.Length, type);

            expectedValues.Add(expected);
        }

        var data = new BuilderTestData(Pattern)
        {
            ExpectedValues = expectedValues.ToList()
        };

        return data;
    }
}