using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PerfSmokeTests {
    public class Strings : PerfTest {
        protected override List<TestDelegate> Tests {
            get {
                return new List<TestDelegate> {
                    StartsWith, // We know this one fails right now.
                    StartsWithOrdinal,
                    Format,
                    Compare,
                    CompareOrdinal,
                };
            }
        }

        private IEnumerable<string> collection = new[] {
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "ZYXWVUTSRQPONMLKJIHGFEDCBA",
        };

        public long StartsWith() {
            var stopwatch = new Stopwatch();
            const string SAMPLE_STRING = "SAMPLETEXTSTRING";

            stopwatch.Start();
            for (int i = 0; i < iterations; i++) {
                foreach (string item in collection) {
                    SAMPLE_STRING.StartsWith(item);
                }
            }
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        public long StartsWithOrdinal() {
            var stopwatch = new Stopwatch();
            const string SAMPLE_STRING = "SAMPLETEXTSTRING";

            stopwatch.Start();
            for (int i = 0; i < iterations; i++) {
                foreach (string item in collection) {
                    SAMPLE_STRING.StartsWith(item, StringComparison.Ordinal);
                }
            }
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        public long Format() {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            for (int i = 0; i < iterations; i++) {
                foreach (string item in collection) {
                    String.Format("{0} asdfasdfasfdasf", collection);
                }
            }
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        public long Compare() {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            for (int i = 0; i < iterations; i++) {
                foreach (string lhs in collection) {
                    foreach (string rhs in collection) {
                        String.Compare(lhs, rhs);
                    }
                }
            }
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        public long CompareOrdinal() {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            for (int i = 0; i < iterations; i++) {
                foreach (string lhs in collection) {
                    foreach (string rhs in collection) {
                        String.Compare(lhs, rhs, StringComparison.Ordinal);
                    }
                }
            }
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}
