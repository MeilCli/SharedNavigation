using Fody;
using SharedNavigation.NETStandard.Fody;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedNavigation.NETStandard.Test
{
    public class WeaverTest
    {
        public static TestResult TestResult { get; }

        static WeaverTest()
        {
            var weavingTask = new ModuleWeaver();
            TestResult = weavingTask.ExecuteTestRun("SharedNavigation.NETStandard.Test.Assembly.dll", false);
        }
    }
}
