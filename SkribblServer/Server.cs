using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SkribblServer
{
    internal class Server
    {


        //public static Dictionary<(int, int)> roomList = new Dictionary<(int, int)>();
        public static Dictionary<int, List<Client>> roomsList = new Dictionary<int, List<Client>>();
        public static int roomId = 0;
        public static string[] words;

        public static void StartServer()
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostEntry.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3000);
            int counter;
            string server = "localhost";
            string database = "skribbl_game_db";
            string uid = "root";
            string password = "admin";
            SkribblDbConnection dbCon = new SkribblDbConnection(server, database, uid, password);
            words = dbCon.GetAllWords();

            try
            {
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(ipEndPoint);
                listener.Listen(10);
                Console.WriteLine("Waiting for a connection...");
                Socket handler;


                counter = 0;
                while (true)
                {
                    counter++;
                    handler = listener.Accept();
                    Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started!");
                    Client client = new Client();
                    client.startClient(handler, counter);
                    //clientList.Add(client);

                }
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }


        }
    }
}
