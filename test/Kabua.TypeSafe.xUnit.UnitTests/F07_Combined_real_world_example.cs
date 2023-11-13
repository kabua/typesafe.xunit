using FluentAssertions;
using Kabua.TypeSafe.xUnit.UnitTests.F04_MemberTestData_real_world_example_objects;
using Kabua.TypeSafe.xUnit.UnitTests.F07_Combined_real_world_example_objects;

namespace Kabua.TypeSafe.xUnit.UnitTests;

/// <summary>
/// Here we can't use attributes for all the various tests, so we combine inline tests (<see cref="BuilderTestDataAttribute"/>) with a test property (<see cref="MemberTestDataAttribute"/>)
/// </summary>
/// <remarks>
/// Notice how all of this is type safe and the display output is listed in the order the tests are specified.
/// </remarks>
public class C71_Combined_real_world_example
{
    /// <summary>
    /// Creating objects, like <see cref="BuilderTestData.ExpectedValues"/> can't be done within an attribute, so we use a property that returns a <see cref="List{T}"/>
    /// </summary>
    private static List<BuilderTestData> BuildLiteralExpressionData => new()
        {
            new (@"removeEndingSpaces   ") { ExpectedValues = { new (@"removeEndingSpaces", 21, typeof(LiteralExpressionFactory)) }},
            new (@"removeEndingSpaces\   ") { ExpectedValues = { new (@"removeEndingSpaces ", 21, typeof(LiteralExpressionFactory)) }},
            new (@"removeEndingSpaces\ \  ") { ExpectedValues = { new (@"removeEndingSpaces  ", 21, typeof(LiteralExpressionFactory)) }},
        };

    [FormattedTheory]
    [BuilderTestData("", Types = new[] { typeof(object) })]
    [BuilderTestData("name")]
    [BuilderTestData("na?me", "na", "?", "me", Types = new[] { typeof(LiteralExpressionFactory), typeof(VarLenStringExpressionFactory), typeof(LiteralExpressionFactory) })]
    [BuilderTestData("nam*", "nam", "*", Types = new[] { typeof(LiteralExpressionFactory), typeof(VarLenStringExpressionFactory) })]
    [BuilderTestData("na[me]", "na", "[em]", Types = new[] { typeof(LiteralExpressionFactory), typeof(RangeExpressionFactory) })]
    [BuilderTestData(" KeepLeadSpaces", " KeepLeadSpaces", Types = new[] { typeof(LiteralExpressionFactory) })]

#if NET6_0_OR_GREATER
    [MemberTestData<BuilderTestData>(nameof(BuildLiteralExpressionData))]
#else
    [MemberTestData(typeof(BuilderTestData), nameof(BuildLiteralExpressionData))]
#endif
    public void T01_BuildLiteralExpressionTests(BuilderTestData test)
    {
        test.Pattern.Should().NotBeNull();
        test.ExpectedValues.Should().HaveCountGreaterThan(-1);

        foreach (ExpectedTestData expectedValue in test.ExpectedValues)
        {
            expectedValue.Value.Should().NotBeNull();
            expectedValue.Size.Should().BeGreaterThan(-1);
        }
    }
}