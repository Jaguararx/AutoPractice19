namespace COE.Example.Tests
{
    /// <summary>
    /// Class for storing the values of common feature file tags used in our test scenarios
    /// </summary>
    public class TestTags
    {
        /// <summary>
        /// Usage denotes that tests possibly will open new tabs in browser during scenario execution
        /// Tests using this tag will have closed all additionally opened tabs after scenario execution
        /// </summary>
        public const string CloseSecondaryTabs = nameof(CloseSecondaryTabs);

        /// <summary>
        /// Usage denotes that this is a visual test
        /// Visual test specific setup and teardown methods will be called during execution
        /// </summary>
        public const string VisualTest = nameof(VisualTest);


        /// <summary>
        /// Usage denotes that after the test is finished any alerts should be automatically handled and cleared by the teardown code
        /// </summary>
        public const string HandleAlerts = nameof(HandleAlerts);

    }
}
