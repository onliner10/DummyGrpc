using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace DummyGrpc
{
    class GrpcServer : DummyGrpcServer.DummyGrpcServerBase
    {
        public override Task<TestReply> TestMethod(TestRequest request, ServerCallContext context)
        {
            return Task.FromResult(new TestReply()
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
    }

    class Program
    {
        const int Port = 50051;

        static void Main(string[] args)
        {
            var server = new Server
            {
                Services = { DummyGrpcServer.BindService(new GrpcServer()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
