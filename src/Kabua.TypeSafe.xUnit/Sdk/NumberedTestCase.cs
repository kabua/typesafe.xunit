using System;
using System.ComponentModel;
using System.Diagnostics;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit.Sdk;

/// <summary>
/// Default implementation of <see cref="IXunitTestCase"/> for xUnit v2 that supports tests decorated with <see cref="FormattedTheoryAttribute"/>.
/// </summary>
[DebuggerDisplay(@"\{ class = {TestMethod.TestClass.Class.Name}, method = {TestMethod.Method.Name}, display = {DisplayName}, skip = {SkipReason} \}")]
public class NumberedTestCase : XunitTestCase
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public NumberedTestCase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XunitTestCase"/> class.
    /// </summary>
    /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
    /// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
    /// <param name="defaultMethodDisplayOptions">Default method display options to use (when not customized).</param>
    /// <param name="testMethod">The test method this test case belongs to.</param>
    /// <param name="testId">The test number for this <see cref="DataAttribute"/></param>
    /// <param name="testMethodArguments">The arguments for the test method.</param>
    public NumberedTestCase(IMessageSink diagnosticMessageSink,
        TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions,
        ITestMethod testMethod,
        int? testId,
        object[] testMethodArguments = null!)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
    {
        TestId = testId;
    }

    protected override string GetDisplayName(IAttributeInfo factAttribute, string baseDisplayName)
    {
        var reflectionAttribute = factAttribute as IReflectionAttributeInfo;
        if (reflectionAttribute?.Attribute is ICustomDisplayName dataDisplayNameFormatter)
        {
            var context = new DisplayNameFormattingContext(factAttribute, this, Method, baseDisplayName, TestMethodArguments, MethodGenericTypes, TestId);
            var results = dataDisplayNameFormatter.GetDisplayName(context);

            return results;
        }

        return base.GetDisplayName(factAttribute, baseDisplayName);
    }

    protected int? TestId
    {
        get
        {
            EnsureInitialized();
            return _testId;
        }
        set
        {
            _testId = value;
            EnsureInitialized();
        }
    }

    /// <inheritdoc/>
    public override void Serialize(IXunitSerializationInfo data)
    {
        base.Serialize(data);

        data.AddValue("TestId", _testId);
    }

    /// <inheritdoc/>
    public override void Deserialize(IXunitSerializationInfo data)
    {
        base.Deserialize(data);

        _testId = data.GetValue<int?>("TestId");
    }

    private int? _testId;
}