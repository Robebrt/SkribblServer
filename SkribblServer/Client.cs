using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using SkribblClient;
using System.Timers;

namespace SkribblServer
{
    public class Client
    {
        [JsonIgnore] int clNo;
        [JsonIgnore] Socket socket;
        public string username;
        public string avatar;
        public Boolean isDrawing;
        public int score;
        public System.Timers.Timer timer;
        int countdownSeconds;
        [JsonIgnore] StringBuilder StringBuilder;

        public void startClient(Socket socketIn, int clientNo)
        {
            this.socket = socketIn;
            this.clNo = clientNo;
            Thread clientThread = new Thread(doChat);
            clientThread.Start();
            StringBuilder = new StringBuilder();
        }
        public void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[2048];
            string dataFromClient = null;


            string rCount = null;
            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    int noBytesRecieved = socket.Receive(bytesFrom);
                    string stringBytesFrom = Encoding.ASCII.GetString(bytesFrom, 0, noBytesRecieved);
                    StringBuilder.Append(stringBytesFrom);
                    ParseRequest();
                    if (noBytesRecieved == 0)
                    {
                        foreach (var room in Server.roomsList)
                        {
                            room.Value.RemoveAll(client => client == this);
                        }
                        break;
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
        public void ParseRequest()
        {
            string serverResponse = null;
            Byte[] sendBytes = null;
            while (StringBuilder.ToString().Contains("<EOF>"))
            {
                string dataFromClient = StringBuilder.ToString().Substring(0, StringBuilder.ToString().IndexOf("<EOF>") + "<EOF>".Length);
                StringBuilder.Remove(0, dataFromClient.Length);

                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("<EOF>"));
                Console.WriteLine("From client: " + clNo + dataFromClient);
                serverResponse = RequestHandler(dataFromClient, this);
                sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                Console.WriteLine(serverResponse);
                socket.Send(sendBytes);

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
        public void SendMessageExcludeMe(List<Client> list, Byte[] bytesToSend)
        {
            foreach (Client client in list)
            {
                if (client != this)
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
        }
        private void TimerElapsed(ElapsedEventArgs e, int roomId)
        {
            // Trimite un mesaj la fiecare secundă trecută

            if (countdownSeconds > 0)
            {
                // Trimiteți valoarea countdownSeconds către clienți
                string message = countdownSeconds.ToString();
                if (Server.roomsList.TryGetValue(roomId, out List<Client> list))
                {
                    SendMessageToClients(list, Encoding.ASCII.GetBytes("<Timer>" + message));
                }
                countdownSeconds--; // Scădeți valoarea countdownSeconds cu 1
            }
            else
            {
                // Timer-ul a expirat, opriți-l sau executați alte acțiuni necesare
                if (Server.roomsList.TryGetValue(roomId, out List<Client> list))
                {
                    
                    int currentIndex = list.FindIndex(client => client.isDrawing == true);
                    Client currentClient = list[currentIndex];
                    currentClient.isDrawing = false;
                    int nextIndex;
                    if (currentIndex == list.Count - 1)
                    {
                        nextIndex = 0;
                    }
                    else
                    {
                        nextIndex = (currentIndex + 1) % list.Count;
                    }
                    Client nextClient = list[nextIndex];
                    nextClient.isDrawing = true;
                }
                timer.Stop();
                List<Player> playerList = list.Select(client => new Player(client.username, client.avatar, client.isDrawing)).ToList();
                string json = JsonConvert.SerializeObject(playerList);
                string response = "Room joined" + roomId + json + "<EOF>";
                byte[] bytesToSend = Encoding.ASCII.GetBytes(response);
                SendMessageToClients(list, bytesToSend);

                countdownSeconds = 10;
                timer.Start();
                // Restul acțiunilor pe care doriți să le efectuați când timer-ul ajunge la 0
            }

        }
        public string RequestHandler(string clientRequest, Client client)
        {
            string response = "";
            if (clientRequest.Contains("<Create room>"))
            { //create the room
                try
                {
                    List<Client> clientList = new List<Client>();
                    //int roomId = Int32.Parse(clientRequest.Replace("<Create room>", ""));
                    clientList.Add(client);

                    int roomId = Server.roomId++;
                    string playerJson = (clientRequest.Replace("<Create room>", ""));
                    Player player = JsonConvert.DeserializeObject<Player>(playerJson);
                    this.username = player.username;
                    this.avatar = player.avatar;
                    this.score = player.score;
                    player.isDrawing = true;
                    this.isDrawing = true;

                    countdownSeconds = 10; // Numărul de secunde pentru countdown

                    timer = new System.Timers.Timer(1000); // Interval de 1 secundă (1000 milisecunde)
                    timer.Elapsed += (sender, e) => TimerElapsed(e, roomId);
                    timer.Start();

                    Server.roomsList.Add(roomId, clientList);
                    List<Player> playerList = clientList.Select(client => player).ToList();
                    string json = JsonConvert.SerializeObject(playerList);
                    response = "Room created" + roomId + json + "<EOF>";
                }
                catch (FormatException ex)
                {
                    response = "Cannot create room";
                }
            }
            else if (clientRequest.Contains("<Join room>"))
            {
                try
                {

                    int roomId = Int32.Parse(clientRequest.Replace("<Join room>", ""));
                    if (Server.roomsList.TryGetValue(roomId, out List<Client> list))
                    {
                        list.Add(client);

                        List<Player> playerList = list.Select(client => new Player(client.username, client.avatar, client.isDrawing)).ToList();
                        string json = JsonConvert.SerializeObject(playerList);
                        response = "Room joined" + roomId + json + "<EOF>";
                        byte[] bytesToSend = Encoding.ASCII.GetBytes(response);
                        SendMessageToClients(list, bytesToSend);
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
            else if (clientRequest.Contains("<Draw>"))
            {
                //draw to all users
                string message = clientRequest.Replace("<Draw>", "");
                int roomId = Int32.Parse(message.Substring(0, 1));

                Byte[] sendBytes = Encoding.ASCII.GetBytes("<Draw>" + message.Substring(1, message.Length - 1) + "@");
                if (Server.roomsList.TryGetValue(roomId, out List<Client> list))
                {
                    SendMessageExcludeMe(list, sendBytes);
                    //response = "Message sent";
                }
                else
                {
                    response = "Cannot sent message";
                }
            }
            else if (clientRequest.Contains("<Chat>"))
            {
                //send message to users
                string message = clientRequest.Replace("<Chat>", "");
                int roomId = Int32.Parse(message.Substring(0, 1));
                Byte[] sendBytes = Encoding.ASCII.GetBytes(this.username + ": " + message.Substring(1, message.Length - 1));
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
            else if (clientRequest.Contains("<Set username>"))
            {
                string playerJson = clientRequest.Replace("<Set username>", "");
                Player player = JsonConvert.DeserializeObject<Player>(playerJson);
                this.username = player.username;
                this.avatar = player.avatar;
                this.score = player.score;
                this.isDrawing = player.isDrawing;
                response = "Username set";
            }
            return response;
        }

    }
}
