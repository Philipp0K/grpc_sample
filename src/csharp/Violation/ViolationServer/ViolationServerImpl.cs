using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Logging;
using Bsvt.Common;

namespace Bsvt.Stroibat
{
    internal class ViolationServerImpl : ViolationService.ViolationServiceBase
    {
        IProtoRepository<Violation> repository;
        ILogger logger;

        public ViolationServerImpl(IProtoRepository<Violation> repository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public override Task<AddViolationResponse> AddViloation(AddViloationRequest request, ServerCallContext context)
        {
            logger.Info(request.ToString());
            var vi = new Violation();
            vi.CameraId = request.CameraId;
            vi.StartTime = request.StartTime;
            vi.StopTime = request.StopTime;
            int id = repository.Add(vi);
            var resp = new AddViolationResponse();
            resp.Id = id;
            return Task.FromResult(resp);
        }

        public override Task<Violation> GetViolation(GetViolationRequest request, ServerCallContext context)
        {
            logger.Info(request.ToString());
            var item = repository.Get(request.Id);
            var resp = new GetViolationResponse() { Item = item };
            logger.Info(resp.ToString());
            return Task.FromResult(item);
        }


        public override async Task EnumerateViolations(Empty request, IServerStreamWriter<Violation> responseStream, ServerCallContext context)
        {
            logger.Info(request.ToString());
            foreach (var d in repository)
            {
                await responseStream.WriteAsync(d);
            }
        }
    }
}
