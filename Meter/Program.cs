using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Meter
{
    class Program
    {
        static void Main(string[] args)
        {
            var n = int.Parse(args[0]);
            var concurrent = int.Parse(args[1]);
            var ip = args[2];
            var testMethod = args[3] == "grpc" ? TestFunctions.GrpcTest(ip) : TestFunctions.RestTest(ip);

            Console.WriteLine("Warming up..");
            new ThreadOrchestrator<Measurement>(100, 10, testMethod).Start();
            Console.WriteLine("Warmup done");

            Console.WriteLine("Starting test");
            var orchestrator = new ThreadOrchestrator<Measurement>(n, concurrent, testMethod);
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
