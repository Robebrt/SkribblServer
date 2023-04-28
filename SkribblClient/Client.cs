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
        Socket sender = null;
        Form1 form = null;
        RichTextBox errorTextBox = null;
        Boolean connection;
        public Client(Form1 form1)
        {
            form = form1;
            errorTextBox = form1.richTextBox2;
        }
        public void StartClient(RichTextBox rb)
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
                    
                    byte[] msg = Encoding.ASCII.GetBytes("User connected<EOF>");

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    
                    rb.AppendText(Encoding.ASCII.GetString(bytes, 0, bytesRec)+"\n");
                    Task.Run(() =>
                    {
                        readMessage(rb);
                    });

                }
                catch (ArgumentNullException ane)
                {
                    sendError("ArgumentNullException : {0} "+ane.ToString());
                }
                catch (SocketException se)
                {
                    sendError("SocketException : {0} "+se.ToString());
                }
                catch (Exception e)
                {
                    sendError("Unexpected exception : {0} "+e.ToString());
                }

            }
            catch (Exception e)
            {
                sendError(e.ToString());
            }
        }
        public void sendError(string err)
        {
            form.RunOnUiThread(() => errorTextBox.AppendText(err+"\n"));
        }
        public void sendMessage(byte[] msg, RichTextBox rb, TextBox tb)
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
                sendError("Unexpected exception : {0} "+ e.ToString());
            }
        }
        public void readMessage(RichTextBox rb)
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
                    form.RunOnUiThread(() => rb.AppendText(Encoding.ASCII.GetString(bytes, 0, bytesRec) + "\n"));
                }
                catch(Exception ex) 
                {
                    break;
                }
            }

        }
        public void stopClient()
        {
            connection = false;
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
