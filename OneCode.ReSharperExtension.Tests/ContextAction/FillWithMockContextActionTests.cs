﻿using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using OneCode.ReSharperExtension.ContextActions;

namespace Abc.MoqComplete.Tests.ContextAction
{
    [TestNetCore21("Moq/4.10.1")]
    public class FillWithMockContextActionTests : ContextActionExecuteTestBase<FillWithMockContextAction>
    {
        protected override string RelativeTestDataPath => "ContextAction";
        protected override string ExtraPath => "";

        [TestCase("fill_with_mock")]
        [TestCase("fill_with_mock_with_existing_mock")]
        [TestCase("fill_parameters_with_generics")]
        public void should_test_execution(string name) => DoOneTest(name);
    }
}
