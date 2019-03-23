using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedNavigation.NETStandard.Test.Assembly;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SharedNavigation.NETStandard.Test
{
    [TestClass]
    public class SimpleTest
    {
        private class OtherViewModel : INavigationViewModel
        {
            public void RestoreState(byte[] state)
            {
                throw new NotImplementedException();
            }

            public byte[] SaveState()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            //Assert.AreEqual(string.Empty, string.Join('\n', WeaverTest.TestResult.Messages.Select(x => x.Text)));

            Type viewType = WeaverTest.TestResult.Assembly.GetType("SharedNavigation.NETStandard.Test.Assembly.SimpleView");
            Type viewModelType = WeaverTest.TestResult.Assembly.GetType("SharedNavigation.NETStandard.Test.Assembly.SimpleViewModel");
            Type[] nestedType = viewType.GetNestedTypes(BindingFlags.NonPublic);
            Type navigationActionType = nestedType.First(x => typeof(INavigationAction).IsAssignableFrom(x));
            ConstructorInfo navigationActionConstructor = navigationActionType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { viewType },
                null
            );

            MethodInfo createGenericNavigateAsyncMethod(Type genericType)
            {
                return navigationActionType
                .GetMethod("NavigateAsync")
                .MakeGenericMethod(genericType);
            }

            var view = (dynamic)Activator.CreateInstance(viewType);
            var viewModel = Activator.CreateInstance(viewModelType);
            var navigationAction = Activator.CreateInstance(navigationActionType, new object[] { view });

            await (Task)createGenericNavigateAsyncMethod(viewModelType).Invoke(navigationAction, new object[] { viewModel });

            Assert.AreEqual(true, view.IsPushAsyncCalled);

            view.IsPushAsyncCalled = false;

            await (Task)createGenericNavigateAsyncMethod(typeof(OtherViewModel)).Invoke(navigationAction, new object[] { new OtherViewModel() });

            Assert.AreEqual(false, view.IsPushAsyncCalled);
        }
    }
}
