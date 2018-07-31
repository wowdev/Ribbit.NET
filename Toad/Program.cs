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
            client.Connect();
            var response = client.Request("v1/summary");
            Console.WriteLine(response.ToString());
        }
    }
}
