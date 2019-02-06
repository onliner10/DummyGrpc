using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DummyGrpc;
using Grpc.Core;

namespace Meter
{
    class Program
    {
        public static Func<Measurement> GrpcTest()
        {
            var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            var client = new DummyGrpcServer.DummyGrpcServerClient(channel);

            return () =>
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
            };
           
        }

        static void Main(string[] args)
        {
            var n = int.Parse(args[0]);
            var concurrent = int.Parse(args[1]);

            var requestsDone = 0;
     
            var orchestrator = new ThreadOrchestrator<Measurement>(n, concurrent, GrpcTest());
            var measurements = orchestrator.Start().OrderBy(x=>x.Start);

            var csvBuilder = new StringBuilder();
            csvBuilder.Append("Start;DurationMs;Success");
            csvBuilder.Append(Environment.NewLine);
            foreach (var measurement in measurements)
            {
                csvBuilder.AppendFormat("{0};{1};{2}", measurement.Start.ToString(CultureInfo.InvariantCulture),
                    measurement.Duration.TotalMilliseconds, measurement.IsSuccessfull ? "1" : "0");
                csvBuilder.Append(Environment.NewLine);
            }
            File.WriteAllText("testResults.csv", csvBuilder.ToString());

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
