using System;
using DummyGrpc;
using Grpc.Core;

namespace Meter
{
    class Program
    {
        static void Main(string[] args)
        {
            var requestsDone = 0;
            var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new DummyGrpcServer.DummyGrpcServerClient(channel);

            var reply = client.TestMethod(new TestRequest()
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
            Console.WriteLine("Greeting: " + reply.Prop1);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
