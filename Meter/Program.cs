using System;
using DummyGrpc;
using Grpc.Core;

namespace Meter
{
    class Program
    {
        static void Main(string[] args)
        {
            var n = int.Parse(args[0]);
            var concurrent = int.Parse(args[1]);

            var requestsDone = 0;
            var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new DummyGrpcServer.DummyGrpcServerClient(channel);

            var orchestrator = new ThreadOrchestrator<Measurement>(n, concurrent, () =>
            {
                var measurementStart = DateTimeOffset.UtcNow;
                try
                {
                    client.TestMethod(new TestRequest()
                    {
                        Prop1 = "this is the test string",
                        Prop2 = "1235 tr xzx eeef aasdsadasx",
                        Prop3 = "fdkfmsdlkfm rmfk fsdmklfs df ewr ewre fds",
                        Prop4 = Int32.MinValue,
                        Prop5 = Int32.MaxValue,
                        Prop6 = Int32.MaxValue / 7,
                        Prop7 = Double.MaxValue,
                        Prop8 = double.MinValue,
                        Prop9 = double.MaxValue / 7,
                        Prop10 = true,
                        Prop11 = false,
                        Prop12 = true
                    });
                }
                catch (Exception)
                {
                    return new Measurement(measurementStart, DateTimeOffset.UtcNow, false);
                }

                return new Measurement(measurementStart, DateTimeOffset.UtcNow, true);
            });
            var measurements = orchestrator.Start();

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
