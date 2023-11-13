using System.Reflection;
using Kabua.TypeSafe.xUnit.Sdk;
using Kabua.TypeSafe.xUnit.UnitTests.F04_MemberTestData_real_world_example_objects;

namespace Kabua.TypeSafe.xUnit.UnitTests.F06_real_world_example_objects;

/// <summary>
/// Here's a real world example
/// </summary>
public class GlobPatternDataAttribute : InlineObjectAttribute
{
    public GlobPatternDataAttribute(string pattern, params string[] segments)
        : this(pattern, false, "", segments)
    {
    }

    public GlobPatternDataAttribute(int testId, string pattern, params string[] segments)
        : this(testId, pattern, false, "", segments)
    {
    }

    public GlobPatternDataAttribute(int testId, string pattern, bool isVolume, string driveSegmentText, params string[] segments)
        : this(pattern, isVolume, driveSegmentText, segments)
    {
        TestId = testId;
    }

    public GlobPatternDataAttribute(string pattern, bool isVolume, string driveSegmentText, params string[] segments)
    {
        Pattern = pattern;
        IsVolume = isVolume;
        DriveSegmentText = driveSegmentText;
        Segments = segments;
    }

    public int? TestId { get; }

    public string Pattern { get; }

    public bool IsVolume { get; }
    public string[] Segments { get; }

    public string DriveSegmentText { get; set; }

    public bool IsNegated { get; set; }

    public bool IsUnc { get; set; }

    public bool DriveSegmentIsAbsPath { get; set; }

    public bool IsLastDirectoryOnly { get; set; }

    protected override object GetObject(MemberInfo testMethod)
        => new GlobPatternData(Pattern,
            IsNegated: IsNegated,
            DriveSegment: new DriveSegmentData(
                IsVolume: IsVolume || DriveSegmentIsAbsPath,
                IsUnc: IsUnc,
                Text: DriveSegmentText,
                IsDirectoryOnly: DriveSegmentIsAbsPath),
            Segments: Segments,
            IsDirectoryOnly: IsLastDirectoryOnly
        );
}