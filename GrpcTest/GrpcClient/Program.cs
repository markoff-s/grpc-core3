using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcService.Protos;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new DataStreamer.DataStreamerClient(channel);

            var request = new DataRequest();
            var response = await client.GetDataAsync(request);
            Console.WriteLine($"GetDataAsync response = {response.Message}");

            using var xlTokenSource = new CancellationTokenSource();
            var xlToken = xlTokenSource.Token;
            using (var streamingCall = client.GetDataStream(request, cancellationToken: xlToken))
            {
                while (await streamingCall.ResponseStream.MoveNext(xlToken))
                {
                    var current = streamingCall.ResponseStream.Current;
                    Console.WriteLine($"GetDataStream response = {current.Message}");
                }
            }


            Console.ReadLine();
            xlTokenSource.Cancel();
        }
    }
}
