using System.Diagnostics;

namespace Kabua.TypeSafe.xUnit.UnitTests.F04_MemberTestData_real_world_example_objects;

[DebuggerDisplay("{Text}")]
public record DriveSegmentData(bool IsVolume, bool IsUnc, string Text, bool IsDirectoryOnly)
{
    public bool IsVolume { get; } = IsVolume;
    public bool IsUnc { get; } = IsUnc;
    public string Text { get; } = Text;
    public bool IsDirectoryOnly { get; } = IsDirectoryOnly;
}