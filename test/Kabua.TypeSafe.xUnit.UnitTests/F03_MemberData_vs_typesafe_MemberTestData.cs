using FluentAssertions;
using System.Reflection;

namespace Kabua.TypeSafe.xUnit.UnitTests;

/// <summary>
/// Working with a multi-dimensional array of objects is error prone, challenging to maintain, and are <b>not</b> 1st class unit test citizens.<br/>
/// </summary>
/// <remarks>
/// Note that <see cref="MemberDataAttribute"/> test data properties, fields. and methods <b>must</b> be public. What happen to encapsulation?
/// </remarks>
public class C31_using_MemberData_Property_data_model_version_01
{
    /// <summary>
    /// Take for example this local property...
    /// </summary>
    public static IEnumerable<object[]> PersonPropertyData31 => new List<object[]>
        {
            new object[] { "Cathy", "Smith", 27 },
            new object[] { "Bryce", "Miller", 35},
            new object[] { "Alex",  "Jones", 5},
        };

    /// <summary>
    /// Using <see cref="MemberDataAttribute"/> is error prone, as this example shows.<br/>
    /// </summary>
    [Theory(Skip = "As expected, param 'string d' will cause a runtime-error")]
    [MemberData(nameof(PersonPropertyData31))]
    public void T01_Runtime_error_params_do_not_match_object_model(int a, int b, int c, string d)
    {
        a.Should().BeGreaterThan(0);
        b.Should().Be(1);
        c.Should().BeLessThan(0);
        d.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// This example is a little better, but we can do better...
    /// </summary>
    [Theory]
    [MemberData(nameof(PersonPropertyData31))]
    public void T02_Better_named_object_params_version_01(string firstName, string lastName, int age)
    {
        firstName.Should().NotBeNullOrWhiteSpace();
        lastName.Should().NotBeNullOrWhiteSpace();
        age.Should().BeGreaterThan(0);
    }
}

/// <summary>
/// Let's convert <see cref="C31_using_MemberData_Property_data_model_version_01"/> to be type-safe.<br/>
/// First, let's create a simple record
/// </summary>
public class PersonTestData32
{
    public PersonTestData32(string FirstName, string LastName, int Age)
    {
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.Age = Age;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}

public class C32_using_MemberTestData_Property_data_model_version_01
{
    /// <summary>
    /// Notice the we an now support both public and private static properties, fields and methods thus encapsulating our test data.<br/>
    /// Also note that the signature changed from <see cref="IEnumerable{object[]}"/> to <see cref="List{PersonTestData31}"/>, which is type-safe.
    /// </summary>
    private static List<PersonTestData32?> PersonPropertyData32 => new()
        {
            new PersonTestData32("Cathy", "Smith", 27),
            new PersonTestData32("Bryce", "Miller", 35),
            new PersonTestData32("Alex", "Jones", 5),
        };

    /// <summary>
    /// We now have unit tests that are type-safe, 1st class citizen 
    /// </summary>
    /// <remarks>
    /// Depending on the .NET version, we can use chose between two different versions of the <see cref="MemberTestDataAttribute"/>, the none-generic and the generic versions.
    /// </remarks>

    // Use formatted test numbers...
    [FormattedTheory]
#if NET6_0_OR_GREATER
    [MemberTestData<PersonTestData32>(nameof(PersonPropertyData32))]
#else
    [MemberTestData(typeof(PersonTestData32), nameof(PersonPropertyData32))]
#endif

    public void T01_Using_the_TypeSafe_ClassTestData_with_Theory_version_01(PersonTestData32 testData)
    {
        testData.FirstName.Should().NotBeNullOrWhiteSpace();
        testData.LastName.Should().NotBeNullOrWhiteSpace();
        testData.Age.Should().BeGreaterThan(0);
    }
}

//
// ------------------------------------------------------------------------------------------------
//

/// <summary>
/// Now let's say application changed and we now need to update our unit tests.
/// </summary>
public class C33_using_MemberData_Property_data_model_version_02
{
    /// <summary>
    /// Your boss states it now needs to support Addresses. So you think 1 address and write this.<br/>
    /// Only to find out you need to support 1 or more. Now what?
    /// </summary>
    public static IEnumerable<object[]> PersonPropertyData33 => new List<object[]>
        {
            new object[] { "Cathy", "Smith", 27, new object[] {"1 apple st", "Mankato", "Mississippi", "96522"} },
            new object[] { "Bryce", "Miller", 35, new object[] {"1292 Dictum Ave.", "San Antonio", "MI", "47096"} },
            new object[] { "Alex",  "Jones", 5, new object[] {"102 Integer Rd", "Corona", "NM", "08219"}},
        };

    /// <summary>
    /// Dealing with object arrays of object arrays is painful.
    /// </summary>
    [Theory]
    [MemberData(nameof(PersonPropertyData33))]
    public void T01_Forgot_the_address_params_version_02(string firstName, string lastName, int age, object[] address)
    {
        firstName.Should().NotBeNullOrWhiteSpace();
        lastName.Should().NotBeNullOrWhiteSpace();
        age.Should().BeGreaterThan(0);

        // What a nightmare the following is. Reminds me of working in ADO.NET with all the manual casting that must be done.
        //
        var street = (string) address[0];
        var city = (string) address[1];
        var state = (string) address[2];
        var zip = (string) address[3];

        street.Should().NotBeNullOrWhiteSpace();
        city.Should().NotBeNullOrWhiteSpace();
        state.Should().NotBeNullOrWhiteSpace();
        zip.Should().NotBeNullOrWhiteSpace();
    }
}

/// <summary>
/// Let's convert <see cref="C31_using_MemberData_Property_data_model_version_01"/> to be type-safe.<br/>
/// First, let's create a simple record
/// </summary>
public class PersonTestData34
{
    public PersonTestData34(string FirstName, string LastName, int Age)
    {
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.Age = Age;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }

    public List<Address> Addresses { get; set; } = new();
}

public class Address
{
    public Address(string street, string city, string state, string zip)
    {
        Street = street;
        City = city;
        State = state;
        Zip = zip;
    }

    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string Zip { get; }
}

public class C34_using_MemberTestData_Property_data_model_version_01
{
    /// <summary>
    /// Adding the <see cref="Address"/> was easy, changing it from a single address to an array of addresses was also very easy to change.
    /// </summary>
    private static List<PersonTestData34> PersonPropertyData34 => new()
        {
            new PersonTestData34("Cathy", "Smith", 27) { Addresses = { new Address("1 apple st", "Mankato", "Mississippi", "96522") } },
            new PersonTestData34("Bryce", "Miller", 35) { Addresses = { new Address("1292 Dictum Ave.", "San Antonio", "MI", "47096") } },
            new PersonTestData34("Alex", "Jones", 5) { Addresses = { new Address("102 Integer Rd", "Corona", "NM", "08219"), new Address("1940 Tortor Str", "Santa Rosa", "MN", "98804") } },
        };

    /// <summary>
    /// Creating 1st class unit test, allows our code and unit tests to follow best practices and patterns.
    /// </summary>
    // Use formatted test numbers...
    [FormattedTheory]
#if NET6_0_OR_GREATER
    [MemberTestData<PersonTestData34>(nameof(PersonPropertyData34))]
#else
    [MemberTestData(typeof(PersonTestData34), nameof(PersonPropertyData34))]
#endif

    public void T01_Using_the_TypeSafe_ClassTestData_with_Theory(PersonTestData34 testData)
    {
        testData.FirstName.Should().NotBeNullOrWhiteSpace();
        testData.LastName.Should().NotBeNullOrWhiteSpace();
        testData.Age.Should().BeGreaterThan(0);

        foreach (var address in testData.Addresses)
        {
            address.Street.Should().StartWith("1");
            // etc.
        }
    }
}

