using Grpc.Net.Client;

namespace Demo.Basket.GRPCClient
{
    public class Client
    {
        public static void Main() { }

        public static async Task<string> GetAllAsync()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.ResponseAsync(new ClientRequest());
            return reply.Message;
        }
    }
}