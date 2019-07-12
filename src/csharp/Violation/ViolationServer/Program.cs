using Grace.DependencyInjection;
using Grpc.Core;
using Grpc.Core.Logging;
using System;
using Bsvt.Common;

namespace Bsvt.Stroibat
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = args[1];
            var port = int.Parse(args[2]);
            var container = new DependencyInjectionContainer();

            var singletonLife = new Grace.DependencyInjection.Lifestyle.SingletonLifestyle();

            container.Configure(c => c.ExportAs<FilePathProvider, IPathProvider>());
            container.Configure(c => c.ExportAs<FileRepository<Violation>, IProtoRepository<Violation>>().UsingLifestyle(singletonLife));
            container.Configure(c => c.ExportAs<ConsoleLogger, ILogger>());
            container.Configure(c => c.ExportAs<ViolationServerImpl, ViolationService.ViolationServiceBase>());

            Server server = new Server
            {
                Services = { ViolationService.BindService(container.Locate<ViolationService.ViolationServiceBase>()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            try
            {
                server.Start();

                Console.WriteLine($"Server listening {host}:{port}");
                Console.WriteLine("Press any key to stop the server...");
                Console.ReadKey();
            }
            finally
            {
                server.ShutdownAsync().Wait();
            }

        }
    }
}
