using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace SkribblClient
{
    internal class Client
    {
        Socket sender;
        GameForm gameForm;
        Boolean connection;
        public Client(GameForm f1)
        {
            this.gameForm = f1;

        }
        public void StartClient(Player player)
        {
            byte[] bytes = new byte[1024];
            
            try
            {
                // Connect to a Remote server
                // Get Host IP Address that is used to establish a connection
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1
                // If a host has multiple addresses, you will get a list of addresses
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 3000);

                // Create a TCP/IP  socket.
                sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    // Connect to Remote EndPoint
                    sender.Connect(remoteEP);
                    connection = true;


                    // Encode the data string into a byte array.

                    //byte[] msg = Encoding.ASCII.GetBytes("User connected<EOF>");

                    //// Send the data through the socket.
                    //int bytesSent = sender.Send(msg);

                    //// Receive the response from the remote device.
                    //int bytesRec = sender.Receive(bytes);

                    if (player != null)
                    {
                        if(gameForm.actionType == "<Create room>")
                        {
                            CreateRoom(player);
                            //ReadMessage(chatRb);
                        }
                        else if(gameForm.actionType == "<Join room>")
                        {
                            JoinRoom(player);
                        }
                        Task.Run(() =>
                        {
                            ReadMessage();
                        });
                    }
                }
                catch (ArgumentNullException ane)
                {
                    SendError("ArgumentNullException : {0} "+ane.ToString());
                }
                catch (SocketException se)
                {
                    SendError("SocketException : {0} "+se.ToString());
                }
                catch (Exception e)
                {
                    SendError("Unexpected exception : {0} "+e.ToString());
                }

            }
            catch (Exception e)
            {
                SendError(e.ToString());
            }
        }
        public void SendError(string err)
        {
            //form.RunOnUiThread(() => errorTextBox.AppendText(err+"\n"));
        }
        public void CreateRoom(Player player)
        {
            byte[] bytes = new byte[1024];
            //int roomId = player.GetRoom();
            string username = player.username;
            // Encode the data string into a byte array.
            byte[] msg = Encoding.ASCII.GetBytes("<Create room>"+player.username+ "<EOF>");

            // Send the data through the socket.
            int bytesSent = sender.Send(msg);

            // Receive the response from the remote device.
            
            int bytesRec = sender.Receive(bytes);
            string msgReceived = Encoding.ASCII.GetString(bytes);
            if (msgReceived != "Cannot create room")
            {
                int roomId = Convert.ToInt32(msgReceived.Replace("Room created",""));
                gameForm.RunOnUiThread(() =>
                {
                    gameForm.joinRoom(roomId);
                });
            }
            else
            {
                gameForm.RunOnUiThread(() =>
                {
                    gameForm.joinRoom(-1);
                });
            }
                //gameForm.AddMessage(Encoding.ASCII.GetString(bytes, 0, bytesRec) + "\n");

        }
        public void JoinRoom(Player player)
        {
            byte[] bytes = new byte[1024];
            int roomId = gameForm.roomToJoin;
            string username = player.username;

            // Encode the data string into a byte array.
            byte[] setUserMsg = Encoding.ASCII.GetBytes("<Set username>" + player.username + "<EOF>");
            sender.Send(setUserMsg);
            byte[] setUserRec = new byte[1024];
            int bytesSetUserRec = sender.Receive(setUserRec);
            string msgSetUser = Encoding.ASCII.GetString(setUserRec);
            MessageBox.Show(msgSetUser);

            if (msgSetUser.Contains("Username set"))
            {
                byte[] msg = Encoding.ASCII.GetBytes("<Join room>" + roomId + "<EOF>");

                // Send the data through the socket.
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.
                //if (sender.Available > 0)
                //{
                //    int bytesRec = sender.Receive(bytes);
                //   // gameForm.AddMessage(Encoding.ASCII.GetString(bytes, 0, bytesRec) + "\n");
                //}
                int bytesRec = sender.Receive(bytes);
                string msgReceived = Encoding.ASCII.GetString(bytes);
                if (msgReceived != "Cannot join room" && msgReceived != "Room doesn't exist")
                {
                    int roomIdServer = Convert.ToInt32(msgReceived.Replace("Room joined", ""));
                    gameForm.RunOnUiThread(() =>
                    {
                        gameForm.joinRoom(roomIdServer);
                    });
                }
                else
                {
                    gameForm.RunOnUiThread(() =>
                    {
                        gameForm.joinRoom(-1);
                    });
                }
            }
            else
            {
                MessageBox.Show("Error");
            }
        }
        public void SendMessage(byte[] msg)
        {
            byte[] bytes = new byte[1024];
            try
            {
                int bytesSent = sender.Send(msg);
                
            }
            catch(Exception e)
            {
                SendError("Unexpected exception : {0} "+ e.ToString());
            }
        }
        public void ReadMessage()
        {
            while (connection)
            {
                byte[] bytes = new byte[1024];
                try
                {
                    if (sender.Available > 0)
                    {
                        int bytesRec = sender.Receive(bytes);
                        if (bytesRec == 0)
                        {
                            break;
                        }
                        gameForm.RunOnUiThread(() => gameForm.AddMessage(Encoding.ASCII.GetString(bytes, 0, bytesRec) + "\n"));
                        
                    }
                }
                catch(Exception ex) 
                {
                    break;
                }
            }

        }
        public void StopClient()
        {
            connection = false;
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
