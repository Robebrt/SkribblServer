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
        int clNo;
        Socket socket;
        string username;
        public void startClient(Socket socketIn, int clientNo)
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
                    if (noBytesRecieved == 0)
                    {
                        foreach (var room in Server.roomsList)
                        {
                            room.Value.RemoveAll(client => client == this);
                        }
                        break;
                    }
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    Console.WriteLine(dataFromClient);
                    int indexEOF = dataFromClient.IndexOf("<EOF>");
                    if (indexEOF > 0)
                    {
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("<EOF>"));
                        Console.WriteLine("From client: " + clNo + dataFromClient);
                        serverResponse = RequestHandler(dataFromClient, this);
                        sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                        Console.WriteLine(serverResponse);
                        socket.Send(sendBytes);
                    }
                    
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Disconnected");
                    foreach (var room in Server.roomsList)
                    {
                        room.Value.RemoveAll(client => client == this);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                    break;
                }
                

            }
        }
        public void SendMessageToClients(List<Client> list, Byte[] bytesToSend)
        {
            foreach (Client client in list)
            {
                try
                {
                    int bytesSend = client.socket.Send(bytesToSend);
                    //Console.WriteLine($"{client.socket.RemoteEndPoint} {bytesSend}");

                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Failed to send message to a socket: " + ex.Message);
                }
            }
        }
        public string RequestHandler(string clientRequest, Client client)
        {
            string response = "";
            if(clientRequest.Contains("<Create room>"))
            { //create the room
                try
                {
                    List<Client> clientList = new List<Client>();
                    //int roomId = Int32.Parse(clientRequest.Replace("<Create room>", ""));
                    clientList.Add(client);
                    int roomId = Server.roomId++;
                    string username = (clientRequest.Replace("<Create room>", ""));
                    this.username = username;
                    Server.roomsList.Add(roomId, clientList);
                    response = "Room created"+roomId;
                }
                catch(FormatException ex)
                {
                    response = "Cannot create room";
                }
            }
            else if(clientRequest.Contains("<Join room>"))
            {
                try
                {

                    int roomId = Int32.Parse(clientRequest.Replace("<Join room>", ""));
                    if(Server.roomsList.TryGetValue(roomId,out List<Client> list))
                    {
                        list.Add(client);
                        response = "Room joined" + roomId;
                    }
                    else
                    {
                        response = "Room doesn't exist";
                    }
                    
                    
                }
                catch (FormatException ex)
                {
                    response = "Cannot join room";
                }
            }
            else if(clientRequest.Contains("<Draw>"))
            {
                //draw to all users
            }
            else if(clientRequest.Contains("<Chat>"))
            {
                //send message to users
                string message = clientRequest.Replace("<Chat>", "");
                int roomId = Int32.Parse(message.Substring(0, 1));
                Byte[] sendBytes = Encoding.ASCII.GetBytes(this.username+": "+message.Substring(1, message.Length - 1));
                if (Server.roomsList.TryGetValue(roomId, out List<Client> list))
                {
                    SendMessageToClients(list, sendBytes);
                    //response = "Message sent";
                }
                else
                {
                    response = "Cannot sent message";
                }
            }
            else if(clientRequest.Contains("<Set username>"))
            {
                string username = clientRequest.Replace("<Set username>", "");
                this.username = username;
                response = "Username set";
            }
            return response;
        }
    }
}
