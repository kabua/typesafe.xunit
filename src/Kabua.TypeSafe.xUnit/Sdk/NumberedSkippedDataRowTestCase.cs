using System;
using System.ComponentModel;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kabua.TypeSafe.xUnit.Sdk;

/// <summary>
/// Represents a test case that had a valid data row, but the data row was generated by a data attribute with the skip property set.
/// </summary>
/// <remarks>This class is only ever used if the discoverer is pre-enumerating theories and the data row is serializable.</remarks>
public class NumberedSkippedDataRowTestCase : XunitSkippedDataRowTestCase
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public NumberedSkippedDataRowTestCase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XunitSkippedDataRowTestCase"/> class.
    /// </summary>
    /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
    /// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
    /// <param name="defaultMethodDisplayOptions">Default method display options to use (when not customized).</param>
    /// <param name="testMethod">The test method this test case belongs to.</param>
    /// <param name="testId">The test number for this <see cref="DataAttribute"/></param>
    /// <param name="skipReason">The reason that this test case will be skipped</param>
    /// <param name="testMethodArguments">The arguments for the test method.</param>
    public NumberedSkippedDataRowTestCase(IMessageSink diagnosticMessageSink,
        TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions,
        ITestMethod testMethod,
        int? testId,
        string skipReason,
        object[] testMethodArguments = null!)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, skipReason, testMethodArguments)
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