using System;
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
            new ThreadOrchestrator<Measurement>(2000, 100, testMethod).Start();
            Console.WriteLine("Warmup done");

            Console.WriteLine("Starting test");
            var testStartTime = DateTimeOffset.UtcNow;
            var orchestrator = new ThreadOrchestrator<Measurement>(n, concurrent, testMethod);
            var measurements = orchestrator.Start().OrderBy(x=>x.Start);

            var csvBuilder = new StringBuilder();
            csvBuilder.Append("Start;DurationMs;Success;Error");
            csvBuilder.Append(Environment.NewLine);

            foreach (var measurement in measurements)
            {
                csvBuilder.AppendFormat("{0};{1};{2};{3}", (measurement.Start - testStartTime).TotalMilliseconds,
                    measurement.Duration.TotalMilliseconds, measurement.IsSuccessfull ? "1" : "0", measurement.ErrorReason ?? "");
                csvBuilder.Append(Environment.NewLine);
            }
            File.WriteAllText($"{args[3]}.csv", csvBuilder.ToString());

            Console.WriteLine("Experiment done!");
        }
    }
}
