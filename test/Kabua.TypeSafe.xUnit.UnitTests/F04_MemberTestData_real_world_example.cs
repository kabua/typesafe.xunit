using FluentAssertions;
using Kabua.TypeSafe.xUnit.UnitTests.F04_MemberTestData_real_world_example_objects;

namespace Kabua.TypeSafe.xUnit.UnitTests;

public class C41_MemberTestData_real_world_example
{
    private static List<BuilderTestData> BuildLiteralExpressionData => new()
    {
        // One expected value
        new (@"removeEndingSpaces\   ") { ExpectedValues = { new (@"removeEndingSpaces ", 21, typeof(LiteralExpressionFactory)) }},
        new (@"removeEndingSpaces\ \  ") { ExpectedValues = { new (@"removeEndingSpaces  ", 21, typeof(LiteralExpressionFactory)) }},

        // Two expected values..
        new (".**") { ExpectedValues = { new (".", 1, typeof(LiteralExpressionFactory)), new("*", 2) }},
        new (".**.") { ExpectedValues = { new (".", 1, typeof(LiteralExpressionFactory)), new("*.", 3) }},
        new (".*.*") { ExpectedValues = { new (".", 1, typeof(LiteralExpressionFactory)), new("*", 3) }},
    };


    [FormattedTheory]
#if NET6_0_OR_GREATER
    [MemberTestData<BuilderTestData>(nameof(BuildLiteralExpressionData))]
#else
    [MemberTestData(typeof(BuilderTestData), nameof(BuildLiteralExpressionData))]
#endif
    public void T01_BuildLiteralExpressionTests(BuilderTestData test)
    {
        test.Pattern.Should().NotBeNullOrWhiteSpace();
        test.ExpectedValues.Should().HaveCountGreaterThan(0);

        foreach (ExpectedTestData expectedValue in test.ExpectedValues)
        {
            expectedValue.Value.Should().NotBeNullOrWhiteSpace();
            expectedValue.Size.Should().BeGreaterThan(0);

            expectedValue.Type?.Should().Be(typeof(LiteralExpressionFactory));
        }
    }
}