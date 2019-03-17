using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SharedNavigation.NETStandard.Test
{
    [TestClass]
    public class FinderTest
    {
        private string createMessage(string type, string message)
        {
            return $"SNNF: SharedNavigation.NETStandard.Test.Assembly.{type}: {message}";
        }

        private void assertMessage(string message)
        {
            Assert.IsTrue(WeaverTest.TestResult.Messages.Where(x => x.Text == message).Any());
        }

        private void assertNotMessage(string message)
        {
            Assert.IsFalse(WeaverTest.TestResult.Messages.Where(x => x.Text == message).Any());
        }

        private void assertWarning(string message)
        {
            Assert.IsTrue(WeaverTest.TestResult.Warnings.Where(x => x.Text == message).Any());
        }

        private void assertNotWarning(string message)
        {
            Assert.IsFalse(WeaverTest.TestResult.Warnings.Where(x => x.Text == message).Any());
        }

        #region For isNavigationView
        [TestMethod]
        public void TestIsNavigationView_IsInterface()
        {
            assertMessage(createMessage("IFinderView1", "is interface"));
        }

        [TestMethod]
        public void TestIsNavigationView_HasNotInterface()
        {
            assertMessage(createMessage("FinderView2", "has not interface"));
        }

        [TestMethod]
        public void TestIsNavigationView_HasNotMethod()
        {
            // Unable test, because C# code can not make class that dose not contain method.
            // assertMessage(createMessage("FinderView3", "has not method"));
        }

        [TestMethod]
        public void TestIsNavigationView_NotImplment()
        {
            assertMessage(createMessage("FinderView4", "not implement INavigationView"));
        }

        [TestMethod]
        public void TestIsNavigationView_ReturnTrue()
        {
            assertNotMessage(createMessage("FinderView5", "is interface"));
            assertNotMessage(createMessage("FinderView5", "has not interface"));
            assertNotMessage(createMessage("FinderView5", "has not method"));
            assertNotMessage(createMessage("FinderView5", "not implement INavigationView"));
        }

        [TestMethod]
        public void TestIsNavigationView_ExtendInterface()
        {
            assertNotMessage(createMessage("FinderView6", "is interface"));
            assertNotMessage(createMessage("FinderView6", "has not interface"));
            assertNotMessage(createMessage("FinderView6", "has not method"));
            assertNotMessage(createMessage("FinderView6", "not implement INavigationView"));
        }

        [TestMethod]
        public void TestIsNavigationView_ExtendClass()
        {
            // Not corresnpond to extended class, now
            assertMessage(createMessage("FinderView7", "has not interface"));
        }
        #endregion

        #region For findNavigateMethods
        [TestMethod]
        public void TestFindNavigateMethods_HasNotAttribute()
        {
            assertMessage(createMessage("FinderView20", "PushAsync: has not NavigateAttribute"));
        }

        [TestMethod]
        public void TestFindNavigateMethods_HasNotParameter()
        {
            assertWarning(createMessage("FinderView21", "PushAsync: NavigateAttribute method must have 1 parameter"));
        }

        [TestMethod]
        public void TestFindNavigateMethods_HasMoreParameter()
        {
            assertWarning(createMessage("FinderView22", "PushAsync: NavigateAttribute method must have 1 parameter"));
        }

        [TestMethod]
        public void TestFindNavigateMethods_ReturnBool()
        {
            assertWarning(createMessage("FinderView23", "PushAsync: NavigateAttribute method must return Task"));
        }

        [TestMethod]
        public void TestFindNavigateMethods_ReturnGenericsTask()
        {
            assertWarning(createMessage("FinderView24", "PushAsync: NavigateAttribute method must return Task"));
        }

        [TestMethod]
        public void TestFindNavigateMethods_ReturnTrue()
        {
            assertNotMessage(createMessage("FinderView25", "PushAsync: has not NavigateAttribute"));
            assertNotWarning(createMessage("FinderView25", "PushAsync: NavigateAttribute method must have 1 parameter"));
            assertNotWarning(createMessage("FinderView25", "PushAsync: NavigateAttribute method must return Task"));
        }
        #endregion

        #region For findCanNavigateMethods
        [TestMethod]
        public void TestFindCanNavigateMethods_HasNotAttribute()
        {
            assertMessage(createMessage("FinderView40", "CanPush: has not CanNavigateAttribute"));
        }

        [TestMethod]
        public void TestFindCanNavigateMethods_HasNotParameter()
        {
            assertWarning(createMessage("FinderView41", "CanPush: CanNavigateAttribute method must have 1 parameter"));
        }

        [TestMethod]
        public void TestFindCanNavigateMethods_HasMoreParameter()
        {
            assertWarning(createMessage("FinderView42", "CanPush: CanNavigateAttribute method must have 1 parameter"));
        }

        [TestMethod]
        public void TestFindNavigateMethods_ReturnString()
        {
            assertWarning(createMessage("FinderView43", "CanPush: CanNavigateAttribute method must return bool"));
        }

        [TestMethod]
        public void TestFindCanNavigateMethods_ReturnTrue()
        {
            assertNotMessage(createMessage("FinderView44", "CanPush: has not CanNavigateAttribute"));
            assertNotWarning(createMessage("FinderView44", "CanPush: CanNavigateAttribute method must have 1 parameter"));
            assertNotWarning(createMessage("FinderView44", "CanPush: CanNavigateAttribute method must return Task"));
        }
        #endregion
    }
}
