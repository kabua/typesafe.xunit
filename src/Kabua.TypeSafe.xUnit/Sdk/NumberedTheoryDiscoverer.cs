using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit.Sdk;

/// <summary>
/// Implementation of <see cref="IXunitTestCaseDiscoverer"/> that supports finding test cases
/// on methods decorated with <see cref="FormattedTheoryAttribute"/>.
/// </summary>
public class NumberedTheoryDiscoverer : IXunitTestCaseDiscoverer
{
    static NumberedTheoryDiscoverer()
    {
        var assembly = typeof(TheoryDiscoverer).Assembly;
        var type = assembly.GetType("Xunit.Serialization.XunitSerializationInfo");
        s_canSerializeObjectInfo = type?.GetMethod("CanSerializeObject", BindingFlags.NonPublic | BindingFlags.Static);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TheoryDiscoverer"/> class.
    /// </summary>
    /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
    public NumberedTheoryDiscoverer(IMessageSink diagnosticMessageSink)
    {
        DiagnosticMessageSink = diagnosticMessageSink;
    }

    /// <summary>
    /// Gets the message sink to be used to send diagnostic messages.
    /// </summary>
    protected IMessageSink DiagnosticMessageSink { get; }

    /// <summary>
    /// Creates test cases for a single row of data. By default, returns a single instance of <see cref="XunitTestCase"/>
    /// with the data row inside of it.
    /// </summary>
    /// <param name="discoveryOptions">The discovery options to be used.</param>
    /// <param name="testMethod">The test method the test cases belong to.</param>
    /// <param name="theoryAttribute">The theory attribute attached to the test method.</param>
    /// <param name="dataRow">The row of data for this test case.</param>
    /// <param name="testId">The test number for this <see cref="DataAttribute"/></param>
    /// <returns>The test cases</returns>
    protected virtual IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
        IAttributeInfo theoryAttribute, object[] dataRow, int? testId)
        => new[] { new NumberedTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, testId, dataRow) };

    /// <summary/>
    /// <summary>
    /// Creates test cases for a single row of skipped data. By default, returns a single instance of <see cref="XunitSkippedDataRowTestCase"/>
    /// with the data row inside of it.
    /// </summary>
    /// <remarks>If this method is overridden, the implementation will have to override <see cref="TestMethodTestCase.SkipReason"/> otherwise
    /// the default behavior will look at the <see cref="TheoryAttribute"/> and the test case will not be skipped.</remarks>
    /// <param name="discoveryOptions">The discovery options to be used.</param>
    /// <param name="testMethod">The test method the test cases belong to.</param>
    /// <param name="theoryAttribute">The theory attribute attached to the test method.</param>
    /// <param name="dataRow">The row of data for this test case.</param>
    /// <param name="testId">The test number for this <see cref="DataAttribute"/></param>
    /// <param name="skipReason">The reason this test case is to be skipped</param>
    /// <returns>The test cases</returns>
    protected virtual IEnumerable<IXunitTestCase> CreateTestCasesForSkippedDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
        IAttributeInfo theoryAttribute, object[] dataRow, int? testId, string skipReason)
        => new[] { new NumberedSkippedDataRowTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, testId, skipReason, dataRow) };

    /// <summary>
    /// Creates test cases for a skipped theory. By default, returns a single instance of <see cref="XunitTestCase"/>
    /// (which inherently discovers the skip reason via the fact attribute).
    /// </summary>
    /// <param name="discoveryOptions">The discovery options to be used.</param>
    /// <param name="testMethod">The test method the test cases belong to.</param>
    /// <param name="theoryAttribute">The theory attribute attached to the test method.</param>
    /// <param name="skipReason">The skip reason that decorates <paramref name="theoryAttribute"/>.</param>
    /// <returns>The test cases</returns>
    protected virtual IEnumerable<IXunitTestCase> CreateTestCasesForSkip(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute, string skipReason)
        => new[] { new XunitTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod) };

    /// <summary>
    /// Creates test cases for the entire theory. This is used when one or more of the theory data items
    /// are not serializable, or if the user has requested to skip theory pre-enumeration. By default,
    /// returns a single instance of <see cref="NumberedTheoryTestCase"/>, which performs the data discovery
    /// at runtime.
    /// </summary>
    /// <param name="discoveryOptions">The discovery options to be used.</param>
    /// <param name="testMethod">The test method the test cases belong to.</param>
    /// <param name="theoryAttribute">The theory attribute attached to the test method.</param>
    /// <returns>The test case</returns>
    protected virtual IEnumerable<IXunitTestCase> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
        => new[] { new NumberedTheoryTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, theoryAttribute) };

    /// <summary>
    /// Discover test cases from a test method.
    /// </summary>
    /// <remarks>
    /// This method performs the following steps:
    /// - If the theory attribute is marked with Skip, returns the single test case from <see cref="CreateTestCasesForSkippedDataRow"/>;
    /// - If pre-enumeration is off, or any of the test data is non serializable, returns the single test case from <see cref="CreateTestCasesForTheory"/>;
    /// - If there is no theory data, returns a single test case of <see cref="ExecutionErrorTestCase"/> with the error in it;
    /// - Otherwise, it returns one test case per data row, created by calling <see cref="CreateTestCasesForDataRow"/> or <see cref="CreateTestCasesForSkippedDataRow"/> if the data attribute has a skip reason.
    /// </remarks>
    /// <param name="discoveryOptions">The discovery options to be used.</param>
    /// <param name="testMethod">The test method the test cases belong to.</param>
    /// <param name="theoryAttribute">The theory attribute attached to the test method.</param>
    /// <returns>Returns zero or more test cases represented by the test method.</returns>
    public virtual IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
    {
        // Special case Skip, because we want a single Skip (not one per data item); plus, a skipped test may
        // not actually have any data (which is quasi-legal, since it's skipped).
        var skipReason = theoryAttribute.GetNamedArgument<string>(nameof(FormattedTheoryAttribute.Skip));
        var enableTestIds = theoryAttribute.GetNamedArgument<bool>(nameof(FormattedTheoryAttribute.EnableOrderedTests));
        var startingTestId = theoryAttribute.GetNamedArgument<int>(nameof(FormattedTheoryAttribute.StartingTestNumber));

        if (skipReason != null)
            return CreateTestCasesForSkip(discoveryOptions, testMethod, theoryAttribute, skipReason);

        if (discoveryOptions.PreEnumerateTheoriesOrDefault())
        {
            try
            {
                var dataAttributes = testMethod.Method.GetCustomAttributes(typeof(DataAttribute));
                var results = new List<IXunitTestCase>();

                var testId = startingTestId;
                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();

                    IDataDiscoverer discoverer;
                    try
                    {
                        discoverer = ExtensibilityPointFactory.GetDataDiscoverer(DiagnosticMessageSink, discovererAttribute);
                    }
                    catch (InvalidCastException)
                    {
                        results.Add(CreateExecutionErrorTestCaseForDataDiscoverer(dataAttribute, testMethod, discoveryOptions, "does not implement IDataDiscoverer"));
                        continue;
                    }

                    if (discoverer == null)
                    {
                        results.Add(CreateExecutionErrorTestCaseForDataDiscoverer(dataAttribute, testMethod, discoveryOptions, "does not exist."));
                        continue;
                    }

                    skipReason = dataAttribute.GetNamedArgument<string>("Skip");

                    if (!discoverer.SupportsDiscoveryEnumeration(dataAttribute, testMethod.Method))
                        return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);

                    var data = discoverer.GetData(dataAttribute, testMethod.Method);
                    if (data == null)
                    {
                        results.Add(
                            new ExecutionErrorTestCase(
                                DiagnosticMessageSink,
                                discoveryOptions.MethodDisplayOrDefault(),
                                discoveryOptions.MethodDisplayOptionsOrDefault(),
                                testMethod,
                                $"Test data returned null for {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}. Make sure it is statically initialized before this test method is called."
                            )
                        );

                        continue;
                    }

                    foreach (var dataRow in data)
                    {
                        // Determine whether we can serialize the test case, since we need a way to uniquely
                        // identify a test and serialization is the best way to do that. If it's not serializable,
                        // this will throw and we will fall back to a single theory test case that gets its data at runtime.
                        if (!CanSerializeObject(dataRow))
                        {
                            DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Non-serializable data ('{dataRow.GetType().FullName}') found for '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}'; falling back to single test case."));
                            return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
                        }

                        var testCases =
                            skipReason != null
                                ? CreateTestCasesForSkippedDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow, enableTestIds ? testId : null, skipReason)
                                : CreateTestCasesForDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow, enableTestIds ? testId : null);

                        results.AddRange(testCases);

                        testId++;
                    }
                }

                if (results.Count == 0)
                    results.Add(new ExecutionErrorTestCase(DiagnosticMessageSink,
                        discoveryOptions.MethodDisplayOrDefault(),
                        discoveryOptions.MethodDisplayOptionsOrDefault(),
                        testMethod,
                        $"No data found for {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}"));

                return results;
            }
            catch (Exception ex) // If something goes wrong, fall through to return just the XunitTestCase
            {
                //Console.WriteLine($"Exception thrown during theory discovery on '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}'; falling back to single test case.{Environment.NewLine}{ex}");
                throw new Exception($"Exception thrown during theory discovery on '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}'; falling back to single test case.{Environment.NewLine}{ex}");

                //DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Exception thrown during theory discovery on '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}'; falling back to single test case.{Environment.NewLine}{ex}"));
            }
        }

        return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
    }

    private ExecutionErrorTestCase CreateExecutionErrorTestCaseForDataDiscoverer(IAttributeInfo dataAttribute, ITestMethod testMethod,
        ITestFrameworkDiscoveryOptions discoveryOptions, string reason)
    {
        if (dataAttribute is IReflectionAttributeInfo reflectionAttribute)
            return new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod,
                $"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} {reason}");
        else
            return new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod,
                $"A data discoverer specified on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} {reason}");
    }

    internal static bool CanSerializeObject(object value)
    {
        var obj = s_canSerializeObjectInfo?.Invoke(null, new[]
            {
                value
            });

        return obj != null && (bool) obj;
    }

    private static readonly MethodInfo? s_canSerializeObjectInfo;
}