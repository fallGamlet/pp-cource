using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Telegram
{
    public class TelegramBot
    {
        private RestClient client;
        private bool isRunning = false;
        private long lastUpdate = 0;

        public TelegramBot(string token) 
        {
            var baseUrl = $"https://api.telegram.org/bot{token}/";
            client = new RestClient(baseUrl);
            client.UseNewtonsoftJson();
        }

        public async Task Run() 
        {
            isRunning = true;
            while (isRunning) 
            {
                MakeProcessIteration();
                Thread.Sleep(1000);
            }
        }

        private async void MakeProcessIteration() 
        {
            try {
                var updates = await GetUpdates(lastUpdate);
                lastUpdate = GetLastUpdateId(updates, lastUpdate);
                ProcessUpdates(updates);
            } catch(Exception e) {
                Console.WriteLine("!!! Failure: ");
                Console.WriteLine(e);
                Console.WriteLine("!!! end failure");
            }
        }

        public void Stop() 
        {
            isRunning = false;
        }

        private async Task<List<UpdateJson>> GetUpdates(long offset)
        {
            var argOffset = offset > 0 ? $"&offset={offset}" : "";
            var path = $"getUpdates?limit=10{argOffset}";
            var request = new RestRequest(path, DataFormat.Json);
            request.AddHeader("content-type", "application/json");
            var response = await client.GetAsync<ResponseJson<List<UpdateJson>>>(request);
            return response.result ?? new List<UpdateJson>();
        }

        private long GetLastUpdateId(List<UpdateJson> items, long defaultValue) {
            if (items == null || items.Count == 0) {
                return defaultValue;
            } else {
                return items.Last()?.update_id ?? defaultValue;
            }
        }

        private void ProcessUpdates(List<UpdateJson> items) {
            items?.ForEach(item => ProcessUpdates(item));
        }

        private void ProcessUpdates(UpdateJson item) {
            Print(item);
        }

        private void Print<T>(T value) {
            var text = JsonConvert.SerializeObject(value);
            Console.WriteLine(text);
        }

    }

    class ResponseJson<T> {
        public bool ok;
        public T result;
    }

    class UpdateJson {
        public long update_id;
        public MessageJson message;
        public MessageJson edited_message;
        public MessageJson channel_post;
        public MessageJson edited_channel_post;
        public InlineQueryJson inline_query;
    }

    class MessageJson {
        public long message_id;
        public UserJson from;
        public long date;
        public ChatJson chat;
        public UserJson forward_from;
        public ChatJson forward_from_chat;
    }

    class InlineQueryJson {
        public string id;
        public UserJson from;
        public LocationJson location;
        public string query;
        public string offset;
    }

    class UserJson {
        public long id;
        public bool is_bot;
        public string username;
    }

    class ChatJson {
        public long id;
        public string type;
        public string title;
    }

    class LocationJson {
        public float longitude;
        public float latitude;
    }
}