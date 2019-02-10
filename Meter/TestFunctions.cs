using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DummyGrpc;
using Grpc.Core;
using Newtonsoft.Json;
using RestSharp;

namespace Meter
{
    public static class TestFunctions
    {
        public static Func<Measurement> GrpcTest(string ip)
        {
            var channel = new Channel($"{ip}:50051", ChannelCredentials.Insecure);
            var client = new DummyGrpcServer.DummyGrpcServerClient(channel);

            return () =>
            {
                var measurementStart = DateTimeOffset.UtcNow;
                try
                {
                    client.TestMethod(new TestRequest()
                    {
                        Prop1 = Guid.NewGuid().ToString(),
                        Prop2 = Guid.NewGuid().ToString(),
                        Prop3 = Guid.NewGuid().ToString(),
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new Measurement(measurementStart, DateTimeOffset.UtcNow, ex.Message);
                }

                return new Measurement(measurementStart, DateTimeOffset.UtcNow);
            };
        }

        public static Func<Measurement> RestTest(string ip)
        {
            var requestModel = new TestRequest()
            {
                Prop1 = Guid.NewGuid().ToString(),
                Prop2 = Guid.NewGuid().ToString(),
                Prop3 = Guid.NewGuid().ToString(),
                Prop4 = Int32.MinValue,
                Prop5 = Int32.MaxValue,
                Prop6 = Int32.MaxValue / 7,
                Prop7 = Double.MaxValue,
                Prop8 = double.MinValue,
                Prop9 = double.MaxValue / 7,
                Prop10 = true,
                Prop11 = false,
                Prop12 = true
            };

            var client = new RestClient($"http://{ip}:5000");
            string jsonToSend = JsonConvert.SerializeObject(requestModel);

            return () =>
            {
                var measurementStart = DateTimeOffset.UtcNow;
                try
                {
                    var r = new RestRequest("/api/test", Method.POST);
                    r.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
                    r.RequestFormat = DataFormat.Json;

                    var response = client.Execute(r);
                    if (!response.IsSuccessful)
                    {
                        var error = $"Error: {response.ErrorMessage}, {response.StatusDescription}, {response.StatusCode}";
                        Console.WriteLine(error);
                        return new Measurement(measurementStart, DateTimeOffset.UtcNow, error);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new Measurement(measurementStart, DateTimeOffset.UtcNow, ex.Message);
                }

                return new Measurement(measurementStart, DateTimeOffset.UtcNow);
            };
        }

    }
}
