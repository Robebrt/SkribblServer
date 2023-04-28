using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace SkribblServer
{
    internal class Client
    {
        string clNo;
        Socket socket;
        public void startClient(Socket socketIn, string clientNo)
        {
            this.socket = socketIn;
            this.clNo = clientNo;
            Thread clientThread = new Thread(doChat);
            clientThread.Start();
        }
        public void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    int noBytesRecieved = socket.Receive(bytesFrom);
                    if(noBytesRecieved == 0)
                    {
                        Server.clientList.Remove(Int32.Parse(clNo));
                        break;
                    }    
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("<EOF>"));
                    Console.WriteLine(" >> " + "From client-" + clNo + dataFromClient);

                    rCount = Convert.ToString(requestCount);
                    serverResponse = "Cient " + clNo + " said: " + dataFromClient;
                    sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    Console.WriteLine(Server.clientList.Count());
                    foreach (Client client in Server.clientList.Values)
                    {
                        try
                        {
                            int bytesSend = client.socket.Send(sendBytes);
                            //Console.WriteLine($"{client.socket.RemoteEndPoint} {bytesSend}");

                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine("Failed to send message to a socket: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                    break;
                }

            }
        }
    }
}
