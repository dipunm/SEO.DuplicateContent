using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReturnNull.CanonicalRoutes.Helpers;
using Shouldly;

namespace ReturnNull.CanonicalRoutes.Tests
{
    [TestFixture]
    public class QuerystringHelperTests
    {
        [Test]
        public void ToKeyValuePairs_GivenAnEmptyString_ShouldReturnEmpty()
        {
            "".ToKeyValuePairs().ShouldBeEmpty();
        }

        [Test]
        public void ToKeyValuePairs_StartingWithQuestionMark_ShouldExcludeFromResults()
        {
            "?key1=value1&key2=value2".ToKeyValuePairs().ShouldAllBe(pair => !pair.Key.Contains("?"));
        }

        [Test]
        public void ToKeyValuePairs_GivenNParameters_ShouldReturnNPairs()
        {
            const int N = 4;
            "key=value&key2=val&key3=value&key4=30".ToKeyValuePairs().Count().ShouldBe(N);
        }

        [Test]
        public void ToKeyValuePairs_WhenNoEquals_ShouldCreateNullValue()
        {
            "key".ToKeyValuePairs().Single().Value.ShouldBeNull();
        }

        [Test]
        public void ToKeyValuePairs_WhenNoEquals_ShouldTreatStringAsKey()
        {
            const string key = "key";
            key.ToKeyValuePairs().Single().Key.ShouldBe(key);
        }

        [Test]
        public void ToKeyValuePairs_WhenMultipleSubsequentAmpersands_ShouldTreatAsOneAmpersand()
        {
            "key=value&&key2=value2".ToKeyValuePairs().Count().ShouldBe(2);
        }

        [Test]
        public void ToKeyValuePairs_WhenDuplicateKeys_ShouldHaveAllKeyValuePairs()
        {
            "key=value&key=value2".ToKeyValuePairs().Count().ShouldBe(2);
        }

        [Test]
        public void ToKeyValuePairs_WhenMultipleEquals_ShouldTreatAnythingAfterFirstEqualAsValue()
        {
            "key=v=a=lue".ToKeyValuePairs().Single().Value.ShouldBe("v=a=lue");
        }

        [Test]
        public void ToQuerystring_WhenMultiplePairsProvided_ShouldJoinWithAmpersand()
        {
            new[]
            {
                new KeyValuePair<string, string>("key", "value"),
                new KeyValuePair<string, string>("key", "value2")
            }.ToQuerystring().ShouldBe("key=value&key=value2");
        }

        [Test]
        public void ToQuerystring_WhenPairsProvided_ShouldJoinKeyAndValueWithEqual()
        {
            new [] { new KeyValuePair<string, string>("key", "value")}.ToQuerystring().ShouldBe("key=value");
        }

        [Test]
        public void ToQuerystring_WhenPairValueIsNull_ShouldNotIncludeEqualCharacter()
        {
            new[] { new KeyValuePair<string, string>("key", null) }.ToQuerystring().ShouldBe("key");
        }
    }
}