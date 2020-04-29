using System;
using Newtonsoft.Json;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Hello dotnetcore!");
            AliTrade.Lingla p = new AliTrade.Lingla();
            var data = p.GetQueryTrade(Convert.ToDateTime("2020-04-08 19:22:43.223"), 1);
            Console.WriteLine(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data)));
            Console.ReadKey();
        }
    }
}
