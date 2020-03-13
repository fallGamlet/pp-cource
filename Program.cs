using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using RestSharp.Serializers.NewtonsoftJson;

namespace http_proj
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main start " + GetThreadId());

            textsUseCase();

            Console.WriteLine("Press <any key> for exit");
            Console.ReadKey();
        }

        static int GetThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }

        static void textsUseCase()
        {
            var task= getTexts();
            task.Wait();
            var texts = task.Result;
            var text = string.Join(", ", texts.ToArray());
            Console.WriteLine("Result: {0}", text);
        }

        static async Task<List<string>> getTexts()
        {
            var text1 = await getText1();
            var text2 = await getText2();
            return new List<string>() { text1, text2 };
        }

        static async Task<string> getText1()
        {
            var client = new RestClient("https://jsonplaceholder.typicode.com");
            client.UseNewtonsoftJson();
            var request = new RestRequest("todos/1", DataFormat.Json);
            TodoItem item = await client.GetAsync<TodoItem>(request);
            
            return item.ToString();
        }

        static async Task<string> getText2()
        {
            var client = new RestClient("https://jsonplaceholder.typicode.com");
            client.UseNewtonsoftJson();
            var request = new RestRequest("todos/2", DataFormat.Json);
            TodoItem item = await client.GetAsync<TodoItem>(request);
            
            return item.ToString();
        }
    }

    class TodoItem {
        public long userId = 0;
        public long id = 0;
        public string title = "";
        public bool completed = false;

        override public string ToString() {
            return $"(id: {id}, userId: {userId}, title: {title}, completed: {completed})";
        }
    }

}
