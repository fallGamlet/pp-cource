using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeoLocate;
using Wheathers;
using Telegram;
using Configuration;

namespace http_proj
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome my program");
            
            var config = new Config("./config.ini");
            var bot = new Telegram.Bot(config.getValue("telegram-bot"));
            var botTask = bot.Run();            
            botTask.Wait();

            Console.WriteLine("Program finished");
        }

        static void printArgs(string[] args) 
        {
            Console.WriteLine("Arguments:");
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
            Console.WriteLine("end arguments");
        }

        static int GetThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }

        static void WheatherUseCase() {
            var task = GetWheather();
            task.Wait();
            var whather = task.Result;
            Console.WriteLine($"Whather: ");
            Console.WriteLine(whather);
        }

        static async Task<Wheather> GetWheather() {
            var location = await new GeoLocation().GetAsync();
            return await new WheatherManager().getWheatherAsync(location.Lat, location.Lng);
        }
    }
}
