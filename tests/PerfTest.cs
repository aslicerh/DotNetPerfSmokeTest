using System;
using System.Collections.Generic;

namespace PerfSmokeTests {
    public abstract class PerfTest {
        protected delegate long TestDelegate();
        protected abstract List<TestDelegate> Tests { get; }
        public int iterations;

        public void RunTests() {
            foreach (var test in Tests) {
                long time = test();
                Console.WriteLine($"{this.GetType().Name}-{test.Method.Name} : {time}ms");
            }
        }
    }
}

