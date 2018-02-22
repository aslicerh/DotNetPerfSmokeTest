using System;
using System.Collections.Generic;

namespace PerfSmokeTests {
    class DotnetPerfSmokeTests {
        public const int DEFAULT_ITERATIONS = 10000;
        private static List<PerfTest> categories = new List<PerfTest> {
            new PerfSmokeTests.Strings { iterations = DEFAULT_ITERATIONS * 4 },
            new PerfSmokeTests.Network { iterations = DEFAULT_ITERATIONS },
            new PerfSmokeTests.Files { iterations = DEFAULT_ITERATIONS / 2 },
        };

        static void Main(string[] args) {
            Console.WriteLine("Running Tests...\n");
            foreach(var category in categories) {
                category.RunTests();
            }
        }
    }
}


