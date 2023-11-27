using System.Diagnostics;
using System.Reflection;
using FluentAssertions;
// ReSharper disable MemberCanBePrivate.Local

namespace Kabua.TypeSafe.xUnit.UnitTests;

public class C51_Custom_InlineObject
{

    /// <summary>
    /// Derive from <see cref="InlineObjectAttribute"/> to create your custom inline object data attribute.</summary>
    /// <remarks>Notice that you can mark the attribute private. Thus no information leakage.</remarks>
    private class SimpleStringDataAttribute : InlineObjectAttribute
    {
        protected override object GetObject(MemberInfo testMethod) => "simple test";
    }

    [Theory]
    [SimpleStringData]
    public void T01_SimpleStringTest(string test)
    {
        test.Should().Be("simple test");
    }

    //
    // ------------------------------------------------------------------------------------------------
    //

    [FormattedTheory]
    [MySimpleObjectData("Bob", "Smith")]
    [MySimpleObjectData("Bob", "Smith", Age = 10)]
    [MySimpleObjectData("Alice", "Copper", Age = 50, Skip = "This Test")]
    public void T02_MySimpleObjectTest(MySimpleObject test)
    {
        test.FirstName.Should().Be("Bob");
        test.LastName.Should().Be("Smith");
        test.Age?.Should().Be(10);
    }

    private class MySimpleObjectDataAttribute : InlineObjectAttribute
    {
        public MySimpleObjectDataAttribute(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public int Age { get; set; }

        /// <summary>
        /// Note: can't use nullable types in attributes so we will use a simple test.
        /// </summary>
        protected override object GetObject(MemberInfo testMethod) => new MySimpleObject(FirstName, LastName, Age > 0 ? Age : null);
    }

    public record MySimpleObject
    {
        public MySimpleObject(string firstName, string lastName, int? age = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public int? Age { get; }
    }
}