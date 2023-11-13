namespace Kabua.TypeSafe.xUnit.UnitTests.F04_MemberTestData_real_world_example_objects;

public class BuilderTestData
{
    public BuilderTestData(string pattern)
    {
        Pattern = pattern;
    }

    public string Pattern { get; }

    public List<ExpectedTestData> ExpectedValues { get; set; } = new();
}