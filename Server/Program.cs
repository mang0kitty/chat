using System;
using System.Threading.Tasks;

namespace chat
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync() {
            await Task.WhenAll(RunServer(), RunClient());
        }

        static async Task RunServer() {
            var server = new Server(port: 8000);
            await server.RunAsync();
        }

        static async Task RunClient() {
            var client = new Client(port: 8000);
            await client.ConnectAsync();

            await client.SendMessageAsync(user: "Aideen", message: "Hello");
            var message = await client.ReceiveMessageAsync();
            Console.WriteLine($"{message.User}: {message.Payload}");
        }
    }
}
