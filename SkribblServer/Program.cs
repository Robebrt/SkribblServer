using System;

namespace SkribblServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Skribbl Server";

            Server.StartServer();

            Console.ReadKey();
        }
    }
}