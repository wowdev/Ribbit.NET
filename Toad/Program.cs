using System;

using Ribbit.Constants;
using Ribbit.Protocol;

namespace TestTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client(Region.US);
            var response = client.Request("v1/products/wow/versions");
            Console.WriteLine(response.ToString());
            response = client.Request("v1/products/pro/versions");
            Console.WriteLine(response.ToString());
        }
    }
}
