using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using ReturnNull.CanonicalRoutes.Internal;
using ReturnNull.CanonicalRoutes.Mvc;
using Shouldly;

namespace ReturnNull.CanonicalRoutes.Tests
{
    [TestFixture]
    public class AttributeHelperTests
    {
        private Mock<ActionDescriptor> _mockActionDescriptor;
        private Mock<ControllerDescriptor> _mockControllerDescriptor;
        private List<CanonicalAttribute> _actionAttributes;
        private List<CanonicalAttribute> _controllerAttributes;

        [SetUp]
        public void Setup()
        {
            _actionAttributes = new List<CanonicalAttribute>();
            _controllerAttributes = new List<CanonicalAttribute>();

            _mockActionDescriptor = new Mock<ActionDescriptor>();
            _mockControllerDescriptor = new Mock<ControllerDescriptor>();
            _mockActionDescriptor.Setup(d => d.GetCustomAttributes(typeof(CanonicalAttribute), true))
                .Returns(() => _actionAttributes.Cast<object>().ToArray());
            _mockActionDescriptor.Setup(a => a.ControllerDescriptor)
                .Returns(_mockControllerDescriptor.Object);
            _mockControllerDescriptor.Setup(d => d.GetCustomAttributes(typeof(CanonicalAttribute), true))
                .Returns(() => _controllerAttributes.Cast<object>().ToArray());
        }

        [Test]
        public void AttributeHelper_WhenOnlyActionHasAttribute_ReturnsAttribute()
        {
            var mockAttribute = new CanonicalAttribute();
            _actionAttributes.Add(mockAttribute);

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.ShouldBe(mockAttribute);
        }

        [Test]
        public void AttributeHelper_WhenOnlyControllerHasAttribute_ReturnsAttribute()
        {
            var mockAttribute = new CanonicalAttribute();
            _controllerAttributes.Add(mockAttribute);

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.ShouldBe(mockAttribute);
        }

        [Test]
        public void AttributeHelper_WhenThereAreNoAttributes_ReturnsNull()
        {
            _controllerAttributes.Clear();
            _actionAttributes.Clear();

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.ShouldBeNull();
        }

        [Test]
        public void AttributeHelper_WhenBothControllerAndActionHasAnAttribute_ReturnsCombinedResult()
        {
            _controllerAttributes.Add(new CanonicalAttribute {Ruleset = "ruleset", Query = new [] {"a"}, Sensitive = new[] { "z" } });
            _actionAttributes.Add(new CanonicalAttribute { Ruleset = "ruleset", Query = new[] { "b" }, Sensitive = new[] { "y" } });

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.Ruleset.ShouldBe("ruleset");
            setting.Query.ShouldBe(new [] {"a", "b"});
            setting.Sensitive.ShouldBe(new [] {"z", "y"});
        }

        [Test]
        public void AttributeHelper_WhenAttributesHaveDifferentQueries_ShouldMergeQueryArray()
        {
            _controllerAttributes.Add(new CanonicalAttribute { Query = new[] { "a" } });
            _actionAttributes.Add(new CanonicalAttribute { Query = new[] { "b" }});

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.Query.ShouldBe(new[] { "a", "b" });
        }

        [Test]
        public void AttributeHelper_WhenAttributesHaveSomeDuplicateQueries_ShouldDedupeQueryValues()
        {
            _controllerAttributes.Add(new CanonicalAttribute { Query = new[] { "a" } });
            _actionAttributes.Add(new CanonicalAttribute { Query = new[] { "a", "b" } });

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.Query.ShouldBe(new[] { "a", "b" });
        }

        [Test]
        public void AttributeHelper_WhenAttributesHaveSomeDuplicateSensitives_ShouldDedupeSensitiveValues()
        {
            _controllerAttributes.Add(new CanonicalAttribute { Sensitive = new[] { "a" } });
            _actionAttributes.Add(new CanonicalAttribute { Sensitive = new[] { "a", "b" } });

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.Sensitive.ShouldBe(new[] { "a", "b" });
        }

        [Test]
        public void AttributeHelper_WhenActionAttributeHasNoDefinedRuleset_ShouldUseRulesetOnControllerAttribute()
        {
            _controllerAttributes.Add(new CanonicalAttribute { Ruleset = "ruleset" });
            _actionAttributes.Add(new CanonicalAttribute());

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.Ruleset.ShouldBe("ruleset");
        }

        [Test]
        public void AttributeHelper_WhenActionAndControllerAttributesHaveDefinedRuleset_ShouldUseRulesetOnActionAttribute()
        {
            _controllerAttributes.Add(new CanonicalAttribute { Ruleset = "controllerRuleset" });
            _actionAttributes.Add(new CanonicalAttribute { Ruleset = "actionRuleset" });

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.Ruleset.ShouldBe("actionRuleset");
        }

        [Test]
        public void AttributeHelper_WhenActionAttributeHasNoDefinedRouteName_ShouldUseRouteNameOnControllerAttribute()
        {
            _controllerAttributes.Add(new CanonicalAttribute { RouteName = "default" });
            _actionAttributes.Add(new CanonicalAttribute());

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.RouteName.ShouldBe("default");
        }

        [Test]
        public void AttributeHelper_WhenDifferentRouteNameProvidedOnControllerAndAction_ShouldUseRouteDefinedOnAction()
        {
            _controllerAttributes.Add(new CanonicalAttribute { RouteName = "default" });
            _actionAttributes.Add(new CanonicalAttribute { RouteName = "special" });

            var setting = AttributeHelper.GetCanonicalSettings(_mockActionDescriptor.Object);

            setting.RouteName.ShouldBe("special");
        }
    }
}
