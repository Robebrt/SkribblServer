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
                    //if(noBytesRecieved == 0)
                    //{
                    //    Server.clientList.Remove(this);
                    //    break;
                    //}    
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    Console.WriteLine(dataFromClient);
                    int indexEOF = dataFromClient.IndexOf("<EOF>");
                    if (indexEOF > 0)
                    {
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("<EOF>"));
                        Console.WriteLine("From client: " + clNo + dataFromClient);
                        serverResponse = RequestHandler(dataFromClient, this);
                        Console.WriteLine(serverResponse);
                        sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                        //Console.WriteLine(Server.clientList.Count());
                    }
                    
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
                    int roomId = Int32.Parse(clientRequest.Replace("<Create room>", ""));
                    clientList.Add(client);
                    Server.roomsList.Add(roomId, clientList);
                    response = "Room created";
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
                        response = "Room joined";
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
            }
            return response;
        }
    }
}
