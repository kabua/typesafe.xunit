using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit.Sdk;

/// <summary>
///     The test case runner for xUnit.net v2 theories (which could not be pre-enumerated;
///     pre-enumerated test cases use <see cref="XunitTestCaseRunner" />).
/// </summary>
public class NumberedTheoryTestCaseRunner : XunitTestCaseRunner
{
    static NumberedTheoryTestCaseRunner()
    {
        var assembly = typeof(TheoryDiscoverer).Assembly;
        var type = assembly.GetType("Xunit.Sdk.SerializationHelper");
        s_getType = type?.GetMethod("GetType", BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Standard, new[] { typeof(string), typeof(string) }, null);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NumberedTheoryTestCaseRunner" /> class.
    /// </summary>
    /// <param name="testCase">The test case to be run.</param>
    /// <param name="theoryAttribute">The theory attribute attached to the test method.</param>
    /// <param name="displayName">The display name of the test case.</param>
    /// <param name="skipReason">The skip reason, if the test is to be skipped.</param>
    /// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
    /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
    /// <param name="messageBus">The message bus to report run status to.</param>
    /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
    /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
    public NumberedTheoryTestCaseRunner(NumberedTheoryTestCase testCase,
        IAttributeInfo theoryAttribute, string displayName, string skipReason, object[] constructorArguments,
        IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
        : base(testCase, displayName, skipReason, constructorArguments, s_noArguments, messageBus, aggregator, cancellationTokenSource)
    {
        TheoryAttribute = theoryAttribute;
        DiagnosticMessageSink = diagnosticMessageSink;
    }

    protected IAttributeInfo TheoryAttribute { get; }

    /// <summary>
    ///     Gets the message sink used to report <see cref="IDiagnosticMessage" /> messages.
    /// </summary>
    protected IMessageSink DiagnosticMessageSink { get; }

    /// <inheritdoc />
    protected override async Task AfterTestCaseStartingAsync()
    {
        await base.AfterTestCaseStartingAsync();

        try
        {
            IEnumerable<IAttributeInfo>? dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute));

            var testId = 0;
            foreach (IAttributeInfo? dataAttribute in dataAttributes)
            {
                IAttributeInfo? discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                List<string> args = discovererAttribute.GetConstructorArguments().Cast<string>().ToList();
                Type? discovererType = GetType(args[1], args[0]);
                if (discovererType == null)
                {
                    Aggregator.Add(CreateInvalidOperationExceptionForDataDiscoverer(dataAttribute, "does not exist."));

                    continue;
                }

                IDataDiscoverer discoverer;
                try
                {
                    discoverer = ExtensibilityPointFactory.GetDataDiscoverer(DiagnosticMessageSink, discovererType);
                }
                catch (InvalidCastException)
                {
                    Aggregator.Add(CreateInvalidOperationExceptionForDataDiscoverer(dataAttribute, "does not implement IDataDiscoverer."));

                    continue;
                }

                IEnumerable<object[]>? data = discoverer.GetData(dataAttribute, TestCase.TestMethod.Method);
                if (data == null)
                {
                    Aggregator.Add(new InvalidOperationException(
                        $"Test data returned null for {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name}. Make sure it is statically initialized before this test method is called."));
                    continue;
                }

                foreach (object[]? dataRow in data)
                {
                    testId++;

                    _toDispose.AddRange(dataRow.OfType<IDisposable>());

                    ITypeInfo[]? resolvedTypes = null;
                    MethodInfo? methodToRun = TestMethod;
                    object?[]? convertedDataRow = methodToRun.ResolveMethodArguments(dataRow);

                    if (methodToRun.IsGenericMethodDefinition)
                    {
                        resolvedTypes = TestCase.TestMethod.Method.ResolveGenericTypes(convertedDataRow);
                        methodToRun = methodToRun.MakeGenericMethod(resolvedTypes.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
                    }

                    Type[] parameterTypes = methodToRun.GetParameters().Select(p => p.ParameterType).ToArray();
                    convertedDataRow = Reflector.ConvertArguments(convertedDataRow, parameterTypes);

                    string theoryDisplayName = GetDisplayName(TheoryAttribute, testId, DisplayName, convertedDataRow, resolvedTypes);
                    ITest? test = CreateTest(TestCase, theoryDisplayName);
                    string? skipReason = SkipReason ?? dataAttribute.GetNamedArgument<string>("Skip");
                    _testRunners.Add(CreateTestRunner(test, MessageBus, TestClass, ConstructorArguments, methodToRun, convertedDataRow,
                        skipReason, BeforeAfterAttributes, Aggregator, CancellationTokenSource));
                }
            }
        }
        catch (Exception ex)
        {
            // Stash the exception so we can surface it during RunTestAsync
            _dataDiscoveryException = ex;
        }
    }

    protected string GetDisplayName(IAttributeInfo factAttribute, int testId, string baseDisplayName, object?[] arguments, ITypeInfo[]? genericTypes)
    {
        var reflectionAttribute = factAttribute as IReflectionAttributeInfo;
        if (reflectionAttribute?.Attribute is ICustomDisplayName dataDisplayNameFormatter)
        {
            var context = new DisplayNameFormattingContext(factAttribute, TestCase, TestCase.TestMethod.Method, baseDisplayName, arguments, genericTypes, testId);
            var results = dataDisplayNameFormatter.GetDisplayName(context);

            return results;
        }

        return TestCase.TestMethod.Method.GetDisplayNameWithArguments(DisplayName, arguments, genericTypes);
    }

    /// <inheritdoc />
    protected override Task BeforeTestCaseFinishedAsync()
    {
        Aggregator.Aggregate(_cleanupAggregator);

        return base.BeforeTestCaseFinishedAsync();
    }

    /// <inheritdoc />
    protected override async Task<RunSummary> RunTestAsync()
    {
        if (_dataDiscoveryException != null)
            return RunTest_DataDiscoveryException();

        RunSummary runSummary = new RunSummary();
        foreach (XunitTestRunner testRunner in _testRunners)
        {
            runSummary.Aggregate(await testRunner.RunAsync());
        }

        // Run the cleanup here so we can include cleanup time in the run summary,
        // but save any exceptions so we can surface them during the cleanup phase,
        // so they get properly reported as test case cleanup failures.
        ExecutionTimer timer = new ExecutionTimer();
        foreach (IDisposable disposable in _toDispose)
        {
            timer.Aggregate(() => _cleanupAggregator.Run(disposable.Dispose));
        }

        runSummary.Time += timer.Total;
        return runSummary;
    }

    private RunSummary RunTest_DataDiscoveryException()
    {
        XunitTest test = new XunitTest(TestCase, DisplayName);

        if (!MessageBus.QueueMessage(new TestStarting(test)))
            CancellationTokenSource.Cancel();
        else if (!MessageBus.QueueMessage(new TestFailed(test, 0, null, Unwrap(_dataDiscoveryException))))
            CancellationTokenSource.Cancel();
        if (!MessageBus.QueueMessage(new TestFinished(test, 0, null)))
            CancellationTokenSource.Cancel();

        return new RunSummary
        {
            Total = 1,
            Failed = 1
        };
    }

    private InvalidOperationException CreateInvalidOperationExceptionForDataDiscoverer(IAttributeInfo dataAttribute, string reason)
    {
        if (dataAttribute is IReflectionAttributeInfo reflectionAttribute)
            return new InvalidOperationException(
                $"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} {reason}");

        return new InvalidOperationException($"A data discoverer specified on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} {reason}");
    }

    /// <summary>
    /// Unwraps an exception to remove any wrappers, like <see cref="TargetInvocationException"/>.
    /// </summary>
    /// <param name="ex">The exception to unwrap.</param>
    /// <returns>The unwrapped exception.</returns>
    private static Exception Unwrap(Exception? ex)
    {
        while (true)
        {
            if (ex is not TargetInvocationException targetInvocationException)
                return ex!;

            ex = targetInvocationException.InnerException;
        }
    }

    internal static Type? GetType(string assemblyName, string typeName)
        => (Type?)s_getType?.Invoke(null, new object?[] { assemblyName, typeName });

    private static readonly MethodInfo? s_getType;

    private static readonly object[] s_noArguments = {};

    private readonly ExceptionAggregator _cleanupAggregator = new();
    private readonly List<XunitTestRunner> _testRunners = new();
    private readonly List<IDisposable> _toDispose = new();
    private Exception? _dataDiscoveryException;
}