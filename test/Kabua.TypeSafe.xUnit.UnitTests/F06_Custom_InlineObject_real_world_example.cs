using Kabua.TypeSafe.xUnit.UnitTests.F04_MemberTestData_real_world_example_objects;
using Kabua.TypeSafe.xUnit.UnitTests.F06_real_world_example_objects;

namespace Kabua.TypeSafe.xUnit.UnitTests;
public class C61_Custom_InlineObject_real_world_example
{
    /// <summary>
    /// Notice the custom test name format we created by overriding the <see cref="GlobPatternData.ToString"/>.<br/>
    /// Can be seen in either Microsoft's Unit Test Explorer or Resharper's Unit Test Sessions test names.
    /// </summary>
    [FormattedTheory]
    [GlobPatternData("")]
    [GlobPatternData("!", "", IsNegated = true)]
    [GlobPatternData("/", false, "", DriveSegmentIsAbsPath = true)]
    [GlobPatternData("c:", true, "c:")]
    [GlobPatternData("!c:", true, "c:", DriveSegmentIsAbsPath = false, IsNegated = true)]
    [GlobPatternData("c:/", true, "C:", DriveSegmentIsAbsPath = true)]
    [GlobPatternData("//host/share", true, "//HOST/SHARE", IsUnc = true)]
    [GlobPatternData("//host/share/", true, "//host/share", IsUnc = true, DriveSegmentIsAbsPath = true)]
    [GlobPatternData("!//host/share/", true, "//host/share", IsUnc = true, DriveSegmentIsAbsPath = true, IsNegated = true)]
    [GlobPatternData(@"c:this/is/a/relative/pattern/test", true, "c:", "this", "is", "a", "relative", "pattern", "test")]
    public void T01_VerifyDriveSegmentOnlyTests(GlobPatternData test)
    {
        // Notice how expressive this can be.
        // We are taking a flat structure and transforming it in to a rich object model.
        // including a record within a record and an array of strings.
        //
        string pattern = test.Pattern;
        bool isNegated = test.IsNegated;
        DriveSegmentData driveSegment = test.DriveSegment;
        string[] segments = test.Segments;
        bool isDirectoryOnly = test.IsDirectoryOnly;
    }
}