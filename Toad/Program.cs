using System;
using Ribbit.NET;

namespace TestTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            var message = new RibbitMessage("v1/summary");
            Console.WriteLine(message.parsed.TextBody);
        }
    }
}
