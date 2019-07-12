using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;

namespace Bsvt.Stroibat
{
    class Program
    {
        public class ViolationClient : ViolationService.ViolationServiceClient
        {
            public ViolationClient(Grpc.Core.Channel channel) : base(channel)
            {
            }

            public async Task ViewAll()
            {

            }
        }

        static void Main(string[] args)
        {
            var host = args[1];
            var port = int.Parse(args[2]);
            var targ = $"{host}:{port}";
            var channel = new Channel(targ, ChannelCredentials.Insecure);
            var client = new ViolationClient(channel);

            char k = '0';
            while(true)
            {
                if (k == 'a')
                {
                    Console.Write("Set CameraID: ");
                    var req = new AddViloationRequest();
                    req.CameraId = int.Parse(Console.ReadLine());
                    req.StartTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow);
                    req.StopTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddMinutes(1));

                    var resp = client.AddViloation(req);

                    var back = client.GetViolation(new GetViolationRequest() { Id = resp.Id });

                    Console.WriteLine($"Request  : {req}");
                    Console.WriteLine($"Response : {resp}");
                    Console.WriteLine($"Back data: {back}");
                }
                else if (k == 'x')
                {
                    Console.WriteLine("adding 1000 items");
                    var beg = DateTime.Now;
                    for (int i = 0; i < 1000; i++)
                    {
                        var req = new AddViloationRequest();
                        req.CameraId = 1;
                        req.StartTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow);
                        req.StopTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddMinutes(1));
                        var resp = client.AddViloation(req);
                    }
                    var end = DateTime.Now;
                    var delta = end - beg;
                    Console.WriteLine($"1000 items added in {delta}");
                }
                else if (k == 'v')
                {
                    ViewRepo(client);
                }
                else if (k == 'q')
                    break;

                Console.Write("Press 'a' to add, 'v' to view, 'x' to spam, 'q' to exit ...");
                k = Console.ReadKey().KeyChar;
                Console.Clear();
            }

            channel.ShutdownAsync().Wait();
        }

        static async Task ViewRepo(ViolationClient client)
        {
            using (var resp = client.EnumerateViolations(new Google.Protobuf.WellKnownTypes.Empty()))
            {
                var stream = resp.ResponseStream;
                Console.WriteLine($"Elements:");
                while (await stream.MoveNext())
                {
                    var d = stream.Current;
                    Console.WriteLine($"\t{d}");
                }
            }
        }
    }

}
