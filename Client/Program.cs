using System;
using System.Threading.Tasks;

namespace Chat.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync() {
            var client = new Chat.Client.Client(port: 8000);
            await client.ConnectAsync();

            await client.SendMessageAsync(user: "Aideen", message: "Hello");
            var message = await client.ReceiveMessageAsync();
            Console.WriteLine($"{message.User}: {message.Payload}");
        }
    }
}
