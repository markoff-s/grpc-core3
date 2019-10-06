using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcService.Protos;
using Microsoft.Extensions.Logging;

namespace GrpcService.Services
{
    public class DataStreamerService : DataStreamer.DataStreamerBase
    {
        private readonly ILogger<DataStreamerService> _logger;
        public DataStreamerService(ILogger<DataStreamerService> logger)
        {
            _logger = logger;
        }

        public override async Task<DataResponse> GetData(DataRequest request, ServerCallContext context)
        {
//            await Task.Delay(2000);

            var response = new DataResponse
            {
                Message = $"What's up at {DateTime.Now}'"
            };

            return response;
        }

        public override async Task GetDataStream(
            DataRequest request, 
            IServerStreamWriter<DataResponse> responseStream, 
            ServerCallContext context)
        {
            await foreach (var message in GetMessages(context.CancellationToken))
            {
                var response = new DataResponse
                {
                    Message = message
                };

                await responseStream.WriteAsync(response);
            }
        }

        private async IAsyncEnumerable<string> GetMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5000, cancellationToken);

                yield return $"What's up from stream at {DateTime.Now}'";
            }
        }
    }
}