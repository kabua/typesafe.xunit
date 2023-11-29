![Logo](https://raw.githubusercontent.com/kabua/typesafe.xunit/main/logo-type-safe-xunit-235.png?raw=true, "Logo")

# Type Safe xUnit

A Type Safe Data-Driven Extension for xUnit.net

## Goal

The following goals were the inspiration behind this project's creation:

1. Make unit testing a 1st-class citizen by using type-safe data-driven objects.
1. To make xUnit data-driven features type safe, easy to use, and flexible, by employing real POCO objects.
1. To make it easier to develop your own type-safe data-driven xUnit attributes, albeit not required.
1. To make it easy to alter the test's display text on a global and per-test basis.
1. To ensure that the sequence in which the data-driven tests were created corresponds to the order in which they are shown in the Test Runner.

## Getting Started

First, the `Kabua.TypeSafe.xUnit` project supports both `.net461` and `.net6` and above. 

Second, the easiest way to get started is to use the nuget packages from [nuget.org](nuget.org).

## Building the Code

These instructions will get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

No installation is required. Even though this project references the `xunit.assert` and `xunit.core` nuget packages.

### Installing and running the tests

1. clone from GitHub 
1. open a CLI
1. navigate to the `Kabua.TypeSafe.xUnit.UnitTests` project.
1. run `dotnet test`. This will build the solution and then run all the xUnit tests.

#### Example

From the command prompt.
```
c:>mkdir temp\Kabua.TypeSafe.xUnit
c:>cd temp\Kabua.TypeSafe.xUnit
c:\temp\Kabua.TypeSafe.xUnit>git clone https://github.com/kabua/typesafe.xunit.git
c:\temp\Kabua.TypeSafe.xUnit>cd TypeSafe.xUnit\Kabua.TypeSafe.xUnit.UnitTests
c:\temp\Kabua.TypeSafe.xUnit\TypeSafe.xUnit\Kabua.TypeSafe.xUnit.UnitTests>dotnet test
```

## Deployment

The `Kabua.TypeSafe.xUnit` project supports both `.net4.61` and `.net6` and above. The package is release on www.nuget.org

## How to use

The `Kabua.TypeSafe.xUnit.UnitTests` contains over 50 examples on how to use the rich type-safe data-driven attributes for xUnit.

#### How the Unit Test Examples Are Structured

To learn how to utilize these new type-safe data-driven attributes, read the examples from the first source file to the last. As concepts in later files build on earlier ones.

To improve readability, we format files as follows:
* Prefixed files with F0x_ (e.g., F01_ to F07_) and include a descriptive name.
* Files can include many classes. Class names begin with CXY_, where X is the file number, and Y in a numbered class. For example: C32_, indicates the second class in the third file.
* To aid with Test Runner formatting, we prefix all tests with T0x_, such as T01_, T02_, etc.

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Tristen Fielding** - *Initial author* - [Kabua](https://github.com/Kabua)
* **Billie Thompson** - *Initial README work* - [PurpleBooth](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/Kabua/typesafe.xunit/contributors) who participated in this project.

## License

This project is licensed under the MIT License so that everyone can use it - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
