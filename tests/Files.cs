using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PerfSmokeTests {
    public class Files : PerfTest {
        protected override List<TestDelegate> Tests {
            get {
                return new List<TestDelegate> {
                    FilestreamRead,
                    FilestreamWrite,
                };
            }
        }

        private static string GetTestFilePath() {
            return Path.Combine(Environment.CurrentDirectory, $@"{DateTime.Now.Ticks}.tmp");
        }

        private static string CreateFile(int size) {
            string testFile = GetTestFilePath();
            byte[] bytes = new byte[size];
            new Random(531033).NextBytes(bytes);
            File.WriteAllBytes(testFile, bytes);
            return testFile;
        }

        public long FilestreamRead() {
            const int BUFFER_SIZE = 4096;

            byte[] bytes = new byte[BUFFER_SIZE];

            var stopwatch = new Stopwatch();

            string filePath = CreateFile(BUFFER_SIZE);
            for (int i = 0; i < iterations; i++) {
                using (FileStream reader = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    BUFFER_SIZE,
                    FileOptions.None))
                {
                    stopwatch.Start();
                    reader.Read(bytes, 0, bytes.Length);
                    stopwatch.Stop();
                }
            }
            File.Delete(filePath);

            return stopwatch.ElapsedMilliseconds;
        }

        public long FilestreamWrite() {
            const int BUFFER_SIZE = 4096;

            byte[] bytes = new byte[BUFFER_SIZE];
            new Random(531033).NextBytes(bytes);

            var stopwatch = new Stopwatch();

            string filePath = GetTestFilePath();
            for (int i = 0; i < iterations; i++) {
                using (FileStream writer = new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Read,
                    BUFFER_SIZE,
                    FileOptions.None))
                {
                    stopwatch.Start();
                    writer.Write(bytes, 0, bytes.Length);
                    stopwatch.Stop();
                }
            }
            File.Delete(filePath);

            return stopwatch.ElapsedMilliseconds;
        }
    }
}

