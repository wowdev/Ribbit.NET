using System;
using Ribbit.Protocol;

namespace TestTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client("eu.version.battle.net", 1119);
            client.Connect();
            var response = client.Request("v1/products/wow/cdns");
            Console.WriteLine(response);
        }
    }
}
