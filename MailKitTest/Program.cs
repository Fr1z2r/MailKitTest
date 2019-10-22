using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;

namespace MailKitTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Task.Run(() => new ImapServerMock().Start());
            using (var client = new ImapClient(new ProtocolLogger(Console.OpenStandardOutput())))
            {
                await client.ConnectAsync("127.0.0.1", 12345);
            }

            Console.WriteLine("Done!");
        }
    }

    class ImapServerMock
    {
        public async Task Start()
        {
            var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
            
            server.Start();

            using (var client = server.AcceptTcpClient())
            {
                using (var stream = client.GetStream())
                {
                    var m1 = Encoding.UTF8.GetBytes(
                        "* OK Yandex IMAP4rev1 at sas8-bccc92f57f23.qloud-c.yandex.net:993 ready to talk with, 2019-Oct-18 07:41:00, 0fHtH613ZiE1\r\n");
                    stream.Write(m1, 0, m1.Length);

                    
                    var m2 = Encoding.UTF8.GetBytes(
                        "* BYE Autologout; idle for too long\r\n");

                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(1000);
                        stream.Write(m2, 0, m2.Length);
                    }
                }
            }
        }
    }
}