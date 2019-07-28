using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Server {
    public Server(int port) {
        this.Listener = new TcpListener(IPAddress.Any, port);
    }

    public bool Running { get; private set; }

    public async Task RunAsync() {
        this.Running = true;
        this.Listener.Start();

        var clientTaskList = new List<Task>();

        while (this.Running) {
            var client = await this.Listener.AcceptTcpClientAsync();
            clientTaskList.Add(RunTcpClientAsync(client));
        }

        await Task.WhenAll(clientTaskList);
    }

    public void Stop() {
        this.Running = false;
        this.Listener.Stop();
    }

    private async Task RunTcpClientAsync(TcpClient client) {
        var stream = client.GetStream();
        using (var sr = new StreamReader(stream))
        using (var sw = new StreamWriter(stream))
        {
            var line = await sr.ReadLineAsync();

            // TODO: Process the command
            Console.WriteLine(line);

            await sw.WriteLineAsync(line);
        }
    }

    private TcpListener Listener { get; set; }
}

