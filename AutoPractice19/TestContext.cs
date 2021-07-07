using System;
using COE.Core;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace COE.Example.Tests
{
    public sealed class TestContext
    {
        public TestContext(ScenarioContext scenarioContext, FeatureContext featureContext, TestSettings settings)
        {
            Current = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            TestSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            FeatureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
            WebDriverSettings = TestSettings.WebDriverSettings;
        }

        public ScenarioContext Current { get; }
        public FeatureContext FeatureContext { get; }

        public TestSettings TestSettings
        {
            get => GetContext<TestSettings>(nameof(TestSettings));
            set => Current[nameof(TestSettings)] = value;
        }

        public WebDriverSettings WebDriverSettings
        {
            get => GetContext<WebDriverSettings>(nameof(WebDriverSettings));
            set => Current[nameof(WebDriverSettings)] = value;
        }

        public Table SignatureRulesTable
        {
            get => GetContext<Table>(nameof(SignatureRulesTable));
            set => Current[nameof(SignatureRulesTable)] = value;
        }

        public void AddAttachmentToTestResult(string filePath) => NUnit.Framework.TestContext.AddTestAttachment(filePath);

        private T GetContext<T>(string name)
        {
            return Current.TryGetValue(name, out T value) ? value : default;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}