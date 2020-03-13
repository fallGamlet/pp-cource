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
            Console.WriteLine("Welcome my program");
            printArgs(args);
            var count = 1;
            if (args.Length >= 1) {
                count = int.Parse(args[0]);
            }
            textsUseCase(count);
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

        static void textsUseCase(int count)
        {
            Console.WriteLine("Start get ToDo list");
            var task = getTodos(count);
            task.Wait();
            var todoList = task.Result;
            var text = getTotoListText(todoList);
            Console.WriteLine("Result: \n{0}", text);
        }
        

        static async Task<List<TodoItem>> getTodos(int count)
        {
            var items = new List<TodoItem>();
            for (int i = 0; i < count; i++)
            {
                items.Add(await getToDo(i+1));
            }
            return items;
        }

        static async Task<TodoItem> getToDo(long id)
        {
            var client = new RestClient("https://jsonplaceholder.typicode.com");
            client.UseNewtonsoftJson();
            var request = new RestRequest($"todos/{id}", DataFormat.Json);
            return await client.GetAsync<TodoItem>(request);
        }

        static String getTotoListText(List<TodoItem> items) {
            var textBuilder = new StringBuilder();
            foreach (var item in items)
            {
                textBuilder.AppendLine(item.ToString());
            }
            return textBuilder.ToString();
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
