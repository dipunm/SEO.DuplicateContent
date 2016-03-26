using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace ReturnNull.CanonicalRoutes.Tests
{
        /**

        Test each Rule:
            - Remove Uncanonical Querystrings
            - Remove Trailing Slash
            - Remove Repeating Slashes
            - Order Querystrings
            - Omit Default Route Values
            - Match Case With Route
            - Lowercase Querystring Values
            - Lowercase Querystring Keys
            - Enforce Scheme
            - Enforce Host
            - Enforce Correct Slug

        Test ActionFilter
            - Finds attribute on action
            - Finds attribute on controller
            - Finds attribute on base controller
            - Merges values properly
            
            - Behaves when no canonicalizer found -- probably should inform why it failed, but don't fail silently
            - Responds to canonicalizer response properly.

        Test Canonicalizer
            - 
        Test QuerystringHelper
        
        Test CanonicalCollection
        
        */



    [TestFixture]
    public class Class1
    {
        [Test]
        public void MyTest()
        {
            
        }
    }
}
