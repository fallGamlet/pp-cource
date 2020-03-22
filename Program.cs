using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeoLocate;

namespace http_proj
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome my program");
            printArgs(args);
            
            geoLocationUseCase();

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

        static void geoLocationUseCase() {
            var geoLocate = new GeoLocation();
            var task = geoLocate.GetAsync();
            task.Wait();
            var location = task.Result;
            Console.WriteLine($"Location: {location}");
        }
    }
}
