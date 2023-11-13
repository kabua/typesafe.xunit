namespace Kabua.TypeSafe.xUnit.UnitTests.F04_MemberTestData_real_world_example_objects;

public class ExpectedTestData
{
    public ExpectedTestData(string value, int? size = null, Type? type = null)
    {
        Value = value;
        Size = size;
        Type = type;
    }

    public string Value { get; set; }

    public int? Size { get; set; }

    public Type? Type { get; set; } = null!;
}