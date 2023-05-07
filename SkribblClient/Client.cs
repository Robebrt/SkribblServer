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
        Form1 form1;
        RichTextBox errorTextBox;
        Boolean connection;
        RichTextBox chatRb;
        public Client(Form1 f1)
        {
            this.form1 = f1;
            //errorTextBox = form1.richTextBox2;
            chatRb = form1.richTextBox1;
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
                        if(form1.actionType == "<Create room>")
                        {
                            CreateRoom(player);
                            ReadMessage(chatRb);
                        }
                        else if(form1.actionType == "<Join room>")
                        {

                        }

                    }
                    //
                    //Task.Run(() =>
                    //{
                    //    ReadMessage(rb);
                    //});

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
            int roomId = player.GetRoom();

            // Encode the data string into a byte array.
            byte[] msg = Encoding.ASCII.GetBytes("<Create room>" + roomId + "<EOF>");

            // Send the data through the socket.
            int bytesSent = sender.Send(msg);

            // Receive the response from the remote device.

                int bytesRec = sender.Receive(bytes);
                chatRb.AppendText(Encoding.ASCII.GetString(bytes, 0, bytesRec) + "\n");
            }
        public void SendMessage(byte[] msg, RichTextBox rb, TextBox tb)
        {
            byte[] bytes = new byte[1024];
            try
            {
                int bytesSent = sender.Send(msg);
                tb.Text = "";
                // Receive the response from the remote device.
                
            }
            catch(Exception e)
            {
                SendError("Unexpected exception : {0} "+ e.ToString());
            }
        }
        public void ReadMessage(RichTextBox rb)
        {
            while (connection)
            {
                byte[] bytes = new byte[1024];
                try
                {   
                    int bytesRec = sender.Receive(bytes);
                    if(bytesRec == 0)
                    {
                        break;
                    }
                    //form.RunOnUiThread(() => rb.AppendText(Encoding.ASCII.GetString(bytes, 0, bytesRec) + "\n"));
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
