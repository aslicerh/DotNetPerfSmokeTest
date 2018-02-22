using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace PerfSmokeTests {
    public class Network : PerfTest {
        protected override List<TestDelegate> Tests {
            get {
                return new List<TestDelegate> {
                    AsyncSendReceive,
                };
            }
        }

        private static async Task OpenLoopbackConnectionAsync(Func<Socket, Socket, Task> func)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                Task<Socket> acceptTask = listener.AcceptAsync();
                Task connectTask = client.ConnectAsync(listener.LocalEndPoint);

                await await Task.WhenAny(acceptTask, connectTask);
                await Task.WhenAll(acceptTask, connectTask);

                using (Socket server = await acceptTask)
                {
                    await func(client, server);
                }
            }
        }

        public long AsyncSendReceive() {
            const int BUFFER_SIZE = 256;
            var stopwatch = new Stopwatch();
            OpenLoopbackConnectionAsync(async (client, server) =>
            {
                byte[] clientBuffer = new byte[BUFFER_SIZE];
                new Random(531033).NextBytes(clientBuffer);
                byte[] serverBuffer = new byte[BUFFER_SIZE];

                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    Task r = client.SendAsync(clientBuffer, SocketFlags.None);
                    await server.ReceiveAsync(serverBuffer, SocketFlags.None);
                    await r;
                }
                stopwatch.Stop();
            }).Wait();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}

