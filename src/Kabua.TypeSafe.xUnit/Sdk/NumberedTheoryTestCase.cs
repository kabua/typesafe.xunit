using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit.Sdk;

/// <summary>
/// Represents a test case which runs multiple tests for theory data, either because the
/// data was not enumerable or because the data was not serializable.
/// </summary>
public class NumberedTheoryTestCase : XunitTestCase
{
    /// <summary/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public NumberedTheoryTestCase()
    {
        TheoryAttribute = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberedTheoryTestCase"/> class.
    /// </summary>
    /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
    /// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
    /// <param name="defaultMethodDisplayOptions">Default method display options to use (when not customized).</param>
    /// <param name="testMethod">The method under test.</param>
    /// <param name="theoryAttribute">The theory attribute attached to the test method.</param>
    public NumberedTheoryTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
    {
        TheoryAttribute = theoryAttribute;
    }

    protected IAttributeInfo TheoryAttribute { get; }

    /// <inheritdoc />
    public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        object[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
        => new NumberedTheoryTestCaseRunner(this, TheoryAttribute, DisplayName, SkipReason, constructorArguments, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource).RunAsync();
}