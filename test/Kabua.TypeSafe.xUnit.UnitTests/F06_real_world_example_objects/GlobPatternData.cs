using System.Diagnostics;
using Kabua.TypeSafe.xUnit.UnitTests.F04_MemberTestData_real_world_example_objects;

namespace Kabua.TypeSafe.xUnit.UnitTests.F06_real_world_example_objects;

[DebuggerDisplay("{Pattern}")]
public record GlobPatternData(string Pattern, bool IsNegated, DriveSegmentData DriveSegment, string[] Segments, bool IsDirectoryOnly)
{
    public string Pattern { get; } = Pattern;
    public bool IsNegated { get; } = IsNegated;
    public DriveSegmentData DriveSegment { get; } = DriveSegment;
    public string[] Segments { get; } = Segments;
    public bool IsDirectoryOnly { get; } = IsDirectoryOnly;
    public override string ToString() => $"Pattern: \"{Pattern}\"";
}