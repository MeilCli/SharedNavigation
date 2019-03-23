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
            TestResult = weavingTask.ExecuteTestRun(
                assemblyPath: "SharedNavigation.NETStandard.Test.Assembly.dll",
                // if true: run peverify, but it throw "The files have difference peverify results." because output path is different.
                runPeVerify: false,
                ignoreCodes: new[] { "0x80131869" }
            );
        }
    }
}
