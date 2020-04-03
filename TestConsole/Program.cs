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
            AliTrade.Lingla ll = new AliTrade.Lingla();
            
            var data = ll.GetTbkOrderDetails(Convert.ToDateTime("2020-04-02"), 1, "", 1, 100);
            Console.WriteLine(JsonConvert.SerializeObject(data));
            Console.ReadKey();
        }
    }
}
