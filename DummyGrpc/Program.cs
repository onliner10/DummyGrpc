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
                Prop1 = Guid.NewGuid().ToString(),
                Prop2 = Guid.NewGuid().ToString(),
                Prop3 = Guid.NewGuid().ToString(),
                Prop4 = int.MinValue,
                Prop5 = int.MaxValue,
                Prop6 = int.MaxValue / 7,
                Prop7 = double.MaxValue,
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
                Ports = { new ServerPort("0.0.0.0", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
