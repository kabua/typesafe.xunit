using System;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit;

///// <summary>
///// Marks a test method as being a data theory. Data theories are tests that are fed
///// various bits of data from a data source, mapping to parameters on the test method.
///// If the data source contains multiple rows, then the test method is executed
///// multiple times (once with each data row). Data is provided by attributes which
///// derive from <see cref="DataAttribute"/> (notably, <see cref="MemberDataAttribute"/> and
///// <see cref="Xunit"/>).
///// </summary>
[XunitTestCaseDiscoverer("Kabua.TypeSafe.xUnit.Sdk.NumberedTheoryDiscoverer", "Kabua.TypeSafe.xUnit")]
[AttributeUsage(AttributeTargets.Method)]
public class FormattedTheoryAttribute : FactAttribute, ICustomDisplayName
{
    /// <summary>
    /// Get or set whether or not to use test numbers to show the tests in the order the tests are defined in the code. Default: <c>true</c>.
    /// </summary>
    public bool EnableOrderedTests { get; set; } = true;

    /// <summary>
    /// Get or set the starting test sequence number. Default: <c>1</c>
    /// </summary>
    public int StartingTestNumber { get; set; } = 1;

    /// <summary>
    /// Get or set the display name format when formatting numbered tests. Default: <c>"{0,3:D}) {1}"</c>
    /// </summary>
    /// <remarks>
    /// Arg 0 - the test number
    /// <br/>
    /// Arg 1 - the output from the <see cref="GetDefaultDisplayName"/>
    /// </remarks>
    public string TestNumberDisplayNameFormat { get; set; } = "{0,3:D}) {1}";

    /// <summary>
    /// If any of the <see cref="DisplayNameFormattingContext.Arguments"/> implements <see cref="ICustomDisplayName"/> then call the first one found.
    /// Otherwise, call the <see cref="GetDefaultDisplayName"/> then if <see cref="DisplayNameFormattingContext.TestId"/> is defined,
    /// format the <see cref="DisplayNameFormattingContext.TestId"/> as defined by the <see cref="TestNumberDisplayNameFormat"/> string.
    /// </summary>
    /// <param name="context">A context containing all the values that can be used to help with formatting the display name.</param>
    /// <returns>the formatted display name</returns>
    public virtual string GetDisplayName(DisplayNameFormattingContext context)
    {
        string displayName;

        var onlyObj = context.Arguments.SingleOrDefault(a => a is ICustomDisplayName);
        if (onlyObj is ICustomDisplayName customDisplayName)
        {
            displayName = customDisplayName.GetDisplayName(context);
        }
        else
        {
            displayName = GetDefaultDisplayName(context);
            displayName = context.GetNumberedDisplayName(displayName, TestNumberDisplayNameFormat);
        }

        return displayName;
    }

    /// <summary>
    /// The default display name is formatted by calling <see cref="DisplayNameFormattingContext.GetDisplayNameWithArguments"/>
    /// </summary>
    /// <param name="context">A context containing all the values that can be used to help with formatting the display name.</param>
    /// <returns>The formatted display name</returns>
    protected virtual string GetDefaultDisplayName(DisplayNameFormattingContext context) => context.GetDisplayNameWithArguments();
}