using FluentAssertions;

namespace Kabua.TypeSafe.xUnit.UnitTests;

/// <summary>
/// When using multiple <see cref="InlineDataAttribute"/> statements, they are normally listed in <b>alphabetical order</b>
/// in Test Explorer and Resharper's Unit Test Sessions, making debugging difficult when tests fail.
/// <br/>
/// However, by changing <see cref="TheoryAttribute"/> to <see cref="FormattedTheoryAttribute"/>,
/// we get numbered tests in the order they are listed in the code. As a result, when tests fail, debugging becomes easier.
/// <br/>
/// Using <see cref="FormattedTheoryAttribute"/> to control display output works with <see cref="ClassDataAttribute"/> and with <see cref="MemberDataAttribute"/> as well.
/// </summary>
public class C11_Use_FormattedTheory_To_Controlling_Display_Order
{
    /// <summary>
    /// When using the default <see cref="TheoryAttribute"/>, the <see cref="InlineDataAttribute"/> tests are listed in <b>alphabetical order</b>
    /// in Microsoft Test Explorer and in Resharper's Unit Test Sessions. Which makes it harder to debug when tests fail.
    /// </summary>
    /// <remarks>
    /// In this example, the tests are ordered alphabetically: 1,5, 9
    /// <br/>
    /// Shown as:<br/>
    /// T01_Using_Theory_InlineData_is_shows_tests_alphabetical(a: 1, b: 1, expected: 2)<br/>
    /// T01_Using_Theory_InlineData_is_shows_tests_alphabetical(a: 5, b: -5, expected: 0)<br/>
    /// T01_Using_Theory_InlineData_is_shows_tests_alphabetical(a: 9, b: 1, expected: 10)
    /// </remarks>
    [Theory]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void T01_Using_Theory_InlineData_is_shows_tests_alphabetical(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }

    /// <summary>
    /// When using the new <see cref="FormattedTheoryAttribute"/>, the <see cref="InlineDataAttribute"/> tests are numbered and listed in the same order as they are shown in code, thus making them easier to debug when tests fail.
    /// </summary>
    /// <remarks>
    /// In this example, the tests are ordered as defined in the code: 9, 5, 1
    /// <br/>
    /// Shown as:<br/>
    /// 1) T02_Using_FormattedTheory_InlineData_is_shows_tests_sequentially(a: 9, b: 1, expected: 10)<br/>
    /// 2) T02_Using_FormattedTheory_InlineData_is_shows_tests_sequentially(a: 5, b: -5, expected: 0)<br/>
    /// 3) T02_Using_FormattedTheory_InlineData_is_shows_tests_sequentially(a: 1, b: 1, expected: 2)
    /// </remarks>
    [FormattedTheory]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void T02_Using_FormattedTheory_InlineData_is_shows_tests_sequentially(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }

    /// <summary>
    /// By using <see cref="FormattedTheoryAttribute.EnableOrderedTests"/> we can control where or not to use sequential test numbers.
    /// </summary>
    /// <remarks>
    /// In this example, the tests are ordered alphabetically: 1,5, 9
    /// <br/>
    /// Shown as:<br/>
    /// T03_Use_FormattedTheory_EnableTestIds_to_disable_sequential_numbering(a: 1, b: 1, expected: 2)
    /// T03_Use_FormattedTheory_EnableTestIds_to_disable_sequential_numbering(a: 5, b: -5, expected: 0)<br/>
    /// T03_Use_FormattedTheory_EnableTestIds_to_disable_sequential_numbering(a: 9, b: 1, expected: 10)<br/>
    /// </remarks>
    [FormattedTheory(EnableOrderedTests = false)]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void T03_Use_FormattedTheory_EnableTestIds_to_disable_sequential_numbering(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }

    /// <summary>
    /// By using <see cref="FormattedTheoryAttribute.TestNumberDisplayNameFormat"/> we can change how the test numbers are formatted.
    /// </summary>
    /// <remarks>
    /// In this example, the test number format has been changed to: #{0,3:D} >> {1}
    /// <br/>
    /// Shown as:<br/>
    /// # 1 >> T04_Use_FormattedTheory_TestNumberDisplayNameFormat_to_change_test_number_format(a: 9, b: 1, expected: 10)<br/>
    /// # 2 >> T04_Use_FormattedTheory_TestNumberDisplayNameFormat_to_change_test_number_format(a: 5, b: -5, expected: 0)<br/>
    /// # 3 >> T04_Use_FormattedTheory_TestNumberDisplayNameFormat_to_change_test_number_format(a: 1, b: 1, expected: 2)
    /// </remarks>
    [FormattedTheory(TestNumberDisplayNameFormat = "#{0,3:D} >> {1}")]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void T04_Use_FormattedTheory_TestNumberDisplayNameFormat_to_change_test_number_format(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }

    /// <summary>
    /// By using <see cref="FormattedTheoryAttribute.StartingTestNumber"/> we can control the starting test number.
    /// </summary>
    /// <remarks>
    /// In this example, the test numbers start at 10
    /// <br/>
    /// Shown as:<br/>
    /// 10) T05_Use_FormattedTheory_StartingTestId_to_change_the_starting_test_number(a: 9, b: 1, expected: 10)<br/>
    /// 11) T05_Use_FormattedTheory_StartingTestId_to_change_the_starting_test_number(a: 5, b: -5, expected: 0)<br/>
    /// 12) T05_Use_FormattedTheory_StartingTestId_to_change_the_starting_test_number(a: 1, b: 1, expected: 2)
    /// </remarks>
    [FormattedTheory(StartingTestNumber = 10)]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void T05_Use_FormattedTheory_StartingTestId_to_change_the_starting_test_number(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }
}