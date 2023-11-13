namespace Kabua.TypeSafe.xUnit;

/// <summary>
/// Provides the ability to dynamically generate the test's display name
/// </summary>
public interface ICustomDisplayName
{
    /// <summary>
    /// Format the display name given the <paramref name="context"/> object.
    /// </summary>
    /// <param name="context">The <see cref="DisplayNameFormattingContext"/> to use for formatting.</param>
    /// <returns>The formatted display name.</returns>
    public string GetDisplayName(DisplayNameFormattingContext context);
}