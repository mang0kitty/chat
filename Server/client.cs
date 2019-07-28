using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class Client {
    public Client(string hostname = "localhost", int port = 8000) {
        this.Connection = new TcpClient();
        Hostname = hostname;
        Port = port;
    }

    public string Hostname { get; }

    public int Port { get; }

    private TcpClient Connection { get; set; }

    public async Task ConnectAsync() {
        await this.Connection.ConnectAsync(this.Hostname, this.Port);
    }

    public async Task SendMessageAsync(string user, string message) {
        await SendRawAsync($".MSG {user} {message}");
    }

    public async Task SendRawAsync(string payload) {
        using (var sr = new StreamWriter(this.Connection.GetStream(), encoding: Encoding.UTF8, bufferSize: 1024, leaveOpen: true)) {
            await sr.WriteLineAsync(payload);
        }
    }

    public async Task<Message> ReceiveMessageAsync() {
        using(var sr = new StreamReader(this.Connection.GetStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true)) {
            while(true) {
                var line = await sr.ReadLineAsync();

                // TODO: Use the full parser for messages here
                var parts = line.Split(' ');
                switch (parts[0]) {
                    case ".MSG":
                        return new Message(parts[1], parts[2]);
                    case ".PING":
                        await SendRawAsync(".PING");
                        break;
                }
            }
        }
    }
}

public class Message {
    public Message(string user, string payload) {
        User = user;
        Payload = payload;
    }

    public string User {get; private set;}

    public string Payload {get; private set;}
}

