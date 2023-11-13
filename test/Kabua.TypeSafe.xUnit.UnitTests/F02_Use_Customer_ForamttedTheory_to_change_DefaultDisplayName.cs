using FluentAssertions;

namespace Kabua.TypeSafe.xUnit.UnitTests;

/// <summary>
/// Create an <see cref="Attribute"/> derived from <see cref="FormattedTheoryAttribute"/> to change the default display name and more.
/// </summary>
public class FormattedTheoryExample21Attribute : FormattedTheoryAttribute
{
    /// <summary>
    /// Show only the <see cref="DisplayNameFormattingContext.BaseDisplayName"/>
    /// </summary>
    protected override string GetDefaultDisplayName(DisplayNameFormattingContext context) => context.BaseDisplayName;
}

public class C21_Use_Customer_ATheory_ToChange_DefaultDisplayName_Example21
{
    /// <summary>
    /// In this example we will only display the method name
    /// </summary>
    /// <remarks>
    /// Shown as:<br/>
    /// 1) Using_a_CustomATheory_to_change_default_DisplayName_is_easy<br/>
    /// 2) Using_a_CustomATheory_to_change_default_DisplayName_is_easy<br/>
    /// 3) Using_a_CustomATheory_to_change_default_DisplayName_is_easy
    /// </remarks>
    [FormattedTheoryExample21]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void Using_a_CustomATheory_to_change_default_DisplayName_is_easy(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }
}

//
// ------------------------------------------------------------------------------------------------
//

public class FormattedTheoryExample22Attribute : FormattedTheoryAttribute
{
    /// <summary>
    /// Show only the <see cref="DisplayNameFormattingContext.GetArgumentsWithNames"/>, with the <see cref="DisplayNameFormattingContext.BaseDisplayName"/>
    /// </summary>
    protected override string GetDefaultDisplayName(DisplayNameFormattingContext context) => context.GetArgumentsWithNames(context.BaseDisplayName);
}

public class C22_Use_Customer_ATheory_ToChange_DefaultDisplayName_Example22
{
    /// <summary>
    /// In this example we will duplicate the default xUnit display format, meaning the method name with named arguments.
    /// </summary>
    /// <remarks>
    /// Shown as:<br/>
    /// 1) Sequential Test (a: 9, b: 1, expected: 10)<br/>
    /// 2) Sequential Test (a: 5, b: -5, expected: 0)<br/>
    /// 3) Sequential Test (a: 1, b: 1, expected: 2)
    /// </remarks>
    [FormattedTheoryExample22(DisplayName = "Sequential Test")]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void Using_a_CustomATheory_to_change_default_DisplayName_is_easy(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }
}

//
// ------------------------------------------------------------------------------------------------
//

public class FormattedTheoryExample23Attribute : FormattedTheoryAttribute
{
    /// <summary>
    /// Show only the <see cref="DisplayNameFormattingContext.GetArgumentsWithNames"/>
    /// </summary>
    protected override string GetDefaultDisplayName(DisplayNameFormattingContext context) => context.GetArgumentsWithNames();
}

public class C23_Use_Customer_ATheory_ToChange_DefaultDisplayName_Example23
{
    /// <summary>
    /// In this example we will only show arguments with names.
    /// </summary>
    /// <remarks>
    /// Shown as:<br/>
    /// 1) a: 9, b: 1, expected: 10<br/>
    /// 2) a: 5, b: -5, expected: 0<br/>
    /// 3) a: 1, b: 1, expected: 2
    /// </remarks>
    [FormattedTheoryExample23]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void Using_a_CustomATheory_To_Change_Default_DisplayName_Is_Easy(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }
}

//
// ------------------------------------------------------------------------------------------------
//

public class FormattedTheoryExample24Attribute : FormattedTheoryAttribute
{
    /// <summary>
    /// Show only the <see cref="DisplayNameFormattingContext.GetArgumentsWithoutNames"/>, with custom display name.
    /// </summary>
    protected override string GetDefaultDisplayName(DisplayNameFormattingContext context) => context.GetArgumentsWithoutNames("My Tests");
}

public class C24_Use_Customer_ATheory_ToChange_DefaultDisplayName_Example24
{
    /// <summary>
    /// In this example we will replace the method name with "My Tests" and display only the raw arguments (without their names).
    /// </summary>
    /// <remarks>
    /// Shown as:<br/>
    /// 1) My Tests (9, 1, 10)<br/>
    /// 2) My Tests (5, -5, 0)<br/>
    /// 3) My Tests (1, 1, 2)
    /// </remarks>
    [FormattedTheoryExample24]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void Using_a_CustomATheory_To_Change_Default_DisplayName_Is_Easy(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }
}

//
// ------------------------------------------------------------------------------------------------
//

public class FormattedTheoryExample25Attribute : FormattedTheoryAttribute
{
    /// <summary>
    /// Show only the <see cref="DisplayNameFormattingContext.GetArgumentsWithoutNames"/>
    /// </summary>
    protected override string GetDefaultDisplayName(DisplayNameFormattingContext context) => context.GetArgumentsWithoutNames();
}

public class C25_Use_Customer_ATheory_ToChange_DefaultDisplayName_Example25
{
    /// <summary>
    /// In this example we will display only the raw arguments (without their names).
    /// </summary>
    /// <remarks>
    /// Shown as:<br/>
    /// 1) 9, 1, 10<br/>
    /// 2) 5, -5, 0<br/>
    /// 3) 1, 1, 2
    /// </remarks>
    [FormattedTheoryExample25]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void Using_a_CustomATheory_To_Change_Default_DisplayName_Is_Easy(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }
}

//
// ------------------------------------------------------------------------------------------------
//

public class FormattedTheoryExample26Attribute : FormattedTheoryAttribute
{
    /// <summary>
    /// If needed, we can also override the main <see cref="GetDisplayName"/> method to control all aspects of the display name.
    /// </summary>
    public override string GetDisplayName(DisplayNameFormattingContext context) => "hi";
}

public class C26_Use_Customer_ATheory_ToChange_DefaultDisplayName_Example26
{
    /// <summary>
    /// In this example we will replace the entire display name logic with the word "hi".
    /// </summary>
    /// <remarks>
    /// Shown as:<br/>
    /// hi <br/>
    /// hi [2]<br/>
    /// hi [3] 
    /// </remarks>
    [FormattedTheoryExample26]
    [InlineData(9, 1, 10)]
    [InlineData(5, -5, 0)]
    [InlineData(1, 1, 2)]
    public void Using_a_CustomATheory_To_Change_Default_DisplayName_Is_Easy(int a, int b, int expected)
    {
        var actual = a + b;

        actual.Should().Be(expected);
    }
}